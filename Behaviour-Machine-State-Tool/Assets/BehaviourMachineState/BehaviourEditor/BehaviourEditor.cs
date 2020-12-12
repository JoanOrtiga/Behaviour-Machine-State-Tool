using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace BehaviourMachineState.Editor
{
    public class BehaviourEditor : EditorWindow
    {
        #region VARIABLES
        static List<BaseNode> windows = new List<BaseNode>();
        Vector3 mousePosition;

        bool makeTransition;

        bool clickedOnWindow;
        int selectedIndex;
        BaseNode selectedNode;

        public static BehaviourGraph currentGraph;

        static GraphNode graphNode;

        public enum UserActions
        {
            addState, addTransitionNode, deleteNode, commentNode
        }
        #endregion

        #region INIT
        [MenuItem("BehaviourEditor/Editor")]
        static void ShowEditor()
        {
            BehaviourEditor editor = EditorWindow.GetWindow<BehaviourEditor>();

            editor.minSize = new Vector2(800, 600);
        }

        private void OnEnable()
        {
            if (graphNode == null)
            {
                graphNode = CreateInstance<GraphNode>();

                graphNode.windowRect = new Rect(10, position.height * 0.7f, 200, 100);
                graphNode.windowTitle = "Graph";
            }

            windows.Clear();

            windows.Add(graphNode);

            LoadGraph();
        }
        #endregion

        #region GUI METHODS
        private void OnGUI()
        {
            Event e = Event.current;
            mousePosition = e.mousePosition;

            UserInput(e);
            DrawWindows();
        }

        void DrawWindows()
        {
            BeginWindows();

            foreach (BaseNode item in windows)
            {
                item.DrawCurve();
            }

            for (int i = 0; i < windows.Count; i++)
            {
                windows[i].windowRect = GUI.Window(i, windows[i].windowRect, DrawNodeWindow, windows[i].windowTitle);
            }

            EndWindows();
        }

        void DrawNodeWindow(int id)
        {
            windows[id].DrawWindow();
            GUI.DragWindow();
        }

        void UserInput(Event e)
        {
            if (e.button == 1 && !makeTransition)
            {
                if (e.type == EventType.MouseDown)
                {
                    RightClick(e);
                }
            }

            if (e.button == 0 && !makeTransition)
            {
                if (e.type == EventType.MouseDown)
                {
                    LeftClick(e);
                }

                if (e.type == EventType.MouseDrag)
                {
                    for (int i = 0; i < windows.Count; i++)
                    {
                        if (windows[i].windowRect.Contains(e.mousePosition))
                        {
                            if (currentGraph != null)
                                currentGraph.SetNode(windows[i]);
                            break;
                        }
                    }
                }
            }

            if (e.keyCode == KeyCode.Delete)
            {
                if (e.type == EventType.KeyDown)
                {
                    DeleteKey(e);
                }
            }
        }

        void RightClick(Event e)
        {
            selectedIndex = -1;
            clickedOnWindow = false;
            for (int i = 0; i < windows.Count; i++)
            {
                if (windows[i].windowRect.Contains(e.mousePosition))
                {
                    clickedOnWindow = true;
                    selectedNode = windows[i];

                    selectedIndex = i;
                    break;
                }
            }

            if (!clickedOnWindow)
            {
                AddNewNode(e);
            }
            else
            {
                ModifyNode(e);
            }
        }

        void DeleteKey(Event e)
        {
            selectedNode = null;

            for (int i = 0; i < windows.Count; i++)
            {
                if (windows[i].windowRect.Contains(e.mousePosition))
                {
                    clickedOnWindow = true;
                    selectedNode = windows[i];
                    break;
                }
            }

            if (clickedOnWindow)
            {
                ContextCallBack(UserActions.deleteNode);
            }

            e.Use();
        }


        void AddNewNode(Event e)
        {
            GenericMenu menu = new GenericMenu();

            if (currentGraph != null)
            {
                //    menu.AddSeparator("");
                menu.AddItem(new GUIContent("Add State"), false, ContextCallBack, UserActions.addState);
                menu.AddItem(new GUIContent("Add Comment"), false, ContextCallBack, UserActions.commentNode);


            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Add State"));
                menu.AddDisabledItem(new GUIContent("Add Comment"));
            }

            menu.ShowAsContext();
            e.Use();

        }

        void ModifyNode(Event e)
        {
            GenericMenu menu = new GenericMenu();

            if (selectedNode is StateNode)
            {
                StateNode stateNode = (StateNode)selectedNode;
                if (stateNode.currentState != null)
                {
                    menu.AddItem(new GUIContent("Add Transition"), false, ContextCallBack, UserActions.addTransitionNode);
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent("Add Transition"));
                }

                menu.AddItem(new GUIContent("Delete"), false, ContextCallBack, UserActions.deleteNode);
            }
            else if (selectedNode is CommentNode)
            {
                menu.AddItem(new GUIContent("Delete"), false, ContextCallBack, UserActions.deleteNode);

            }
            else if (selectedNode is TransitionNode)
            {
                menu.AddItem(new GUIContent("Delete"), false, ContextCallBack, UserActions.deleteNode);

            }

            menu.ShowAsContext();
            e.Use();
        }

        void ContextCallBack(object o)
        {
            UserActions a = (UserActions)o;
            switch (a)
            {
                case UserActions.addState:

                    AddStateNode(mousePosition);

                    break;

                case UserActions.addTransitionNode:

                    if (selectedNode is StateNode)
                    {
                        StateNode from = (StateNode)selectedNode;
                        Transition transition = from.AddTransition();
                        AddTransitionNode(from.currentState.transitions.Count, transition, from);
                    }

                    break;

                case UserActions.commentNode:

                    AddCommentNode(mousePosition);

                    break;

                default:
                    break;

                case UserActions.deleteNode:

                    if (selectedNode is StateNode)
                    {
                        StateNode target = (StateNode)selectedNode;

                        target.ClearReferences();
                        windows.Remove(target);
                    }

                    if (selectedNode is TransitionNode)
                    {
                        TransitionNode target = (TransitionNode)selectedNode;
                        windows.Remove(target);

                        if (target.enterState.currentState.transitions.Contains(target.targetTransition))
                        {
                            target.enterState.currentState.transitions.Remove(target.targetTransition);
                        }
                    }

                    if (selectedNode is CommentNode)
                    {
                        windows.Remove(selectedNode);
                    }
                    break;
            }
        }

        void LeftClick(Event e)
        {

        }
        #endregion

        #region HELPER METHODS

        public static StateNode AddStateNode(Vector2 pos)
        {
            StateNode stateNode = CreateInstance<StateNode>();
            stateNode.windowRect = new Rect(pos.x, pos.y, 200, 300);
            stateNode.windowTitle = "State";

            windows.Add(stateNode);

           // currentGraph.SetStateNode(stateNode);

            return stateNode;
        }

        public static CommentNode AddCommentNode(Vector2 pos)
        {
            CommentNode commentNode = CreateInstance<CommentNode>();

            commentNode.windowRect = new Rect(pos.x, pos.y, 200, 100);
            commentNode.windowTitle = "Comment";

            windows.Add(commentNode);

            return commentNode;
        }

        public static TransitionNode AddTransitionNode(int index, Transition transition, StateNode from)
        {
            Rect fromRect = from.windowRect;
            fromRect.x += 50;
            float targetY = fromRect.y - fromRect.height;

            if (from.currentState != null)
            {
                targetY += index * 100;
            }

            fromRect.y = targetY;

            fromRect.x += 200 + 100;
            fromRect.y += (fromRect.height * 0.7f);

            return AddTransitionNode(new Vector2(fromRect.x, fromRect.y), transition, from);
        }

        public static TransitionNode AddTransitionNode(Vector2 pos, Transition transition, StateNode from)
        {
            TransitionNode transitionNode = CreateInstance<TransitionNode>();
            transitionNode.Init(from, transition);

            transitionNode.windowRect = new Rect(pos.x, pos.y, 200, 80);
            transitionNode.windowTitle = "Condition Check";
            windows.Add(transitionNode);

            from.dependencies.Add(transitionNode);

            return transitionNode;
        }

        public static void DrawNodeCurve(Rect start, Rect end, bool left, Color curveColor)
        {
            Vector3 startPos = new Vector3(

                (left) ? start.x + start.width : start.x,
                start.y + (start.height * 0.5f),
                0
                );

            Vector3 endPos = new Vector3(end.x + (end.width * 0.5f), end.y + (end.height * 5f), 0);

            Vector3 startTan = startPos + Vector3.right * 50;
            Vector3 endTan = endPos + Vector3.left * 50;

            Color shadow = new Color(0, 0, 0, 0.06f);

            for (int i = 0; i < 3; i++)
            {
                Handles.DrawBezier(startPos, endPos, startTan, endTan, shadow, null, (i + 1) * 0.5f);
            }

            Handles.DrawBezier(startPos, endPos, startTan, endTan, curveColor, null, 1);
        }

        public static void ClearWindowsFromList(List<BaseNode> nodes)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (windows.Contains(nodes[i]))
                {
                    windows.Remove(nodes[i]);
                }
            }
        }

        public static void LoadGraph()
        {
            windows.Clear();
            windows.Add(graphNode);

            if (currentGraph == null)
                return;

            currentGraph.Init();

            List<SavedStateNode> savedNodes = new List<SavedStateNode>();
            savedNodes.AddRange(currentGraph.savedStateNodes);

            currentGraph.savedStateNodes.Clear();

            for (int i = savedNodes.Count - 1; i >= 0; i--)
            {
                StateNode stateNode = AddStateNode(savedNodes[i].position);
                stateNode.currentState = savedNodes[i].state;

                stateNode.collapse = savedNodes[i].isCollapsed;
                currentGraph.SetStateNode(stateNode);

                //Load transitions;
            }
        }
        #endregion
    }
}
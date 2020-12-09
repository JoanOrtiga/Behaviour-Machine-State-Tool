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
        #endregion

        #region GUI METHODS
        private void OnGUI()
        {
            Event e = Event.current;
            mousePosition = e.mousePosition;

            UserInput(e);
            DrawWindows();
        }

        private void OnEnable()
        {
              windows.Clear();
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

            //    menu.AddSeparator("");
            menu.AddItem(new GUIContent("Add State"), false, ContextCallBack, UserActions.addState);
            menu.AddItem(new GUIContent("Add Comment"), false, ContextCallBack, UserActions.commentNode);

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

                    StateNode stateNode = new StateNode
                    {
                        windowRect = new Rect(mousePosition.x, mousePosition.y, 200, 300),
                        windowTitle = "State"
                    };

                    windows.Add(stateNode);

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

                    CommentNode commentNode = new CommentNode
                    {
                        windowRect = new Rect(mousePosition.x, mousePosition.y, 200, 100),
                        windowTitle = "Comment"
                    };

                    windows.Add(commentNode);
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
                    
                    if(selectedNode is TransitionNode)
                    {
                        TransitionNode target = (TransitionNode)selectedNode;
                        windows.Remove(target);

                        if (target.enterState.currentState.transitions.Contains(target.targetTransition))
                        {
                            target.enterState.currentState.transitions.Remove(target.targetTransition);
                        }
                    }

                    if(selectedNode is CommentNode)
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

            TransitionNode transitionNode = CreateInstance<TransitionNode>();
            transitionNode.Init(from, transition);

            transitionNode.windowRect = new Rect(fromRect.x + 200 + 100, fromRect.y + (fromRect.height * 0.7f), 200, 80);
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
        #endregion
    }
}
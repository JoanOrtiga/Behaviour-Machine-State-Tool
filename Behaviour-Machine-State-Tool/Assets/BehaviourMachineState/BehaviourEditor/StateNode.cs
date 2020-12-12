using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.IO;


namespace BehaviourMachineState.Editor
{
    public class StateNode : BaseNode
    {
        public bool collapse;
        bool previousCollapse;
        public State currentState;
        public bool isDuplicated;

        public State previousState;

        SerializedObject serializedState;

        ReorderableList onEnterList;
        ReorderableList onStateList;    
        ReorderableList onExitList;

        public List<BaseNode> dependencies = new List<BaseNode>();

        public override void DrawWindow()
        {
            if (currentState == null)
            {
                EditorGUILayout.LabelField("Add state to modify");
            }
            else
            {
                if (!collapse)
                {
              //      windowRect.height = 300;
                }
                else
                {
                    windowRect.height = 100;
                }

                collapse = EditorGUILayout.Toggle(" ", collapse);
            }

            currentState = (State)EditorGUILayout.ObjectField(currentState, typeof(State), false);

            if(previousCollapse != collapse)
            {
                previousCollapse = collapse;
                //BehaviourEditor.currentGraph.SetStateNode(this);
            }

            if (previousState != currentState)
            {
                serializedState = null;

                isDuplicated = BehaviourEditor.currentGraph.isStateNodeDuplicate(this);

                if (!isDuplicated)
                {
                    BehaviourEditor.currentGraph.SetStateNode(this);
                    previousState = currentState;


                    for (int i = 0; i < currentState.transitions.Count; i++)
                    {

                    }
                }
            }

            if (isDuplicated)
            {

                EditorGUILayout.LabelField("State is duplicated!");
                windowRect.height = 100;
                return;
            }

            if (currentState != null)
            {
                if (serializedState == null)
                {
                    serializedState = new SerializedObject(currentState);
                    onEnterList = new ReorderableList(serializedState, serializedState.FindProperty("onEnter"), true,true,true,true);
                    onStateList = new ReorderableList(serializedState, serializedState.FindProperty("onState"), true,true,true,true);
                    onExitList = new ReorderableList(serializedState, serializedState.FindProperty("onExit"), true,true,true,true);
                }

                if (!collapse)
                {
                    serializedState.Update();
                    HandleReordeableList(onEnterList, "On Enter");
                    HandleReordeableList(onStateList, "On State");
                    HandleReordeableList(onExitList, "On Exit");

                    EditorGUILayout.LabelField("");
                    onEnterList.DoLayoutList();
                    EditorGUILayout.LabelField("");
                    onStateList.DoLayoutList();
                    EditorGUILayout.LabelField("");
                    onExitList.DoLayoutList();

                    serializedState.ApplyModifiedProperties();

                    float standard = 350;
                    standard += (onStateList.count + onEnterList.count + onExitList.count) * EditorGUIUtility.singleLineHeight;
                    windowRect.height = standard;
                }
            }
        }

        private void HandleReordeableList(ReorderableList list, string targetName)
        {
            list.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, targetName);
            };

            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = list.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
            };
        }

        public override void DrawCurve()
        {

        }

        public Transition AddTransition()
        {
            return currentState.AddTransition();
        }

        public void ClearReferences()
        {
            BehaviourEditor.ClearWindowsFromList(dependencies);
            dependencies.Clear();
        }
    }

}

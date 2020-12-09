using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.IO;

using BehaviourMachineState.Editor;

namespace BehaviourMachineState
{
    public class StateNode : BaseNode
    {
        bool collapse;
        public UnityEditor.Animations.AnimatorState currentState;
        UnityEditor.Animations.AnimatorState previousState;

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
                    windowRect.height = 300;
                }
                else
                {
                    windowRect.height = 100;
                }

                collapse = EditorGUILayout.Toggle(" ", collapse);
            }

            currentState = (UnityEditor.Animations.AnimatorState)EditorGUILayout.ObjectField(currentState, typeof(UnityEditor.Animations.AnimatorState), false);

            if (previousState != currentState)
            {
                serializedState = null;

                previousState = currentState;
                ClearReferences();

                for (int i = 0; i < currentState.transitions.Count; i++)
                {
                    dependencies.Add(BehaviourEditor.AddTransitionNode(i, currentState.transitions[i], this));
                }
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
                    standard += onStateList.count * EditorGUIUtility.singleLineHeight;
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

        public UnityEditor.Animations.AnimatorTransition AddTransition()
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

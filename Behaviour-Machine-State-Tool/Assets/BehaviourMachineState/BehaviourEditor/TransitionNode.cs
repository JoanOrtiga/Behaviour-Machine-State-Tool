using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BehaviourMachineState.Editor
{
    public class TransitionNode : BaseNode
    {
        public bool isDuplicated;
        public Condition targetCondition;
        public Condition previousCondition;

        public Transition transition;

        public StateNode enterState;
        public StateNode targetState;


        public void Init(StateNode enterState, Transition transition)
        {
            this.enterState = enterState;
        }

        public override void DrawWindow()
        {
            EditorGUILayout.LabelField("");
            targetCondition = (Condition)EditorGUILayout.ObjectField(targetCondition, typeof(Condition), false);

            if (targetCondition == null)
            {
                EditorGUILayout.LabelField("No Condition!");
            }
            else
            {
                if (isDuplicated)
                {
                    EditorGUILayout.LabelField("Duplicated condition");
                }
                else
                {
             /*       if (transition != null)
                        transition.disable = EditorGUILayout.Toggle("Disable", transition.disable);*/
                }
            }

            if (previousCondition != targetCondition)
            {
                isDuplicated = BehaviourEditor.currentGraph.IsTransitionDuplicated(this);
                if (!isDuplicated)
                {
                    BehaviourEditor.currentGraph.SetNode(this);

                }

                previousCondition = targetCondition;
            }
        }

        public override void DrawCurve()
        {
            if (enterState)
            {
                Rect rect = windowRect;
                rect.y += windowRect.height * 0.5f;
                rect.width = 1;
                rect.height = 1;

                BehaviourEditor.DrawNodeCurve(enterState.windowRect, rect, true, Color.black);
            }
        }
    }
}


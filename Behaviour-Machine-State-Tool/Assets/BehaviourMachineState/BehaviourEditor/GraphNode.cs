using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BehaviourMachineState.Editor
{
    public class GraphNode : BaseNode
    {
        BehaviourGraph previousGraph;

        public override void DrawWindow()
        {
            if (BehaviourEditor.currentGraph == null)
            {
                EditorGUILayout.LabelField("Add Graph To Modify");
            }

            BehaviourEditor.currentGraph = (BehaviourGraph)EditorGUILayout.ObjectField(BehaviourEditor.currentGraph, typeof(BehaviourGraph), false);

            if(BehaviourEditor.currentGraph == null)
            {
                if(previousGraph != null)
                {
                    //Clear Windows
                    previousGraph = null;
                }

                EditorGUILayout.LabelField("No Graph Assigned!");
                return;
            }

            if (previousGraph != BehaviourEditor.currentGraph)
            {
                previousGraph = BehaviourEditor.currentGraph;

                BehaviourEditor.LoadGraph();
            }
        }

        public override void DrawCurve()
        {
            base.DrawCurve();
        }
    }
}


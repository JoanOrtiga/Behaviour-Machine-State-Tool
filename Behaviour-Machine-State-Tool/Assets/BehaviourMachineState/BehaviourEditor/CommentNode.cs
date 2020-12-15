using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BehaviourMachineState.Editor
{
    public class CommentNode : BaseNode
    {
        public string comment = "This is a comment";
        private string previousComment;

        public override void DrawWindow()
        {
            comment = GUILayout.TextArea(comment, 200);

            if(previousComment != comment)
            {
                previousComment = comment;
                BehaviourEditor.currentGraph.SetNode(this);
            }
        }
    }
}


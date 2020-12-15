using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourMachineState.Editor;

namespace BehaviourMachineState
{
    [CreateAssetMenu]
    public class BehaviourGraph : ScriptableObject
    {
        public List<SavedStateNode> savedStateNodes = new List<SavedStateNode>();

        Dictionary<StateNode, SavedStateNode> stateNodesDict = new Dictionary<StateNode, SavedStateNode>();
        Dictionary<State, StateNode> stateDict = new Dictionary<State, StateNode>();

        public List<SavedCommentNode> savedCommentNodes = new List<SavedCommentNode>();
        Dictionary<CommentNode, SavedCommentNode> commentNodesDict = new Dictionary<CommentNode, SavedCommentNode>();


        public void Init()
        {
            stateNodesDict.Clear();
            stateDict.Clear();

            commentNodesDict.Clear();
        }

        public void SetNode(BaseNode node)
        {
            if (node is StateNode)
            {
                SetStateNode((StateNode)node);
            }

            if (node is TransitionNode)
            {
                SetTransitionNode((TransitionNode)node);
            }

            if (node is CommentNode)
            {
                SetCommentNode((CommentNode)node);
            }
        }

        #region State Nodes

        public bool isStateNodeDuplicate(StateNode node)
        {
            StateNode prevNode = null;
            stateDict.TryGetValue(node.currentState, out prevNode);

            if (prevNode != null)
            {
                return true;
            }

            return false;
        }

        void SetStateNode(StateNode node)
        {

            if (node.isDuplicated)
                return;

            if (node.previousState != null)
            {
                stateDict.Remove(node.previousState);
            }

            if (node.currentState == null)
            {
                return;
            }

            SavedStateNode savedNode = GetSavedState(node);

            if (savedNode == null)
            {
                savedNode = new SavedStateNode();
                savedStateNodes.Add(savedNode);
                stateNodesDict.Add(node, savedNode);
            }

            savedNode.state = node.currentState;
            savedNode.position = new Vector2(node.windowRect.x, node.windowRect.y);
            savedNode.isCollapsed = node.collapse;

            stateDict.Add(savedNode.state, node);
        }

        void ClearStateNode(StateNode node)
        {
            SavedStateNode savedNode = GetSavedState(node);

            if (savedNode != null)
            {
                savedStateNodes.Remove(savedNode);
                stateNodesDict.Remove(node);
            }
        }

        SavedStateNode GetSavedState(StateNode node)
        {
            SavedStateNode savedNode = null;
            stateNodesDict.TryGetValue(node, out savedNode);
            return savedNode;
        }

        StateNode GetStateNode(State state)
        {
            StateNode stateNode = null;
            stateDict.TryGetValue(state, out stateNode);
            return stateNode;
        }

        #endregion

        #region Transition Nodes

        public bool IsTransitionDuplicated(TransitionNode node)
        {
            SavedStateNode savedStateNode = GetSavedState(node.enterState);

            return savedStateNode.IsTransitionDuplicated(node);
        }

        public void SetTransitionNode(TransitionNode node)
        {
            SavedStateNode savedStateNode = GetSavedState(node.enterState);
            savedStateNode.SetTransitionNode(node);
        }

        public void ClearTransitionNode()
        {

        }


        #endregion

        #region Comment Nodes

        void SetCommentNode(CommentNode node)
        {
            SavedCommentNode savedNode = GetSavedComment(node);

            if (savedNode == null)
            {
                savedNode = new SavedCommentNode();
                savedCommentNodes.Add(savedNode);
                commentNodesDict.Add(node, savedNode);
            }

            savedNode.comment = node.comment;
            savedNode.position = new Vector2(node.windowRect.x, node.windowRect.y);
        }

        SavedCommentNode GetSavedComment(CommentNode node)
        {
            SavedCommentNode savedNode = null;
            commentNodesDict.TryGetValue(node, out savedNode);
            return savedNode;
        }

        #endregion
    }

    [System.Serializable]
    public class SavedStateNode
    {
        public State state;
        public Vector2 position;
        public bool isCollapsed;

        public List<SavedCondition> savedConditionNodes = new List<SavedCondition>();

        Dictionary<TransitionNode, SavedCondition> transitionNodesDict = new Dictionary<TransitionNode, SavedCondition>();

        Dictionary<Condition, TransitionNode> conditionDict = new Dictionary<Condition, TransitionNode>();

        public void Init()
        {
            transitionNodesDict.Clear();
            conditionDict.Clear();
        }

        public bool IsTransitionDuplicated(TransitionNode node)
        {
            TransitionNode prevNode = null;
            conditionDict.TryGetValue(node.targetCondition, out prevNode);

            if (prevNode != null)
            {
                return true;
            }

            return false;
        }

        public void SetTransitionNode(TransitionNode node)
        {
            if (node.isDuplicated)
                return;

            if(node.previousCondition != null)
            {
                conditionDict.Remove(node.targetCondition);
            }

            if (node.targetCondition == null)
                return;

            SavedCondition savedCondition = GetSavedCondition(node);
            if(savedCondition == null)
            {
                savedCondition = new SavedCondition();
                savedConditionNodes.Add(savedCondition);
                transitionNodesDict.Add(node, savedCondition);
                node.transition = node.enterState.currentState.AddTransition();
            }

            savedCondition.transition = node.transition;
            savedCondition.condition = node.targetCondition;
            savedCondition.transition.condition = savedCondition.condition;
            savedCondition.position = new Vector2(node.windowRect.x, node.windowRect.y);
            conditionDict.Add(savedCondition.condition, node);
        }

        SavedCondition GetSavedCondition(TransitionNode node)
        {
            SavedCondition savedNode = null;
            transitionNodesDict.TryGetValue(node, out savedNode);
            return savedNode;
        }

        TransitionNode GetTransitionNode(Transition transition)
        {
            TransitionNode stateNode = null;
            conditionDict.TryGetValue(transition.condition, out stateNode);
            return stateNode;
        }
    }

    [System.Serializable]
    public class SavedCondition
    {
        public Transition transition;
        public Condition condition;
        public Vector2 position;
    }

    [System.Serializable]
    public class SavedCommentNode
    {
        public string comment;
        public Vector2 position;
    }

}


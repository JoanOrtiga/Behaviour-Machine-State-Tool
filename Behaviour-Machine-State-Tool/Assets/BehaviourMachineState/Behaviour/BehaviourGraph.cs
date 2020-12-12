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

        public void Init()
        {
            stateNodesDict.Clear();
            stateDict.Clear();
        }

        public void SetNode(BaseNode node)
        {
            if(node is StateNode)
            {
                SetStateNode((StateNode) node);
            }

            if(node is TransitionNode)
            {

            }

            if(node is CommentNode)
            {

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

        public void SetStateNode(StateNode node)
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
            
            if(savedNode == null)
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

        public void ClearStateNode(StateNode node)
        {
            SavedStateNode savedNode = GetSavedState(node);
            
            if(savedNode != null)
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

        public StateNode GetStateNode(State state)
        {
            StateNode stateNode = null;
            stateDict.TryGetValue(state, out stateNode);
            return stateNode;
        }

        #endregion
    }

    [System.Serializable]
    public class SavedStateNode
    {
        public State state;
        public Vector2 position;
        public bool isCollapsed;
    }

    public class SavedTransition
    {
        
    }
}


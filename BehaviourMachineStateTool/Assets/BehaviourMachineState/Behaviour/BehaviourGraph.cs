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

        public void SetStateNode(StateNode node)
        {
            SavedStateNode savedNode = GetSavedState(node);
            
            if(savedNode == null)
            {
                savedNode = new SavedStateNode();
                savedStateNodes.Add(savedNode);
                stateNodesDict.Add(node, savedNode);
            }

            savedNode.state = node.currentState;
            savedNode.position = new Vector2(node.windowRect.x, node.windowRect.y);
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
    }

    [System.Serializable]
    public class SavedStateNode
    {
        public State state;
        public Vector2 position;
    }

    public class SavedTransition
    {
        
    }
}


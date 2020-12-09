using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourMachineState
{
    [CreateAssetMenu]
    public class State : ScriptableObject
    {
        public StateActions[] onState;
        public StateActions[] onEnter;
        public StateActions[] onExit;

        public List<Transition> transitions = new List<Transition>();

        public void OnEnter(StateManager states)
        {
            ExecuteActions(states, onEnter);
        }

        public void OnExit(StateManager states)
        {
            ExecuteActions(states, onExit);
        }

        public void OnStates(StateManager states)
        {
            ExecuteActions(states, onState);
            CheckTransitions(states);
        }

        public void ExecuteActions(StateManager states, StateActions[] stateActions)
        {
            for (int i = 0; i < stateActions.Length; i++)
            {
                if (stateActions[i] != null)
                {
                    stateActions[i].Execute(states);
                }
            }
        }

        public void CheckTransitions(StateManager states)
        {
            for (int i = 0; i < transitions.Count; i++)
            {
                if (transitions[i].disable)
                    continue;

                if (transitions[i].condition.CheckCondition(states))
                {
                    if (transitions[i].targetState != null)
                    {
                        states.currentState = transitions[i].targetState;

                        OnExit(states);
                        states.currentState.OnEnter(states);
                    }
                        

                    return;
                }
            }
        }

        public Transition AddTransition()
        {
            Transition retVal = new Transition();
            transitions.Add(retVal);

            return retVal;
        }
    }
}


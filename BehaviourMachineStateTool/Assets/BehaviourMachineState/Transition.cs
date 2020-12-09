using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourMachineState
{
    [System.Serializable]
    public class Transition
    {
        public Condition condition;
        public State targetState;
        public bool disable;
    }
}


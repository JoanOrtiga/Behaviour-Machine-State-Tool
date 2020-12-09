using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BehaviourMachineState
{
    public class StateManager : MonoBehaviour
    {
        public float health; //TODELETE;
        public State currentState;

        [HideInInspector]
        public float delta;
        public Transform stateTransform;

        private void Start()
        {
            stateTransform = this.transform;
        }


        private void Update()
        {
            if(currentState != null)
            {
                currentState.OnStates(this);
            }
        }
    }

}
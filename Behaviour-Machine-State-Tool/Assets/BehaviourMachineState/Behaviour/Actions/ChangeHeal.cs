using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourMachineState
{
    [CreateAssetMenu(menuName ="Actions/Test/ChangeHeal")]
    public class ChangeHeal : StateActions
    {
        public override void Execute(StateManager states)
        {
            states.health += 10;
        }
    }
}


﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourMachineState
{
    public abstract class Condition : ScriptableObject
    {
        public abstract bool CheckCondition(StateManager state);
    }
}


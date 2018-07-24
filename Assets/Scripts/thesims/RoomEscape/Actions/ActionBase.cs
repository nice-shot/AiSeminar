using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Ai.Goap;

namespace RoomEscape {
    public abstract class ActionBase : GoapAction {

        public static List<IStateful> GetTargetsFromMemory<T>(GoapAgent agent) where T : Component, IStateful {
            Memory memory = agent.GetComponent<Memory>();
            if (memory == null) {
                return GetTargets<T>();
            }

            return memory.GetTargets<T>();
            
        }
    }
}
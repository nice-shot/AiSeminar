using UnityEngine;
using System.Collections;

using Ai.Goap;

namespace RoomEscape {
    public class SafeSpot : MonoBehaviour, IStateful {
        public bool isClear;

        private readonly State state = new State();

        public State GetState() {
            state[States.CLEAR] = new StateValue(isClear);
            return state;
        }
    }
}
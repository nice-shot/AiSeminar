using UnityEngine;

using Infra.Collections;
using Ai.Goap;

namespace RoomEscape {
    public class WanderPosition : MonoBehaviour, IStateful, IPoolable {
        private readonly State state = new State();

        public State GetState() {
            return state;
        }

        public int Activate(params object[] activateParams) {
            return 0;
        }

        public void ReturnSelf() {
            gameObject.SetActive(false);
        }
    }
}
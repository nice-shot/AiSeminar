using UnityEngine;
using Ai.Goap;

namespace RoomEscape {
    public class Door : MonoBehaviour, IStateful {
        public GameObject leadsToRoom;
        [SerializeField]
        private bool isOpen;
        [SerializeField]
        private bool isLocked;

        private bool lockChecked;

        void Awake() {
            lockChecked = false;
        }
        
        public State GetState() {
            State state = new State();
            state["open"] = new StateValue(isOpen);
            // Assume unlocked if no one checked it yet
            state["locked"] = new StateValue(lockChecked ? isLocked : false);
            return state;
        }

        public bool IsLocked() {
            lockChecked = true;
            return isLocked;
        }

        public void Unlock() {
            // Maybe should check if it was locked
            isLocked = false;
        }

        public void Open() {
            isOpen = true;
        }

        public void Close() {
            isOpen = false;
        }
    }
}
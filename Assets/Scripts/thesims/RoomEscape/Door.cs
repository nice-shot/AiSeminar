using UnityEngine;
using UnityEngine.AI;
using Ai.Goap;

namespace RoomEscape {
    public class Door : MonoBehaviour, IStateful {
        public GameObject leadsToRoom;
        [SerializeField]
        private bool isOpen;
        [SerializeField]
        private bool isLocked;

        private bool lockChecked;
        private Animator animator;
        private NavMeshObstacle navObstacle;

        // Animation params
        private int openAnim = Animator.StringToHash("Open");

        void Awake() {
            lockChecked = false;
            animator = GetComponent<Animator>();
            navObstacle = GetComponent<NavMeshObstacle>();
            
            UpdateState();
        }

        private void UpdateState() {
            if (animator != null) {
                animator.SetBool(openAnim, isOpen);
            }

            if (navObstacle != null) {
                navObstacle.enabled = !isOpen;
            }
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
            UpdateState();
        }

        public void Close() {
            isOpen = false;
            UpdateState();
        }
    }
}
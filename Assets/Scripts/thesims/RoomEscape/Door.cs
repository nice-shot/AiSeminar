using UnityEngine;
using UnityEngine.AI;
using Ai.Goap;

namespace RoomEscape {
    public class Door : Interactable, IStateful {
        public GameObject leadsToRoom;
        [SerializeField] private bool isOpen;
        [SerializeField] private bool isLocked;
        [SerializeField] private int strength;
        private bool isBroken = false;

        private bool lockChecked;
        private Animator animator;
        private NavMeshObstacle navObstacle;

        // Animation params
        private int openAnim = Animator.StringToHash("Open");
        private int breakAnim = Animator.StringToHash("Break");

        protected override void Awake() {
            base.Awake();
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
            state[States.OPEN] = new StateValue(isOpen);
            // Assume unlocked if no one checked it yet
            state[States.LOCKED] = new StateValue(lockChecked ? isLocked : false);
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

        public bool Break(int power) {
            strength -= power;
            if (strength <= 0) {
                isOpen = true;
                isBroken = true; // Should add state for this since broken doors can't be closed
                animator.SetTrigger(breakAnim);
                UpdateState();
                return true;
            }
            return false;
        }

        public override string GetMainAction() {
            if (!lockChecked || !isLocked) {
                if (isOpen) {
                    return "Close";
                }
                return "Open";
            }

            return "Double Check";
        }

        public override string GetDescription() {
            string description = "Door";
            if (lockChecked && isLocked) {
                description = "Door (Locked)";
            }
            return description;
        }

        public override string Use() {
            if (IsLocked()) {
                return "Locked!";
            }

            if (isOpen) {
                Close();
                return "Closed Door!";
            }

            Open();
            return "Opened Door!";
        }
    }
}
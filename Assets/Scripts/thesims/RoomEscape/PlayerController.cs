using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RoomEscape {
    /// <summary>
    /// Control player using mouse. Point and click approach
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class PlayerController : MonoBehaviour {

        public Interactable currentTarget;
        public SpeachBubbleController speachBubble;

        public static PlayerController instance;

        private NavMeshAgent navAgent;
        private int interactableLayer;

        const float RAYCAST_DISTANCE = 100f;

        void Awake() {
            interactableLayer = LayerMask.NameToLayer("Interactable");
            navAgent = GetComponent<NavMeshAgent>();

            // Set player as instance
            if (instance == null) {
                instance = this;
            } else {
                Destroy(this.gameObject);
            }
        }

        void Update() {
            if (Input.GetMouseButtonDown(0)) {
                // Move to clicked location
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, RAYCAST_DISTANCE)) { 
                    navAgent.SetDestination(hit.point);
                    navAgent.isStopped = false;

                    // Check interactability
                    if (hit.transform.gameObject.layer == interactableLayer) {
                        currentTarget = hit.transform.GetComponent<Interactable>();
                        AnnounceAction();
                    } else {
                        currentTarget = null;
                    }
                }
            }

            // Perform the action
            if (currentTarget != null && GotToTarget()) {
                if (currentTarget.CanUse()) {
                    speachBubble.Say(currentTarget.Use());
                }
                currentTarget = null;
            }
        }

        private bool GotToTarget() {
            if (!navAgent.pathPending
                && navAgent.pathStatus == NavMeshPathStatus.PathComplete
                && Mathf.Approximately(navAgent.velocity.sqrMagnitude, 0f)
                && navAgent.remainingDistance <= navAgent.stoppingDistance) {
                return true;
            }
            return false;
        }

        private void AnnounceAction() {
            if (currentTarget.CanUse()) {
                speachBubble.Say(currentTarget.GetMainAction()
                                 + " "
                                 + currentTarget.GetDescription());
                
            }
        }
    }
}
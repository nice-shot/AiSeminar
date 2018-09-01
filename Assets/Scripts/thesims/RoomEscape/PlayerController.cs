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

        public SpeachBubbleController speachBubble;
        public Container heldItem;

        public static PlayerController instance;

        private Interactable currentTarget;
        private NavMeshAgent navAgent;
        private int interactableLayer;

        const float RAYCAST_DISTANCE = 100f;

        void Awake() {
            interactableLayer = LayerMask.NameToLayer("Interactable");
            navAgent = GetComponent<NavMeshAgent>();
            heldItem = GetComponent<Container>();

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
                        // TODO: use nav target to allow closing doors from
                        // both sides
                        AnnounceAction();
                    } else {
                        currentTarget = null;
                    }
                }
            }

            // Perform the action
            if (currentTarget != null && GotToTarget()) {
                if (currentTarget.CanUse(heldItem)) {
                    speachBubble.Say(currentTarget.Use(heldItem));
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
            if (currentTarget.CanUse(heldItem)) {
                speachBubble.Say(currentTarget.GetMainAction(heldItem)
                                 + " "
                                 + currentTarget.GetDescription());
                
            }
        }
    }
}
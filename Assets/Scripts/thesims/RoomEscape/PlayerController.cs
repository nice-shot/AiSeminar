using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RoomEscape {
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
            if (instance == null) {
                instance = this;
            } else {
                // There can be only one!
                Destroy(this.gameObject);
            }
        }

        void Update() {
            if (Input.GetMouseButtonDown(0)) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, RAYCAST_DISTANCE)) { 
                    navAgent.SetDestination(hit.point);
                    navAgent.isStopped = false;

                    if (hit.transform.gameObject.layer == interactableLayer) {
                        currentTarget = hit.transform.GetComponent<Interactable>();
                        AnnounceAction();
                    } else {
                        currentTarget = null;
                    }
                }
            }

            if (currentTarget != null && GotToTarget()) {
                speachBubble.Say(currentTarget.Use());
                currentTarget = null;
            }
        }

        private bool GotToTarget() {
            if (!navAgent.pathPending 
                && navAgent.pathStatus == NavMeshPathStatus.PathComplete
                && navAgent.remainingDistance <= navAgent.stoppingDistance) {
                return true;
            }
            return false;
        }

        private void AnnounceAction() {
            if (currentTarget.GetMainAction() != "") {
                speachBubble.Say(currentTarget.GetMainAction()
                                 + " "
                                 + currentTarget.GetDescription());
                
            }
        }
    }
}
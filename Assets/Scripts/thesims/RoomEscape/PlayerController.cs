using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RoomEscape {
    [RequireComponent(typeof(NavMeshAgent))]
    public class PlayerController : MonoBehaviour {

        private NavMeshAgent navAgent;

        const float RAYCAST_DISTANCE = 100f;

        void Awake() {
            navAgent = GetComponent<NavMeshAgent>();
        }

        void Update() {
            if (Input.GetMouseButtonDown(0)) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, RAYCAST_DISTANCE)) { 
                    navAgent.SetDestination(hit.point);
                    navAgent.isStopped = false;
                }
            }
        }
    }
}
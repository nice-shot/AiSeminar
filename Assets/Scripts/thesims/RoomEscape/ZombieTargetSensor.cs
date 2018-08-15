using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using Ai.Goap;

namespace RoomEscape {
    [RequireComponent(typeof(NavMeshAgent))]
    public class ZombieTargetSensor : SensorBase {

        public float viewRadius;

        private const string SURVIVOR_TAG = "Survivor";

        private NavMeshAgent navAgent;
        private List<IStateful> targets = new List<IStateful>();
        private readonly State state = new State();

        void Awake() {
            navAgent = GetComponent<NavMeshAgent>();
        }

        public List<IStateful> FindTargets() {
            List<IStateful> seenTargets = new List<IStateful>();
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, viewRadius);

            foreach (Collider collider in hitColliders) {
                if (!collider.CompareTag(SURVIVOR_TAG)) {
                    continue;
                }

                NavMeshPath path = new NavMeshPath();
                navAgent.CalculatePath(collider.transform.position, path);
                if (path.status == NavMeshPathStatus.PathComplete) {
                    seenTargets.Add(collider.GetComponent<IStateful>());
                }
            }

            return seenTargets;
        }

        public override State GetState() {
            return state;
        }

        // Used to display vision radius
        private void OnDrawGizmos() {
            Gizmos.DrawWireSphere(transform.position, viewRadius);
        }
    }
}
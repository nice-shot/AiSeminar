using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using Infra.Collections;
using Ai.Goap;

namespace RoomEscape {
    public class WanderAroundAction : ActionBase {

        public GameObjectPool wanderPool;
        public float wanderingMaxDistance;

        private Dictionary<GoapAgent, WanderPosition> targetPositions = new Dictionary<GoapAgent, WanderPosition>();
        private Dictionary<GoapAgent, Vector3> previousPositions = new Dictionary<GoapAgent, Vector3>();

        //private WanderPosition targetPosition;

        void Awake() {
            AddEffect(States.WANDER, ModificationType.Set, true);    
        }

        public override List<IStateful> GetAllTargets(GoapAgent agent) {
            if (!targetPositions.ContainsKey(agent)) {
                targetPositions[agent] = null;
                previousPositions[agent] = agent.transform.position;
            }

            WanderPosition targetPosition = targetPositions[agent];
            if (targetPosition == null || !targetPosition.gameObject.activeInHierarchy) {
                targetPositions[agent] = wanderPool.Borrow<WanderPosition>();
                targetPosition = targetPositions[agent];
                targetPosition.transform.position = GetRandomPosition(agent.transform.position,
                                                                      wanderingMaxDistance);
                previousPositions[agent] = agent.transform.position;
            } else if (Vector3.Distance(previousPositions[agent], agent.transform.position) > wanderingMaxDistance) {
                // Used in case the agent moved and is now away from the original wander position
                targetPositions[agent].transform.position = GetRandomPosition(agent.transform.position,
                                                                              wanderingMaxDistance);

            }
            
            return new List<IStateful> { targetPosition };
        }

        private Vector3 GetRandomPosition(Vector3 origin, float distance, int layermask = -1) {
            Vector3 randomPosition = Random.insideUnitSphere * wanderingMaxDistance;
            randomPosition += origin;
            NavMeshHit navHit;
            if (NavMesh.SamplePosition(randomPosition, out navHit, distance, NavMesh.AllAreas)) {
                NavMeshPath path = new NavMeshPath();
                if (NavMesh.CalculatePath(origin, navHit.position, NavMesh.AllAreas, path)) {
                    if (path.status == NavMeshPathStatus.PathComplete) {
                        Debug.DrawRay(navHit.position, Vector3.up * 3, Color.blue, 2f);
                        return navHit.position;
                    }
                }
            }

            // Retries if there was a problem with the nav hit
            return GetRandomPosition(origin, distance, layermask);
        }

        protected override bool OnDone(GoapAgent agent, WithContext context) {
            base.OnDone(agent, context);
            targetPositions[agent].ReturnSelf();
            return true;
        }

        public override bool RequiresInRange() {
            return true;
        }
    }
}
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

        private WanderPosition targetPosition;

        void Awake() {
            AddEffect(States.WANDER, ModificationType.Set, true);    
        }

        public override List<IStateful> GetAllTargets(GoapAgent agent) {
            if (targetPosition == null || !targetPosition.gameObject.activeInHierarchy) {
                targetPosition = wanderPool.Borrow<WanderPosition>();
                targetPosition.transform.position = GetRandomPosition(agent.transform.position, wanderingMaxDistance);
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
            // TODO: Write a different gameobject pool or change this one to allow returning one object / just remove the pool.
            targetPosition.ReturnSelf();
            //WanderPosition target = context.target as WanderPosition;
            //target.ReturnSelf();
            //Debug.Log("Active objects in pool: " + wanderPool.HasActiveObjects());
            return true;
        }

        public override bool RequiresInRange() {
            return true;
        }
    }
}
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

        void Awake() {
            AddEffect(States.WANDER, ModificationType.Set, true);    
        }

        public override List<IStateful> GetAllTargets(GoapAgent agent) {
            WanderPosition target = wanderPool.Borrow<WanderPosition>();

            target.transform.position = GetRandomPosition(agent.transform.position, wanderingMaxDistance);
            
            return new List<IStateful> { target };
        }

        private Vector3 GetRandomPosition(Vector3 origin, float distance, int layermask = -1) {
            Vector3 randomPosition = Random.insideUnitSphere * wanderingMaxDistance;
            randomPosition += origin;

            NavMeshHit navHit;
            if (NavMesh.SamplePosition(randomPosition, out navHit, distance, layermask)) {
                return navHit.position;
            }

            // Retries if there was a problem with the nav hit
            return GetRandomPosition(origin, distance, layermask);
        }

        protected override bool OnDone(GoapAgent agent, WithContext context) {
            base.OnDone(agent, context);
            // TODO: Write a different gameobject pool or change this one to allow returning one object / just remove the pool.
            wanderPool.ReturnAll();
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
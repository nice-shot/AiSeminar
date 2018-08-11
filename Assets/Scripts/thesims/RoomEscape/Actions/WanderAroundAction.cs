using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Infra.Collections;
using Ai.Goap;

namespace RoomEscape {
    public class WanderAroundAction : ActionBase {

        public GameObjectPool wanderPool;

        void Awake() {
            AddEffect(States.WANDER, ModificationType.Set, true);    
        }

        public override List<IStateful> GetAllTargets(GoapAgent agent) {
            WanderPosition target = wanderPool.Borrow<WanderPosition>();
            if (target == null) {
                Debug.LogError("Can't get target!");
                return base.GetAllTargets(agent);
            } else {
                Debug.Log("Got target: " + target);
            }
            target.transform.position = new Vector3(Random.Range(1f, 20f), 0f, Random.Range(1f, 8f));
            return new List<IStateful> { target };
        }

        protected override bool OnDone(GoapAgent agent, WithContext context) {
            base.OnDone(agent, context);
            // Maybe write a different pool
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
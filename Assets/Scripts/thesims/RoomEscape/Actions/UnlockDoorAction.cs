using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Ai.Goap;

namespace RoomEscape {
    public class UnlockDoorAction : GoapAction {
        private List<IStateful> targets;

        void Awake() {
            AddTargetEffect("locked", ModificationType.Set, false);
            AddTargetPrecondition("locked", CompareType.Equal, true);
            AddPrecondition("has" + ItemType.Key.ToString(), CompareType.Equal, true);
            AddEffect("has" + ItemType.Key.ToString(), ModificationType.Set, false);
        }

        void Start() {
            targets = GetTargets<Door>();
        }

        public override List<IStateful> GetAllTargets(GoapAgent agent) {
            return targets;
        }

        public override bool RequiresInRange() {
            return true;
        }

        protected override bool OnDone(GoapAgent agent, WithContext context) {
            base.OnDone(agent, context);

            Container agentContainer = agent.GetComponent<Container>();

            if (agentContainer.itemType != ItemType.Key || agentContainer.item == null) {
                // We don't have a key anymore
                return false;
            }

            Door target = context.target as Door;
            target.Unlock();
            // Destroy the key
            agentContainer.itemType = ItemType.None;
            Destroy(agentContainer.item);
            agentContainer.item = null;

            return true;
        }
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Ai.Goap;

namespace RoomEscape {
    public class UnlockDoorAction : ActionBase {
        void Awake() {
            AddTargetEffect(States.LOCKED, ModificationType.Set, false);
            AddTargetPrecondition(States.LOCKED, CompareType.Equal, true);
            AddPrecondition(States.HELD_ITEM, CompareType.Equal, (int)ItemType.Key);
            AddEffect(States.HELD_ITEM, ModificationType.Set, (int)ItemType.None);
        }

        public override List<IStateful> GetAllTargets(GoapAgent agent) {
            return GetTargetsFromMemory<Door>(agent);
        }

        public override bool RequiresInRange() {
            return true;
        }

        protected override bool OnDone(GoapAgent agent, WithContext context) {
            base.OnDone(agent, context);

            Container agentContainer = agent.GetComponent<Container>();

            if (agentContainer.GetItemType() != ItemType.Key) {
                // We don't have a key anymore
                return false;
            }

            Door target = context.target as Door;
            target.Unlock();
            // Destroy the key
            Item key = agentContainer.DropItem();
            key.Hide();

            return true;
        }
    }
}
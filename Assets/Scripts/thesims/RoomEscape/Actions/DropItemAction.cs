using UnityEngine;
using System.Collections;
using Ai.Goap;

namespace RoomEscape {
    public class DropItemAction : ActionBase {

        void Awake() {
            AddEffect("heldItem", Ai.Goap.ModificationType.Set, (int)ItemType.None);
        }

        public override bool RequiresInRange() {
            return false;
        }

        protected override bool OnDone(GoapAgent agent, WithContext context) {
            base.OnDone(agent, context);
            Container container = agent.GetComponent<Container>();
            Memory locationMemory = agent.GetComponent<Memory>();
            container.DropItem(locationMemory.GetCurrentLocation().transform);

            return true;
        }
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Ai.Goap;


namespace RoomEscape {
    public class PickUpItemAction : GoapAction {
        public ItemType itemType;
        private List<IStateful> targets;
        
        void Awake() {
            AddTargetPrecondition("has" + itemType.ToString(), CompareType.Equal, true);
            AddTargetEffect("has" + itemType.ToString(), ModificationType.Set, false);
            AddEffect("has" + itemType.ToString(), ModificationType.Set, true);
        }

        void Start() {
            targets = GetTargets<ItemSpot>();
        }

        public override bool RequiresInRange() {
            return true;
        }

        public override List<IStateful> GetAllTargets(GoapAgent agent) {
            return targets;
        }

        protected override bool OnDone(GoapAgent agent, WithContext context) {
            base.OnDone(agent, context);
            var target = context.target as Component;
            Container itemContainer = target.GetComponent<Container>();
            Container agentContainer = agent.GetComponent<Container>();

            // Someone took the item by the time we got here
            if (itemContainer.itemType != itemType) {
                return false;
            }

            itemContainer.SwitchItems(agentContainer);
            return true;

        }
    }
}
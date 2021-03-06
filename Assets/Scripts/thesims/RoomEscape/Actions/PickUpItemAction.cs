﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Ai.Goap;


namespace RoomEscape {
    public class PickUpItemAction : ActionBase {
        public ItemType itemType;
        
        void Awake() {
            AddTargetPrecondition(States.TYPE, CompareType.Equal, (int)itemType);
            // Can only pick up visible objects
            AddTargetPrecondition(States.VISIBLE, CompareType.Equal, true);
            AddEffect(States.HELD_ITEM, ModificationType.Set, (int)itemType);
            // Need to not hold anything in order to pickup an item
            AddPrecondition(States.HELD_ITEM, CompareType.Equal, (int)ItemType.None);
        }

        public override bool RequiresInRange() {
            return true;
        }

        public override List<IStateful> GetAllTargets(GoapAgent agent) {
            return GetTargetsFromMemory<Item>(agent);
        }

        protected override bool OnDone(GoapAgent agent, WithContext context) {
            base.OnDone(agent, context);
            Component target = context.target as Component;
            Item targetItem = target.GetComponent<Item>();
            Container container = agent.GetComponent<Container>();

            // Someone took the item by the time we got here
            if (targetItem.type != itemType) {
                return false;
            }

            container.PickUpItem(targetItem);
            return true;

        }
    }
}
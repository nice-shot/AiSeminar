﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Ai.Goap;

namespace RoomEscape {
    public class BreakDoorAction : ActionBase {
        void Awake() {
            AddPrecondition(States.HELD_ITEM, CompareType.Equal, (int)ItemType.Axe);
            AddTargetPrecondition(States.OPEN, CompareType.Equal, false);
            // Should I make sure the door is locked before breaking?...
            AddEffect(States.ESCAPE_ROUTE_AVAILABLE, ModificationType.Set, true);
        }

        public override List<IStateful> GetAllTargets(GoapAgent agent) {
            return GetTargetsFromMemory<Door>(agent);
        }

        public override bool RequiresInRange() {
            return true;
        }

        protected override bool OnDone(GoapAgent agent, WithContext context) {
            base.OnDone(agent, context);

            EscapeDude agentDude = agent as EscapeDude;
            Container agentContainer = agent.GetComponent<Container>();
            Axe axe = agentContainer.GetItem() as Axe; // Should probably error check this

            Component target = context.target as Component;
            Door targetDoor = target.GetComponent<Door>();

            bool broke = targetDoor.Break(axe.Hit());
            if (!axe.gameObject.activeSelf) {
                // The axe broke so we'll drop it
                agentContainer.DropItem();
            }
            
            if (broke) {
                agentDude.escapeRoute = true;
            }

            return broke;
        }
    }
}
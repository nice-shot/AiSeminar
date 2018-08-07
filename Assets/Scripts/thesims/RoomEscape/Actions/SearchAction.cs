using UnityEngine;
using System.Collections;
using Ai.Goap;
using System.Collections.Generic;

namespace RoomEscape {
    public class SearchAction : ActionBase {

        void Awake() {
            AddEffect(States.EXPLORED, ModificationType.Set, true);
            AddPrecondition(States.HELD_ITEM, CompareType.Equal, (int)ItemType.None);
            AddTargetPrecondition(States.SEARCHED, CompareType.Equal, false);
            AddTargetEffect(States.SEARCHED, ModificationType.Set, true);
        }

        public override List<IStateful> GetAllTargets(GoapAgent agent) {
            return GetTargetsFromMemory<Searchable>(agent);
        }

        public override bool RequiresInRange() {
            return true;
        }

        protected override bool OnDone(GoapAgent agent, WithContext context) {
            base.OnDone(agent, context);

            Component target = context.target as Component;
            Searchable targetSearchable = target.GetComponent<Searchable>();

            if (targetSearchable.Search() != ItemType.None) {
                Container agentContainer = agent.GetComponent<Container>();
                agentContainer.PickUpItem(targetSearchable.RetrieveItem());
            }

            return true;
        }
    }
}
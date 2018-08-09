using UnityEngine;
using System.Collections;
using Ai.Goap;
using System.Collections.Generic;

namespace RoomEscape {
    public class GoToSafeSpotAction : ActionBase {

        void Awake() {
            AddEffect(States.ESCAPED, ModificationType.Set, true);
            AddTargetPrecondition(States.CLEAR, CompareType.Equal, true);
        }

        public override List<IStateful> GetAllTargets(GoapAgent agent) {
            return GetTargetsFromMemory<SafeSpot>(agent);
        }

        public override bool RequiresInRange() {
            return true;
        }

        protected override bool OnDone(GoapAgent agent, WithContext context) {
            base.OnDone(agent, context);
            SimulationManager.instance.Stop();
            return true;
        }
    }
}
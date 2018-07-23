using System.Collections.Generic;
using Ai.Goap;

namespace RoomEscape {
    public class OpenDoorAction : GoapAction {

        private List<IStateful> targets;

        void Awake() {
            AddTargetPrecondition("open", CompareType.Equal, false);
            AddTargetPrecondition("locked", CompareType.Equal, false);
            AddEffect("escapeRouteAvailable", ModificationType.Set, true);
        }


        void Start() {
            targets = GetTargets<Door>();
        }

        public override bool RequiresInRange() {
            return true;
        }

        public override List<IStateful> GetAllTargets(GoapAgent agent) {
            return targets;
        }

        protected override bool OnDone(GoapAgent agent, WithContext context) {
            base.OnDone(agent, context);

            Door target = context.target as Door;
            if (target.IsLocked()) {
                return false;
            } else {
                target.Open();
            }

            return true;
        }

    }
}
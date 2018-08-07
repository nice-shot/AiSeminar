using System.Collections.Generic;
using Ai.Goap;

namespace RoomEscape {
    public class OpenDoorAction : ActionBase {

        void Awake() {
            AddTargetPrecondition(States.OPEN, CompareType.Equal, false);
            AddTargetPrecondition(States.LOCKED, CompareType.Equal, false);
            AddEffect(States.ESCAPE_ROUTE_AVAILABLE, ModificationType.Set, true);
        }

        public override bool RequiresInRange() {
            return true;
        }

        public override List<IStateful> GetAllTargets(GoapAgent agent) {
            return GetTargetsFromMemory<Door>(agent);
        }

        protected override bool OnDone(GoapAgent agent, WithContext context) {
            base.OnDone(agent, context);

            Door target = context.target as Door;
            if (target.IsLocked()) {
                return false;
            } else {
                target.Open();
                // Add the new room to memory
                Memory memory = agent.GetComponent<Memory>();
                if (memory != null) {
                    memory.AddLocationToMemory(target.leadsToRoom);
                }
            }

            return true;
        }

    }
}
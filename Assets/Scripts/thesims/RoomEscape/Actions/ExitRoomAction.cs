using UnityEngine;
using Ai.Goap;

namespace RoomEscape {
    // This action is used to motivate agents to get out of rooms
    public class ExitRoomAction : ActionBase {
        public override bool RequiresInRange() {
            return false;
        }

        void Awake() {
            AddEffect(States.ESCAPED, ModificationType.Set, true);
            AddPrecondition(States.ESCAPE_ROUTE_AVAILABLE, CompareType.Equal, true);
        }

        protected override bool OnDone(GoapAgent agent, WithContext context) {
            return base.OnDone(agent, context);
        }
    }
}
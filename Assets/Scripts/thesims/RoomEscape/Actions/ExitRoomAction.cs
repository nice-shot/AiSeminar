using UnityEngine;
using Ai.Goap;

namespace RoomEscape {
    public class ExitRoomAction : ActionBase {
        public override bool RequiresInRange() {
            return false;
        }

        void Awake() {
            AddEffect(States.ESCAPED, ModificationType.Set, true);
            AddPrecondition(States.ESCAPE_ROUTE_AVAILABLE, CompareType.Equal, true);
        }

        protected override bool OnDone(GoapAgent agent, WithContext context) {
            Debug.Log("<color=green>Finished!</color>");
            SimulationManager.instance.Stop();
            return base.OnDone(agent, context);
        }
    }
}
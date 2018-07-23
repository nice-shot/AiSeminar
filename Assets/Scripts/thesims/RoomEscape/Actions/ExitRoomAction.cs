using UnityEngine;
using Ai.Goap;

namespace RoomEscape {
    public class ExitRoomAction : GoapAction {
        public override bool RequiresInRange() {
            return false;
        }

        void Awake() {
            AddEffect("escaped", ModificationType.Set, true);
            AddPrecondition("escapeRouteAvailable", CompareType.Equal, true);
        }

        protected override bool OnDone(GoapAgent agent, WithContext context) {
            Debug.Log("<color=green>Finished!</color>");
            return base.OnDone(agent, context);
        }
    }
}
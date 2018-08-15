using UnityEngine;
using System.Collections;

namespace RoomEscape {
    public class LookAroundAction : ActionBase {
        public override bool RequiresInRange() {
            return false;
        }
    }
}
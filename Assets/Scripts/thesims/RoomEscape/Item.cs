using UnityEngine;
using System.Collections;

using Ai.Goap;

namespace RoomEscape {
    public enum ItemType {
        None,
        Key,
        Axe
    }

    public class Item : MonoBehaviour, IStateful {
        public bool visible;
        public ItemType type;

        private readonly State state = new State();

        public State GetState() {
            state["visible"] = new StateValue(visible);
            state["type"] = new StateValue((int)type);
            return state;
        }
    }
}
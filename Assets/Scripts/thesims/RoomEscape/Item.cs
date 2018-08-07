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
        [SerializeField] private Transform graphic;

        private readonly State state = new State();

        void Awake() {
            if (graphic == null) {
                graphic = transform.Find("Graphic");
            }
        }

        public State GetState() {
            state["visible"] = new StateValue(visible);
            state["type"] = new StateValue((int)type);
            return state;
        }

        public void Hide() {
            visible = false;
            gameObject.SetActive(false);
        }
    }
}
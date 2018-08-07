using UnityEngine;
using System.Collections;

using Infra.Utils;
using Ai.Goap;

namespace RoomEscape {
    [RequireComponent(typeof(Container))]
    public class ItemSpot : MonoBehaviour, IStateful {

        private Container container;
        private readonly State state = new State();

        void Awake() {
            container = GetComponent<Container>();
        }

        public State GetState() {
            // Set state as "has<Item>" with true or false for each item
            foreach (var itemType in EnumUtils.EnumValues<ItemType>()) {
                if (itemType == ItemType.None) continue;
                state["has" + itemType.ToString()] = new StateValue(itemType == container.GetItemType());
            }
            return state;
        }
    }
}
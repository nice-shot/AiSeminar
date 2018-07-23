using UnityEngine;

namespace RoomEscape {
    public enum ItemType {
        None,
        Key,
        Axe
    }

    // Simple container for a single item
    public class Container : MonoBehaviour {
        public GameObject item;
        public ItemType itemType;

        private void Awake() {
            if (item == null) {
                itemType = ItemType.None;
            }
        }

        public void SwitchItems(Container other) {
            GameObject theirItem = other.item;
            ItemType theirType = other.itemType;

            if (item != null) {
                item.transform.SetParent(other.transform);
            }
            other.item = item;
            other.itemType = itemType;

            if (theirItem != null) {
                theirItem.transform.SetParent(other.transform);
            }
            item = theirItem;
            itemType = theirType;
        }
    }
}
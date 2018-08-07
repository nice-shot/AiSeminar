using UnityEngine;

namespace RoomEscape {
    // Simple container for a single item
    public class Container : MonoBehaviour {
        [SerializeField] private Item item;
        [SerializeField] private Transform itemPlacement;

        private void Awake() {
            if (itemPlacement == null) {
                itemPlacement = this.transform;
            }
        }

        public void DropItem(Transform newPlacement = null) {
            if (item == null) {
                return;
            }

            item.transform.SetParent(newPlacement);
            item = null;
        }

        public void PickUpItem(Item newItem) {
            DropItem();
            newItem.transform.SetParent(itemPlacement);
            item = newItem;
        }

        public ItemType GetItemType() {
            if (item == null) {
                return ItemType.None;
            }
            return item.type;
        }
    }
}
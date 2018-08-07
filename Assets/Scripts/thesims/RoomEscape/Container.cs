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

        public Item DropItem(Transform newPlacement = null) {
            if (item == null) {
                return null;
            }

            Item removedItem = item;
            item.transform.SetParent(newPlacement);
            item = null;
            return removedItem;
        }

        public void PickUpItem(Item newItem) {
            DropItem();
            newItem.transform.SetParent(itemPlacement);
            newItem.transform.localPosition = Vector3.zero;
            newItem.transform.localRotation = Quaternion.identity;
            item = newItem;
        }

        public ItemType GetItemType() {
            if (item == null) {
                return ItemType.None;
            }
            return item.type;
        }

        public Item GetItem() {
            return item;
        }
    }
}
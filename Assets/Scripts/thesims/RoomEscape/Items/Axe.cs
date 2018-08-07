using UnityEngine;
using System.Collections;

namespace RoomEscape {
    public class Axe : Item {
        [SerializeField] private int power;
        [SerializeField] private int durability;

        void Awake() {
            type = ItemType.Axe;
        }

        // Returns the hit power but decreases the durability
        public int Hit() {
            durability--;
            if (durability <= 0) {
                // Breaks
                visible = false;
                gameObject.SetActive(false);
            }
            return power;
        }
    }
}
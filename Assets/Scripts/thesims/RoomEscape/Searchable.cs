using UnityEngine;
using System.Collections;

using Ai.Goap;

namespace RoomEscape {
    [RequireComponent(typeof(Container))]
    public class Searchable : Interactable, IStateful {
        // All searchables are not searched by default
        private bool searched = false;

        private Container container;
        private readonly State state = new State();

        protected override void Awake() {
            base.Awake();
            container = GetComponent<Container>();
        }

        public State GetState() {
            state[States.SEARCHED] = new StateValue(searched);
            return state;
        }

        public Container GetContainer() {
            return container;
        }

        public ItemType Search() {
            searched = true;
            return container.GetItemType();
        }

        public Item RetrieveItem() {
            Item item = container.GetItem();
            if (item != null) {
                item.visible = true;
            }
            return container.DropItem();
        }

        public override string GetDescription() {
            if (searched) {
                return name + " (Searched)";
            }
            return name;
        }

        public override string GetMainAction() {
            if (searched) {
                return null;
            }
            return "Search";
        }

        public override bool CanUse() {
            return !searched;
        }

        public override string Use(Container agentContainer) {
            ItemType item = Search();
            if (item == ItemType.None) {
                return "It's empty";
            }

            // Make agent pick up item:
            agentContainer.PickUpItem(RetrieveItem());

            return "Found " + item.ToString() + "!";
        }
    }
}
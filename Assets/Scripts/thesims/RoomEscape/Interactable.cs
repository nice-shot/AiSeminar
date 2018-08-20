using UnityEngine;
using System.Collections;


namespace RoomEscape {
    public abstract class Interactable : MonoBehaviour {

        [SerializeField] private Color highlightColor;
        [SerializeField] private MeshRenderer meshRenderer;

        private Color[] originalColors;

        protected virtual void Awake() {
            if (meshRenderer == null) {
                meshRenderer = GetComponent<MeshRenderer>();
            }
            StoreOriginalColor();
        }

        private void StoreOriginalColor() {
            if (meshRenderer != null) {
                originalColors = new Color[meshRenderer.materials.Length];
                for (int i = 0; i < originalColors.Length; i++) {
                    originalColors[i] = meshRenderer.materials[i].color;
                }
            }
        }

        private void OnMouseEnter() {
            if (meshRenderer != null) {
                foreach (Material material in meshRenderer.materials) {
                    material.color = highlightColor;
                }
            }
        }

        private void OnMouseExit() {
            if (meshRenderer != null) {
                for(int i = 0; i < originalColors.Length; i++) {
                    meshRenderer.materials[i].color = originalColors[i];
                }
            }
        }

        public virtual void Use(Item heldItem) {
            return;
        }
    }
}
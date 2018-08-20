using UnityEngine;
using System.Collections;


namespace RoomEscape {
    public class Interactable : MonoBehaviour {

        [SerializeField] private Color highlightColor;
        [SerializeField] private MeshRenderer meshRenderer;

        private Color[] originalColors;

        private void Awake() {
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
                /*
                Material[] materials = meshRenderer.materials;
                for (int i=0; i < materials.Length; i++) {
                    materials[i].color = originalColors[i];
                }
                meshRenderer.materials = materials;
                */
                for(int i = 0; i < originalColors.Length; i++) {
                    meshRenderer.materials[i].color = originalColors[i];
                }
            }
        }

    }
}
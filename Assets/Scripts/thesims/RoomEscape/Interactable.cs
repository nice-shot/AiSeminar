using UnityEngine;
using System.Collections;


namespace RoomEscape {
    public abstract class Interactable : MonoBehaviour {

        public Color highlightColor;
        public MeshRenderer meshRenderer;

        private Color[] originalColors;
        private InteractionPanelController interactionPanel;

        protected virtual void Awake() {
            if (meshRenderer == null) {
                meshRenderer = GetComponent<MeshRenderer>();
            }
            StoreOriginalColor();
        }

        private void Start() {
            interactionPanel = InteractionPanelController.instance;
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
            interactionPanel.SetText(GetDescription(),
                                     GetMainAction(),
                                     GetSecondaryAction());
            interactionPanel.SetHidden(false);
        }

        private void OnMouseExit() {
            if (meshRenderer != null) {
                for(int i = 0; i < originalColors.Length; i++) {
                    meshRenderer.materials[i].color = originalColors[i];
                }
            }
            interactionPanel.SetHidden(true);
        }

        public virtual string Use() {
            return "";
        }

        public virtual string GetDescription() {
            return "Interactable";
        }

        public virtual string GetMainAction() {
            return null;
        }

        public virtual string GetSecondaryAction() {
            return null;
        }
    }
}
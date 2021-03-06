﻿using UnityEngine;
using System.Collections;


namespace RoomEscape {
    public abstract class Interactable : MonoBehaviour {

        public Color highlightColor;
        public MeshRenderer meshRenderer;

        private Color[] originalColors;
        private InteractionPanelController interactionPanel;
        private Container playerContainer;

        protected virtual void Awake() {
            if (meshRenderer == null) {
                meshRenderer = GetComponent<MeshRenderer>();
            }
            StoreOriginalColor();
        }

        private void Start() {
            interactionPanel = InteractionPanelController.instance;
            playerContainer = PlayerController.instance.GetComponent<Container>();
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
                                     GetMainAction(playerContainer),
                                     GetSecondaryAction());
        }

        private void OnMouseExit() {
            if (meshRenderer != null) {
                for(int i = 0; i < originalColors.Length; i++) {
                    meshRenderer.materials[i].color = originalColors[i];
                }
            }
            interactionPanel.SetHidden(true);
        }
        
        public virtual string Use(Container agentContainer) {
            return "";
        }
        
        public virtual bool CanUse(Container agentContainer) {
            return true;
        }

        // Used for speach bubbles and interaction panel

        public virtual string GetDescription() {
            return "Interactable";
        }

        public virtual string GetMainAction(Container agentContainer) {
            return null;
        }

        public virtual string GetSecondaryAction() {
            return null;
        }
    }
}
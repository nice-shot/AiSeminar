using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InteractionPanelController : MonoBehaviour {
    public static InteractionPanelController instance;

    public Text descriptionText;
    public Text mainActionText;
    public Text secondaryActionText;

    public GameObject mainActionContainer;
    public GameObject secondaryActionContainer;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
        SetHidden(true);
    }

    private void Update() {
        transform.position = Input.mousePosition;
    }

    public void SetText(string description, string main, string secondary) {
        descriptionText.text = description;

        if (main != null) {
            mainActionContainer.SetActive(true);
            mainActionText.text = main;
        } else {
            mainActionContainer.SetActive(false);
        }

        if (secondary != null) {
            secondaryActionContainer.SetActive(true);
            secondaryActionText.text = secondary;
        } else {
            secondaryActionContainer.SetActive(false);
        }
    }

    public void SetHidden(bool hidden) {
        gameObject.SetActive(!hidden);
    }
}

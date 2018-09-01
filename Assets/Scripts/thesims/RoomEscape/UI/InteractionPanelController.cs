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

    private RectTransform rectTransform;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
        rectTransform = GetComponent<RectTransform>();
        SetHidden(true);
    }

    private void Update() {
        transform.position = Input.mousePosition;
        CheckBounds();
    }

    private void CheckBounds() {
        Vector3 positionChange = new Vector3();

        int maxHeight = Screen.height;
        float bottom = transform.position.y;
        float top = bottom + rectTransform.rect.height;

        if (top > maxHeight) {
            positionChange.y = maxHeight - top;
        } else if (bottom < 0) {
            positionChange.y = 0 - bottom;
        }

        int maxWidth = Screen.width;
        float left = transform.position.x;
        float right = left + rectTransform.rect.width;

        if (right > maxWidth) {
            positionChange.x = maxWidth - right;
        } else if (left < 0) {
            positionChange.x = 0 - left;
        }

        transform.position += positionChange;

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
        transform.position = Input.mousePosition;
        gameObject.SetActive(!hidden);
    }
}

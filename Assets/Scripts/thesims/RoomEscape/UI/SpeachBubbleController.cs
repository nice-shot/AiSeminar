using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SpeachBubbleController : MonoBehaviour {
    public Text mainText;
    public float textDuration;
    public Transform target;
    [Tooltip("Move the bubble to better see the character")]
    public Vector2 positionDelta;

    private float startTime;

    private void Awake() {
        gameObject.SetActive(false);
    }

    private void Update() {
        UpdatePosition();

        if (Time.time - startTime >= textDuration) {
            gameObject.SetActive(false);
        }
    }

    private void UpdatePosition() {
        transform.position = Camera.main.WorldToScreenPoint(target.position);
        transform.position += (Vector3)positionDelta;
    }

    public void AttachTarget(Transform target) {
        this.target = target;
    }

    public void Say(string text) {
        startTime = Time.time;
        mainText.text = text;
        UpdatePosition();
        gameObject.SetActive(true);
    }
}

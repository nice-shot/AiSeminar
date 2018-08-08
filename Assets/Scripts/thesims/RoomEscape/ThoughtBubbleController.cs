using UnityEngine;
using System.Collections;

using UnityEngine.UI;

public class ThoughtBubbleController : MonoBehaviour {
    public GameObject agent;
    public float yChange;
    public float xChange;

    void Update() {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(agent.transform.position);
        screenPos.x += xChange;
        screenPos.y += yChange;
        transform.position = screenPos;
    }
}

using UnityEngine;

namespace Infra.Gameplay {
public class TimeManager : MonoBehaviour {
    protected void Update() {
        if (Input.GetKeyUp(KeyCode.Minus) || Input.GetKeyUp(KeyCode.KeypadMinus)) {
            if (Time.timeScale > 1f) {
                Time.timeScale *= 0.5f;
                if (Time.timeScale < 1f) {
                    Time.timeScale = 1f;
                }
            } else if (Time.timeScale > 0.2f) {
                Time.timeScale *= 0.4f;
            } else {
                Time.timeScale = 0.1f;
            }
            DebugUtils.LogError("Time Scale: " + Time.timeScale);
        }
        if (Input.GetKeyUp(KeyCode.Plus) || Input.GetKeyUp(KeyCode.KeypadPlus)
            || Input.GetKeyUp(KeyCode.Equals) || Input.GetKeyUp(KeyCode.KeypadEquals)) {
            if (Time.timeScale < 1f) {
                Time.timeScale *= 2.5f;
                if (Time.timeScale > 1f) {
                    Time.timeScale = 1f;
                }
            } else {
                Time.timeScale += 1f;
            }
            DebugUtils.LogError("Time Scale: " + Time.timeScale);
        }
    }
}
}

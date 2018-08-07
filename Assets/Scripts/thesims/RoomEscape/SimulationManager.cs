using UnityEngine;
using System.Collections;
using UnityEngine.UI;


namespace RoomEscape {
    public class SimulationManager : MonoBehaviour {

        public static SimulationManager instance = null;

        public GameObject doneScreen;

        void Start() {
            if (instance == null) {
                instance = this;
            } else {
                // There can be only one instance
                Destroy(gameObject);
            }
        }

        public void Stop() {
            doneScreen.SetActive(true);
            Time.timeScale = 0f;
        }

        public void Continue() {
            doneScreen.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}
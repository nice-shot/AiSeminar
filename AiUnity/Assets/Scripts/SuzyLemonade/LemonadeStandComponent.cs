using UnityEngine;
using System.Collections;

public class LemonadeStandComponent : MonoBehaviour {
    public int numJars = 0;
    public int maxJars;
    public GameObject[] jarsArray;

    public void Awake() {
        // Assumes we're starting empty
        numJars = 0;
        maxJars = jarsArray.Length;
        foreach (GameObject jar in jarsArray) {
            jar.SetActive(false);
        }
    }

    public void AddLemonade() {
        if (numJars >= maxJars) {
            Debug.Log("Trying to add over max number of lemonade jars");
            return;
        }

        GameObject jar = jarsArray[numJars];
        jar.SetActive(true);
        numJars++;
    }
}


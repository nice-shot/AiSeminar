using UnityEngine;
using System.Collections;

public class LemonadeStandComponent : MonoBehaviour {
    public int numJars = 0;
    public int maxJars;
    public Transform[] lemonadeJarPositions;
    public GameObject jarPrefab;


    public void Awake() {
        // Assumes we're starting empty
        numJars = 0;
        maxJars = lemonadeJarPositions.Length;
    }

    public void AddLemonade() {
        if (numJars >= maxJars) {
            Debug.Log("Trying to add over max number of lemonade jars");
            return;
        }

        Transform jarPosition = lemonadeJarPositions[numJars];
        Instantiate(jarPrefab, jarPosition.position, jarPosition.rotation, this.transform);
        numJars++;
    }
}


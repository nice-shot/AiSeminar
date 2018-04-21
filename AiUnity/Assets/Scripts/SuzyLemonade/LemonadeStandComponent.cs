using UnityEngine;
using System.Collections;

public class LemonadeStandComponent : MonoBehaviour {
    public int numJars = 0;
    public int maxJars;
    public GameObject[] jarsArray;

    private bool isSuzySelling;

    public void Awake() {
        // Assumes we're starting empty
        numJars = 0;
        maxJars = jarsArray.Length;
        isSuzySelling = false;
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

    public void SetSelling(bool isSelling) {
        this.isSuzySelling = isSelling;
    }

    public bool GetSelling() {
        return isSuzySelling;
    }
}


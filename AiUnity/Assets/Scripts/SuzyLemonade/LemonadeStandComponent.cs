using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SuzyLemonade {
public class LemonadeStandComponent : MonoBehaviour {
    public int numJars = 0;
    public int maxJars;
    public GameObject[] jarsArray;

    private bool isSuzySelling;
    private Queue<StandInLineAction> lineForStand = new Queue<StandInLineAction>();

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

    public int GetLineSize() {
        return lineForStand.Count;
    }

    public void AddToLine(StandInLineAction customer) {
        Debug.Log ("Added to line: " + customer);
        lineForStand.Enqueue (customer);
    }

    public void RemoveFirstInLine() {
        StandInLineAction first = lineForStand.Dequeue ();
        Debug.Log ("Removed first in line. Line size is now: " + GetLineSize ());
        first.isWaitingInLine = false;
        // Remove jar
        numJars--;
        GameObject jar = jarsArray [numJars];
        jar.SetActive (false);
    }
}
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LemonTreeComponent : MonoBehaviour {
    public GameObject[] lemonsArray;
    private Queue<GameObject> lemonQueue = new Queue<GameObject> (); 

    public int lemons {
        get {
            return lemonQueue.Count;
        }
    }

    void Awake() {
        // Place lemon for each available position in tree
        foreach (GameObject lemon in lemonsArray) {
            if (lemon.activeSelf) {
                lemonQueue.Enqueue(lemon);
            }
        }
    }

    public GameObject PickLemon() {
        GameObject lemon = lemonQueue.Dequeue();
        return lemon;
    }


}


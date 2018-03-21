using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LemonTreeComponent : MonoBehaviour {
    public Transform[] lemonSpawnPositions;
    public GameObject lemonPrefab;
    private Queue<GameObject> lemonQueue = new Queue<GameObject> (); 

    public int lemons {
        get {
            return lemonQueue.Count;
        }
    }

    void Awake() {
        // Place lemon for each available position in tree
        foreach (Transform lemonPos in lemonSpawnPositions) {
            // TODO: Use object pooling instead of instantiating here
            GameObject lemon = Instantiate(lemonPrefab, lemonPos.position, lemonPos.rotation, this.transform);
            lemonQueue.Enqueue(lemon);
        }
    }

    public GameObject PickLemon() {
        GameObject lemon = lemonQueue.Dequeue();
        return lemon;
    }


}


using UnityEngine;
using System.Collections;

public class LemonadeStandComponent : MonoBehaviour {
    public const int MAX_LEMONADE_JARS = 10;

    private int lemonadeJars = 0;

    // Tries to add a new lemonade jar. Returns whether succeeded
    public bool AddLemonadeJar() {
        lemonadeJars++;
        if (lemonadeJars > MAX_LEMONADE_JARS) {
            Debug.Log("Reached max lemonade jars!");
            lemonadeJars = MAX_LEMONADE_JARS;
            return false;
        }
        return true;
    }



}


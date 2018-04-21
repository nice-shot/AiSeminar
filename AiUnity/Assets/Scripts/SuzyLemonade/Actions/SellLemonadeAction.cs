using UnityEngine;
using System;
using System.Collections;

namespace SuzyLemonade {
public class SellLemonadeAction : SmartAction {
    public float maxSellWaitTime;
    private LemonadeStandComponent targetStand;
    private float sellStartTime;

    void Awake() {
        AddEffect ("sellLemonade", true);
        AddPrecondition ("hasLemonade", true);
    }

    protected override void SmartReset () {
        targetStand = null;
        sellStartTime = 0f;
    }

    protected override bool SmartPerform (GameObject agent) {
        if (sellStartTime == 0) {
            sellStartTime = Time.time;
        }

        // Check if someone is waiting in the line and if we have enough lemonade
        if (targetStand.numJars > 0 && targetStand.GetLineSize () > 0) {
            // Sell lemonade to this person
            targetStand.RemoveFirstInLine ();
            isDone = true;
            return true;
        }

        if (Time.time - sellStartTime > maxSellWaitTime) {
            // No one came to buy
            return false;
        }
        return true;
    }

    public override bool RequiresInRange () {
        return true;
    }

    public override bool CheckProceduralPrecondition(GameObject agent) {
        GameObject[] stands = FindGameObjectsOfType<LemonadeStandComponent> ();
        target = FindClosest (agent, stands);

        if (target == null) {
            return false;
        }

        targetStand = target.GetComponent<LemonadeStandComponent> ();

        return targetStand != null;
    }
}
}
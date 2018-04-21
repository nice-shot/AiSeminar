using UnityEngine;
using System;
using System.Collections;

namespace SuzyLemonade {
public class SellLemonadeAction : SmartAction {
    private LemonadeStandComponent targetStand;

    void Awake() {
        AddEffect ("sellLemonade", true);
        AddPrecondition ("makeLemonade", true);
    }

    protected override void SmartReset () {
        targetStand = null;
    }

    protected override bool SmartPerform (GameObject agent) {
        if (targetStand.numJars > 0) {
            // Check if someone is waiting in the line
            // Sell lemonade to this person
            return true;
        }
        return false;
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
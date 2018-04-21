using UnityEngine;
using System.Collections;

namespace SuzyLemonade {
public class StandInLineAction : SmartAction {
    private LemonadeStandComponent targetStand;

    void Awake() {
        AddEffect ("inFrontOfLine", true);
        AddPrecondition ("hasMoney", true);
    }

    protected override void SmartReset () {
        targetStand = null;
    }

    protected override bool SmartPerform (GameObject agent) {
        // Stand in line and wait for suzy to start selling + wait to be first in line
        if (targetStand.GetSelling ()) {
            isDone = true;
        }
        return true;
        // Allow max wait time before giving up
    }

    public override bool RequiresInRange () {
        return true;
    }

    public override bool CheckProceduralPrecondition (GameObject agent) {
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
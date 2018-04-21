using UnityEngine;
using System.Collections;

namespace SuzyLemonade {
public class StandInLineAction : SmartAction {
    public bool isWaitingInLine;
    public float maxLineWaitTime;
    private float waitInLineStart = 0;

    private LemonadeStandComponent targetStand;

    void Awake() {
        AddEffect ("inFrontOfLine", true);
        AddPrecondition ("hasMoney", true);
    }

    protected override void SmartReset () {
        targetStand = null;
        isWaitingInLine = false;
        waitInLineStart = 0;
    }

    protected override bool SmartPerform (GameObject agent) {
        if (waitInLineStart == 0) {
            targetStand.AddToLine (this);
            waitInLineStart = Time.time;
            isWaitingInLine = true;
        }

        // Max wait time before giving up
        if (Time.time - waitInLineStart > maxLineWaitTime) {
            // This should make him stop the action
            return false;
        }

        // Stand in line and wait to be first in line
        if (isWaitingInLine == false) {
            isDone = true;
        }
        return true;
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
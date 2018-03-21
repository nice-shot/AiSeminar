using System.Collections;
using UnityEngine;

using AI.Goap;

namespace SuzyLemonade {
public class MakeLemonadeAction : GoapAction {
    private bool madeLemonade = false;
    private LemonadeStandComponent targetStand;

    private float startTime = 0f;
    public float workDuration = 2f;
    // Seconds

    void Awake() {
        AddEffect("makeLemonade", true);
        AddPrecondition("hasLemons", true);
    }

    public override void Reset() {
        madeLemonade = false;
        targetStand = null;
        startTime = 0;
    }

    public override bool IsDone() {
        return madeLemonade;
    }

    public override bool RequiresInRange() {
        return true;
    }

    public override bool CheckProceduralPrecondition(GameObject agent) {
        LemonadeStandComponent[] stands = (LemonadeStandComponent[])GameObject.FindObjectsOfType(LemonadeStandComponent);
        LemonadeStandComponent closest;
        float closestDist = 0;

        foreach (LemonadeStandComponent stand in stands) {
            if (closest == null) {
                // first one so choose it for now
                closest = stand;
                closestDist = (stand.transform.position - agent.transform.position).sqrMagnitude;
            } else {
                // is this one closer than the last?
                float dist = (stand.transform.position - agent.transform.position).sqrMagnitude;
                if (dist < closestDist) {
                    // found a closer one
                    closest = stand;
                    closestDist = dist;
                }
            }
        }
        if (closest = null) {
            return false;
        }

        targetStand = closest;
        target = targetStand.gameObject;

        return closest != null;
    }

    public override bool Perform(GameObject agent) {
        if (startTime == 0) {
            startTime = Time.time;
        }

        if (Time.time - startTime > workDuration) {
            // finished making lemonade
            madeLemonade = true;
            // What happens when we can't add one?
            targetStand.AddLemonadeJar();
        }
        return true;
    }
}
}
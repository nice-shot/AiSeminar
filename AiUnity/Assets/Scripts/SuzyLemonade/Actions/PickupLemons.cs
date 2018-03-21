using UnityEngine;
using System.Collections;

using AI.Goap;
namespace SuzyLemonade {
public class PickupLemons : GoapAction {
    private bool gotLemons = false;
    private LemonTreeComponent targetTree;

    private float startTime = 0f;
    public float workDuration = 2f;
    // Seconds

    void Awake() {
        AddEffect("hasLemons", true);
    }

    public override void Reset() {
        gotLemons = false;
        targetTree = null;
        startTime = 0;
    }

    public override bool IsDone() {
        return gotLemons;
    }

    public override bool RequiresInRange() {
        return true;
    }

    public override bool CheckProceduralPrecondition(GameObject agent) {
        LemonTreeComponent[] trees = (LemonTreeComponent[])GameObject.FindObjectsOfType(typeof(LemonTreeComponent));
        LemonTreeComponent closest = null;
        float closestDist = 0;

        foreach (LemonTreeComponent tree in trees) {
            if (tree.lemons <= 0) {
                continue;
            }
            if (closest == null) {
                // first one so choose it for now
                closest = tree;
                closestDist = (tree.transform.position - agent.transform.position).sqrMagnitude;
            } else {
                // is this one closer than the last?
                float dist = (tree.transform.position - agent.transform.position).sqrMagnitude;
                if (dist < closestDist) {
                    // found a closer one
                    closest = tree;
                    closestDist = dist;
                }
            }
        }
        if (closest == null) {
            return false;
        }

        targetTree = closest;
        target = targetTree.gameObject;

        return closest != null;
    }

    public override bool Perform(GameObject agent) {
        if (startTime == 0) {
            startTime = Time.time;
        }

        if (Time.time - startTime > workDuration) {
            // finished making lemonade
            gotLemons = true;
            GameObject lemon = targetTree.PickLemon();
            Person agentPerson = agent.GetComponent<Person>();
            agentPerson.HoldItem(lemon);
        }
        return true;
    }
}
}
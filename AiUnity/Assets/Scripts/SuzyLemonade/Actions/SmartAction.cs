using UnityEngine;
using System.Collections;

using AI.Goap;

namespace SuzyLemonade {
public abstract class SmartAction : GoapAction {

    private float startTime = 0f;
    private bool isSuccessfull;
    // Subclasses should change this when the task is finished
    protected bool isDone;

    public float workDuration;

    public override void Reset () {
        startTime = 0f;
        isSuccessfull = true;
        isDone = false;
        SmartReset ();
    }

    public override bool IsDone () {
        return isDone;
    }

    public override bool Perform(GameObject agent) {
        if (startTime == 0) {
            startTime = Time.time;
        }

        if (Time.time - startTime > workDuration) {
            isSuccessfull = SmartPerform (agent);
        }
        return isSuccessfull;
    }

    public GameObject FindClosest (GameObject agent, GameObject[] possibleTargets) {
        GameObject closest = null;
        float closestDist = 0;

        foreach (GameObject possibleTarget in possibleTargets) {
            if (closest == null) {
                // first one so choose it for now
                closest = possibleTarget;
                closestDist = (possibleTarget.transform.position - agent.transform.position).sqrMagnitude;
            } else {
                // is this one closer than the last?
                float dist = (possibleTarget.transform.position - agent.transform.position).sqrMagnitude;
                if (dist < closestDist) {
                    // found a closer one
                    closest = target;
                    closestDist = dist;
                }
            }
        }

        return closest;
    }

    // Reset after things related to smart reset are complete
    protected abstract void SmartReset ();
    // Perform after time calculation happend. Return false if a problem occured
    protected abstract bool SmartPerform (GameObject agent); 
}
}
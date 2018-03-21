using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI.Goap;

namespace SuzyLemonade {
public abstract class Person : MonoBehaviour, IGoap {
    public float moveSpeed = 1;

    public HashSet<KeyValuePair<string, object>> GetWorldState() {
        HashSet<KeyValuePair<string, object>> worldData = new HashSet<KeyValuePair<string, object>>();

        // implement world data
        return worldData;
    }

    public abstract HashSet<KeyValuePair<string,object>> CreateGoalState();

    public void PlanFailed(HashSet<KeyValuePair<string, object>> failedGoal) {
        // Not handling this here since we are making sure our goals will always succeed.
        // But normally you want to make sure the world state has changed before running
        // the same goal again, or else it will just fail.
    }

    public void PlanFound(HashSet<KeyValuePair<string, object>> goal, Queue<GoapAction> actions) {
        // Yay we found a plan for our goal
        Debug.Log("<color=green>Plan found</color> " + GoapAgent.PrettyPrint(actions));
    }

    public void ActionsFinished() {
        // Everything is done, we completed our actions for this gool. Hooray!
        Debug.Log("<color=blue>Actions completed</color>");
    }

    public void PlanAborted(GoapAction aborter) {
        // An action bailed out of the plan. State has been reset to plan again.
        // Take note of what happened and make sure if you run the same goal again
        // that it can succeed.
        Debug.Log("<color=red>Plan Aborted</color> " + GoapAgent.PrettyPrint(aborter));
    }

    public bool MoveAgent(GoapAction nextAction) {
        // move towards the NextAction's target
        float step = moveSpeed * Time.deltaTime;
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, nextAction.target.transform.position, step);

        if (gameObject.transform.position.Equals(nextAction.target.transform.position)) {
            // we are at the target location, we are done
            nextAction.SetInRange(true);
            return true;
        }

        return false;
    }
}
}
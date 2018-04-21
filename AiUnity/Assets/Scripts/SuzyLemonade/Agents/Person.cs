using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using AI.Goap;

namespace SuzyLemonade {
public abstract class Person : MonoBehaviour, IGoap {
    public GameObject heldItem;
    public NavMeshAgent navAgent;
    public GameObject itemHolder;

    void Awake() {
        if (!itemHolder) {
            itemHolder = this.gameObject;
        }
    }

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
//        float step = moveSpeed * Time.deltaTime;
//        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, nextAction.target.transform.position, step);
        navAgent.SetDestination(nextAction.target.transform.position);

        if (!navAgent.pathPending) {
            if (navAgent.remainingDistance <= navAgent.stoppingDistance) {
                if (!navAgent.hasPath || navAgent.velocity.sqrMagnitude == 0f) {
                    nextAction.SetInRange(true);
                    return true;
                }
            }
        }

        return false;
    }

    public void HoldItem(GameObject item) {
        if (heldItem != null) {
            DropItem();
        }

        item.transform.parent = itemHolder.transform;
        item.transform.localPosition = Vector3.zero;
        heldItem = item;
    }

    public GameObject DropItem() {
        GameObject item = heldItem;
        heldItem.transform.parent = null;
        heldItem = null;
        return item;
    }
}
}
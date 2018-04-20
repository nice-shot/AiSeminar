using UnityEngine;
using System;
using System.Collections;

using AI.Goap;
namespace SuzyLemonade {
public class PickupLemons : SmartAction {
    private LemonTreeComponent targetTree;

    void Awake() {
        AddEffect("hasLemons", true);
    }

    protected override void SmartReset() {
        targetTree = null;
    }

    public override bool RequiresInRange() {
        return true;
    }

    public override bool CheckProceduralPrecondition(GameObject agent) {
        GameObject[] treeGameObjects = FindGameObjectsOfType<LemonTreeComponent>();

        treeGameObjects = Array.FindAll<GameObject> (treeGameObjects, t => t.GetComponent<LemonTreeComponent>().lemons > 0);
        target = FindClosest (agent, treeGameObjects);

        if (target == null) {
            return false;
        }

        targetTree = target.GetComponent<LemonTreeComponent> ();

        return targetTree != null;
    }

    protected override bool SmartPerform(GameObject agent) {
        // finished making lemonade
        isDone = true;
        GameObject lemon = targetTree.PickLemon();
        Person agentPerson = agent.GetComponent<Person>();
        agentPerson.HoldItem(lemon);

        return true;
    }
}
}
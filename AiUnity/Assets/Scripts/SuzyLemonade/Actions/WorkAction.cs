using UnityEngine;
using System.Collections;

namespace SuzyLemonade {
public class WorkAction : SmartAction {
    void Awake() {
        AddEffect ("hasMoney", true);
    }

    protected override void SmartReset () {
    }

    protected override bool SmartPerform (GameObject agent) {
        isDone = true;
        return true;
    }

    public override bool RequiresInRange () {
        return true;
    }

    public override bool CheckProceduralPrecondition (GameObject agent) {
        GameObject[] workBuildings = FindGameObjectsOfType<WorkBuildingComponent> ();
        target = FindClosest (agent, workBuildings);

        return target != null;
    }
}
}
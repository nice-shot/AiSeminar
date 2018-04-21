using UnityEngine;
using System;
using System.Collections;

using AI.Goap;

namespace SuzyLemonade {
public class MakeLemonadeAction : SmartAction {
    private LemonadeStandComponent targetStand;

    void Awake() {
        AddEffect("hasLemonade", true);
        AddEffect("hasLemons", false);
        AddPrecondition("hasLemons", true);
    }

    protected override void SmartReset() {
        targetStand = null;
    }

    public override bool RequiresInRange() {
        return true;
    }

    public static bool FilterLemonadeStand(GameObject stand) {
        LemonadeStandComponent lemonadeStand = stand.GetComponent<LemonadeStandComponent> ();
        return lemonadeStand.numJars < lemonadeStand.maxJars; 
    }

    public override bool CheckProceduralPrecondition(GameObject agent) {
        GameObject[] stands = FindGameObjectsOfType<LemonadeStandComponent> ();

        stands = Array.FindAll<GameObject> (stands, FilterLemonadeStand);
        target = FindClosest (agent, stands);

        if (target == null) {
            return false;
        }

        targetStand = target.GetComponent<LemonadeStandComponent> ();
        return targetStand != null;
    }

    protected override bool SmartPerform(GameObject agent) {
        // finished making lemonade
        isDone = true;
        Person agentPerson = agent.GetComponent<Person>();
        // When lemonade is created the lemon is destroyed
        GameObject lemon = agentPerson.DropItem();
        Destroy(lemon);

        targetStand.AddLemonade();

        return true;
    }
}
}
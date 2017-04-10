using UnityEngine;
using System.Collections.Generic;
using Ai.Goap;

namespace TeamFirewood {
/// <summary>
/// Pick up some resources from a harvest point.
/// </summary>
public class PickUpResourceAction : GoapAction {
    public Item resource;
    public int amountToTake;
    private List<IStateful> targets;

    protected void Awake() {
        AddEffect(resource.ToString(), ModificationType.Add, amountToTake);
        AddTargetPrecondition(resource.ToString(), CompareType.MoreThanOrEqual, amountToTake);
        AddTargetEffect(resource.ToString(), ModificationType.Add, -amountToTake);
    }

    protected void Start() {
        targets = GetTargets<HarvestPoint>();
    }
    
    public override bool RequiresInRange() {
        return true;
    }

    public override List<IStateful> GetAllTargets(GoapAgent agent) {
        return targets;
    }

    protected override bool OnDone(GoapAgent agent, WithContext context) {
        var target = context.target as Component;
        var supplyPile = target.GetComponent<Container>();
        if (supplyPile.items[resource] < amountToTake) {
            // Someone got here before us.
            return false;
        }
        supplyPile.items[resource] -= amountToTake;

        // TODO: Play animations when a task is done.
        var backpack = agent.GetComponent<Container>();
        backpack.items[resource] += amountToTake;
        
        return base.OnDone(agent, context);;
    }
}
}

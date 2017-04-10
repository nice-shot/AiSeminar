using System.Collections.Generic;
using Ai.Goap;

namespace TeamFirewood {
/// <summary>
/// Harvest resources from a point of interest.
/// </summary>
public class HarvestAction : GoapAction {
    public float toolDamage;
    public Item resource;
    public PointOfInterestType harvestSource;
    public int amountToHarvest;
    public int maxAmountToCarry;
    private List<IStateful> targets;

    protected virtual void Awake() {
        AddPrecondition("hasTool", CompareType.Equal, true);
        AddPrecondition(resource.ToString(), CompareType.LessThan, maxAmountToCarry);
        AddEffect(resource.ToString(), ModificationType.Add, amountToHarvest);
    }

    protected void Start() {
        // TODO: Use HarvestPoint and have resources like trees and rocks deplete
        //       over time as they are being consumed.
        targets = PointOfInterest.GetPointOfInterest(harvestSource);
    }
    
    public override bool RequiresInRange() {
        return true;
    }

    public override List<IStateful> GetAllTargets(GoapAgent agent) {
        return targets;
    }

    protected override bool OnDone(GoapAgent agent, WithContext context) {
        // Done harvesting.
        var backpack = agent.GetComponent<Container>();
        backpack.items[resource] += amountToHarvest;
        var tool = backpack.tool.GetComponent<ToolComponent>();
        tool.Use(toolDamage);
        if (tool.IsDestroyed) {
            Destroy(backpack.tool);
            backpack.tool = null;
        }
        return base.OnDone(agent, context);
    }
}
}

using System.Collections.Generic;
using Ai.Goap;

namespace TeamFirewood {
/// <summary>
/// Like harvest action but doesn't require a tool.
/// </summary>
public class CollectBranchesAction : GoapAction {
    private const Item resource = Item.Branches;
    public int amountToHarvest;
    public int maxAmountToCarry;
    private List<IStateful> targets;

    protected virtual void Awake() {
        AddPrecondition(resource.ToString(), CompareType.LessThan, maxAmountToCarry);
        AddEffect(resource.ToString(), ModificationType.Add, amountToHarvest);
        AddTargetPrecondition("has" + resource, CompareType.Equal, true);
    }

    protected void Start() {
        targets = GetTargets<Branches>();
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
        return base.OnDone(agent, context);
    }
}
}
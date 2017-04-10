using UnityEngine;
using System.Collections.Generic;
using Ai.Goap;

namespace TeamFirewood {
/// <summary>
/// Pick up a tool from a harvest point.
/// </summary>
public class PickUpToolAction : GoapAction {
    private List<IStateful> targets;

    protected void Awake() {
        AddPrecondition("hasTool", CompareType.Equal, false);
        AddEffect("hasTool", ModificationType.Set, true);
        AddTargetPrecondition(Item.NewTool.ToString(), CompareType.MoreThan, 0);
        AddTargetEffect(Item.NewTool.ToString(), ModificationType.Add, -1);
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
        if (supplyPile.items[Item.NewTool] == 0) {
            return false;
        }
        supplyPile.items[Item.NewTool] -= 1;

        // Create the tool and add it to the agent.

        var backpack = agent.GetComponent<Container>();
        // TODO: Use GameObjectPool to pool tools instead of instantiating new
        //       ones all the time.
        var prefab = Resources.Load<GameObject>(backpack.toolType);
        var tool = Instantiate(prefab, agent.transform.position, agent.transform.rotation);
        backpack.tool = tool;
        tool.transform.parent = agent.transform;

        return base.OnDone(agent, context);
    }
}
}

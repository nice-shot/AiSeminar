using UnityEngine;
using Infra.Utils;
using Ai.Goap;

namespace TeamFirewood {
public class Branches : PointOfInterest {
    [Range(0f, 1f)]
    [SerializeField] float chanceToHaveBranches = 0.5f;
    private readonly State state = new State();

    protected void Awake() {
        state["has" + Item.Branches] =
            new StateValue(RandomUtils.RandBool(chanceToHaveBranches));
    }

    public override State GetState() {
        // Enable to check again if has branches.
        enabled = true;
        return state;
    }

    protected void Update() {
        state["has" + Item.Branches].value =
            RandomUtils.RandBool(chanceToHaveBranches);
        enabled = false;
    }
}
}

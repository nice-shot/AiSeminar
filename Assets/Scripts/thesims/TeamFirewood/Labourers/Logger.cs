using Ai.Goap;

namespace TeamFirewood {
public class Logger : Worker {
    private readonly WorldGoal worldGoal = new WorldGoal();

    protected override void Awake() {
        base.Awake();
        var goal = new Goal();
        goal["create" + Item.Logs] = new Condition(CompareType.Equal, true);
        worldGoal[this] = goal;
    }

    public override WorldGoal CreateGoalState() {
        return worldGoal;
    }
}
}

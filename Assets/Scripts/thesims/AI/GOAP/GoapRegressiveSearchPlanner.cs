using System.Collections.Generic;
using Infra;
using Ai.AStar;

namespace Ai.Goap {
/// <summary>
/// Plans what actions can be completed in order to fulfill a goal state.
/// </summary>
public static class GoapRegressiveSearchPlanner {
    // This seems like enough...
    private const int MAX_FRINGE_NODES = 2000;
    private const int MAX_DEPTH = 8;

    /// <summary>
    /// A* forward search for a plan that satisfies the given goal.
    /// </summary>
    /// <returns>Returns null if a plan could not be found, or a list of the
    /// actions that must be performed, in order.</returns>
    public static Queue<ITransition> Plan(
            GoapAgent agent,
            WorldGoal goal) {
        var worldState = WorldState.Borrow();
        worldState[agent] = agent.GetState();
        var regressiveSearchGoal = RegressiveSearchWorldGoal.Borrow(goal);

        DebugUtils.Assert(worldState[agent].ContainsKey("x")
            && worldState[agent].ContainsKey("x"),
            "Agent's state must contain his position as 'x' and 'y' keys");

        var path = AStarSearch.Search(agent, regressiveSearchGoal, worldState, true);
        worldState.ReturnSelf();

        GoapAction.WithContext.ReportLeaks();
        State.ReportLeaks();
        WorldState.ReportLeaks();
        WorldGoal.ReportLeaks();
        RegressiveSearchWorldGoal.ReportLeaks();
        WorldEffects.ReportLeaks();

        return path;
    }
}
}

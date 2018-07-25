using System.Collections.Generic;
using Infra;
using Ai.AStar;

namespace Ai.Goap {
/// <summary>
/// Plans what actions can be completed in order to fulfill a goal state.
/// </summary>
public static class GoapPlanner {
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

        DebugUtils.Assert(worldState[agent].ContainsKey("x")
                          && worldState[agent].ContainsKey("y")
                          && worldState[agent].ContainsKey("z"),
                          "Agent's state must contain his position as 'x', 'y' and 'z' keys");

        var path = AStarSearch.Search(agent, worldState, goal);

        GoapAction.WithContext.ReportLeaks();
        State.ReportLeaks();
        WorldState.ReportLeaks();
        WorldGoal.ReportLeaks();
        WorldEffects.ReportLeaks();

        return path;
    }
}
}

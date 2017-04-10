using System.Collections.Generic;
using Infra.Collections;

namespace Ai.AStar {
public interface IState : IPoolableObject {
    IEqualityComparer<IState> GetComparer();
    /// <summary>
    /// Returns if this state qualifies as a goal state.
    /// </summary>
    /// <param name="goal">The goal description.</param>
    /// <param name="returnGoal">If set to <c>true</c>, will return the goal
    /// back to its pool.</param>
    bool IsGoalState(IGoal goal, bool returnGoal = true);
    /// <summary>
    /// Returns the possible transitions from this state.
    /// </summary>
    /// <param name="agent">The agent we're searching a path for.</param>
    List<ITransition> GetPossibleTransitions(ISearchContext agent);
    /// <summary>
    /// Calculates the heuristic cost to reach the goal from the current state.
    /// To find the optimal solution, the heuristic has to be admissible - it
    /// should never overestimate the real cost.
    /// It is generally good enough to use an almost admissible heuristic.
    /// </summary>
    float CalculateHeuristicCost(ISearchContext agent, IGoal goal);
}
}
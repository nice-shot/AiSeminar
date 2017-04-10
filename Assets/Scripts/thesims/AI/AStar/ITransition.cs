using Infra.Collections;

namespace Ai.AStar {
public interface ITransition : IPoolableObject {
    ITransition Clone();
    /// <summary>
    /// Apply the transition to given state in order to calculate the next state.
    /// </summary>
    /// <param name="state">The state to transition from.</param>
    /// <param name="inPlace">Set to true if it's ok to modify the given state.</param>
    IState ApplyToState(IState state, bool inPlace = false);
    /// <summary>
    /// Calculates the cost of the transition from the given state.
    /// </summary>
    float CalculateCost(IState fromState);
}
}

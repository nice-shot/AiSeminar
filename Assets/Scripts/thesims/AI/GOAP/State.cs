using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using Infra;
using Infra.Collections;
using Ai.AStar;

namespace Ai.Goap {
public class StateValue {
    public object value;

    public StateValue() {}

    /// <summary>
    /// Supports int or bool values.
    /// Floats will be problematic when comparing states.
    /// </summary>
    public StateValue(object value) {
        this.value = NormalizeValue(value);
    }

    public static object NormalizeValue(object value) {
        if (value is float) {
            return Mathf.RoundToInt((float)value);
        }
        return value;
    }

    public void ApplyEffect(Effect e) {
        switch (e.modifier) {
        case ModificationType.Set:
            value = e.value;
            break;
        case ModificationType.Add:
            var v1 = (int)e.value;
            var v2 = (int)value;
            value = v2 + v1;
            break;
        }
    }

    public bool CheckCondition(Condition c) {
        double v1;
        double v2;
        switch (c.comparison) {
        case CompareType.Equal:
            return value.Equals(c.value);
        case CompareType.NotEqual:
            return !value.Equals(c.value);
        case CompareType.MoreThan:
            v1 = (int)c.value;
            v2 = (int)value;
            return v2 > v1;
        case CompareType.MoreThanOrEqual:
            v1 = (int)c.value;
            v2 = (int)value;
            return v2 > v1 || value.Equals(c.value);
        case CompareType.LessThan:
            v1 = (int)c.value;
            v2 = (int)value;
            return v2 <= v1;
        case CompareType.LessThanOrEqual:
            v1 = (int)c.value;
            v2 = (int)value;
            return v2 <= v1 || value.Equals(c.value);
        }
        return false;
    }
}

public class State : Dictionary<string, StateValue>, IPoolableObject {
    private static ObjectPool<State> pool = new ObjectPool<State>(20, 8);
    private static int lastPoolSize = 20;

    public static void ReportLeaks() {
        var poolSize = pool.Count;
        if (poolSize > lastPoolSize) {
            DebugUtils.LogError("State pool size: " + poolSize);
            lastPoolSize = poolSize;
        }
    }

    public static State Borrow() {
        var obj = pool.Borrow();
        obj.Clear();
        return obj;
    }

    public void ReturnSelf() {
        pool.Return(this);
    }

    /// <summary>
    /// Check that all items in 'test' are in 'state'. If just one does not
    /// match or is not there then this returns false.
    /// </summary>
    public bool DoConditionsApply(Goal goal) {
        foreach (var t in goal) {
            if (!ContainsKey(t.Key) || !this[t.Key].CheckCondition(t.Value)) {
                return false;
            }
        }
        return true;
    }

    public int UnmetConditionsCount(Goal goal) {
        var count = 0;
        foreach (var t in goal) {
            if (!ContainsKey(t.Key) || !this[t.Key].CheckCondition(t.Value)) {
                ++count;
            }
        }
        return count;
    }

    /// <summary>
    /// Apply the stateChange to the currentState.
    /// Does not modify the current state. Returns a copy.
    /// </summary>
    public State ApplyEffects(Effects stateChange) {
        // Copy state.
        var state = new State();
        foreach (var item in this) {
            state[item.Key] = item.Value;
        }
        // Apply change.
        foreach (var change in stateChange) {
            if (!state.ContainsKey(change.Key)) {
                state[change.Key] = new StateValue(change.Value.value);
            } else {
                // Only copy StateValue when changing it.
                state[change.Key] = new StateValue(state[change.Key].value);
                state[change.Key].ApplyEffect(change.Value);
            }
        }
        return state;
    }

    public override string ToString() {
        var s = new StringBuilder();
        foreach (var kvp in this) {
            s.Append(kvp.Key).Append(':').Append(kvp.Value.value).Append(", ");
        }
        return s.ToString();
    }
}

/// <summary>
/// A dictionary of stateful objects and their state.
/// Can also be a goal if used in GoapRegressiveSearchPlanner.
/// </summary>
public class WorldState : Dictionary<IStateful, State>, IState, IGoal {
    private static ObjectPool<WorldState> pool = new ObjectPool<WorldState>(150, 5);
    private static int lastPoolSize = 150;

    public static void ReportLeaks() {
        var poolSize = pool.Count;
        if (poolSize > lastPoolSize) {
            DebugUtils.LogError("WorldState pool size: " + poolSize);
            lastPoolSize = poolSize;
        }
    }

    public static WorldState Borrow() {
        var obj = pool.Borrow();
        obj.Clear();
        return obj;
    }

    public void ReturnSelf() {
        pool.Return(this);
    }

    public IEqualityComparer<IState> GetComparer() {
        return WorldStateComparer.instance;
    }

    /// <summary>
    /// Returns if this state qualifies as a goal state.
    /// Checks that all items in the goal match the current state. If an item
    /// does not exist in the state, it expands it to contain that item's state.
    /// </summary>
    public bool IsGoalState(IGoal goal, bool returnGoal = true) {
        var worldGoal = goal as WorldGoal;
        DebugUtils.Assert(worldGoal != null, "Expected WorldGoal but got " + goal);
        var isGoal = true;
        foreach (var targetGoal in worldGoal) {
            if (!ContainsKey(targetGoal.Key)) {
                this[targetGoal.Key] = targetGoal.Key.GetState();
            }
            var targetState = this[targetGoal.Key];
            if (!targetState.DoConditionsApply(targetGoal.Value)) {
                isGoal = false;
                break;
            }
        }
        if (returnGoal) {
            worldGoal.ReturnSelf();
        }
        return isGoal;
    }

    public int UnmetConditionsCount(IGoal goal) {
        var worldGoal = goal as WorldGoal;
        DebugUtils.Assert(worldGoal != null, "Expected WorldGoal but got " + goal);
        var count = 0;
        foreach (var targetGoal in worldGoal) {
            if (!ContainsKey(targetGoal.Key)) {
                this[targetGoal.Key] = targetGoal.Key.GetState();
            }
            var targetState = this[targetGoal.Key];
            count += targetState.UnmetConditionsCount(targetGoal.Value);
        }
        return count;
    }

    /// <summary>
    /// Returns the possible transitions from this state.
    /// </summary>
    /// <param name="agent">The agent we're searching a path for.</param>
    public List<ITransition> GetPossibleTransitions(ISearchContext agent) {
        var goapAgent = agent as GoapAgent;
        DebugUtils.Assert(goapAgent != null, "Expected GoapAgent but got " + agent);
        var possibleActions = new List<ITransition>();
        var availableActions = goapAgent.availableActions;
        foreach (var action in availableActions) {
            if (!IsGoalState(action.GetIndependentPreconditions(goapAgent))) {
                // Action does not apply to this state.
                //DebugUtils.LogError("Action: " + action + " does not apply to agent");
                continue;
            }
            var targets = action.GetAllTargets(goapAgent);
            //DebugUtils.LogError("Checking Action: " + GoapAgent.PrettyPrint(action) + " got targets: " + targets.Count);
            foreach (var target in targets) {
                if (goapAgent != (target as GoapAgent)
                    && !IsGoalState(action.GetDependentPreconditions(goapAgent, target))) {
                    // Action does not apply to this target.
                    //DebugUtils.LogError("Action does not apply for target: " + (target as Component).name);
                    continue;
                }
                possibleActions.Add(GoapAction.WithContext.Borrow(action, goapAgent, target));
            }
        }
        return possibleActions;
    }

    /// <summary>
    /// Calculates the heuristic cost to reach the goal from the current state.
    /// To find the optimal solution, the heuristic has to be admissible - it
    /// should never overestimate the real cost.
    /// It is generally good enough to use an almost admissible heuristic.
    /// </summary>
    public float CalculateHeuristicCost(ISearchContext agent, IGoal goal) {
        // TODO: Calculate heuristic for A*.
        return 0f;
    }

    public override string ToString() {
        var s = new StringBuilder();
        foreach (var kvp in this) {
            var target = kvp.Key as Component;
            s.Append(target.name).Append(": ").Append(kvp.Value.ToString()).Append('\n');
        }
        return s.ToString();
    }
}

/// <summary>
/// This is needed to deep-compare the states by value and not by reference.
/// </summary>
public class StateComparer : IEqualityComparer<State> {
    public static StateComparer instance = new StateComparer();
    public bool Equals(State state1, State state2) {
        if (state1 == state2) return true;
        if ((state1 == null) || (state2 == null)) return false;
        if (state1.Count != state2.Count) return false;

        foreach (var kvp in state1) {
            StateValue value2;
            if (!state2.TryGetValue(kvp.Key, out value2)) return false;
            if (!kvp.Value.value.Equals(value2.value)) return false;
        }
        return true;
    }
    public int GetHashCode(State obj) {
        return obj.ToString().GetHashCode();
    }
}

/// <summary>
/// This is needed to deep-compare the states by value and not by reference.
/// </summary>
public class WorldStateComparer : IEqualityComparer<IState> {
    public static WorldStateComparer instance = new WorldStateComparer();
    public bool Equals(IState lhs, IState rhs) {
        if (lhs == rhs) return true;
        if ((lhs == null) || (rhs == null)) return false;

        var state1 = lhs as WorldState;
        var state2 = rhs as WorldState;
        foreach (var kvp in state1) {
            State value2;
            if (!state2.TryGetValue(kvp.Key, out value2)) {
                state2[kvp.Key] = kvp.Key.GetState();
            }
            if (!StateComparer.instance.Equals(kvp.Value, value2)) return false;
        }

        foreach (var kvp in state2) {
            State value1;
            if (!state1.TryGetValue(kvp.Key, out value1)) {
                state1[kvp.Key] = kvp.Key.GetState();
            }
            if (!StateComparer.instance.Equals(kvp.Value, value1)) return false;
        }
        return true;
    }
    public int GetHashCode(IState obj) {
        return obj.ToString().GetHashCode();
    }
}
}
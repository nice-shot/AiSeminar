using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using Infra;
using Infra.Collections;
using Ai.AStar;

namespace Ai.Goap {
public class Goal : Dictionary<string, Condition> {
    public bool AreEffectsRelevantToGoal(Effects effects, State state) {
        DebugUtils.Assert(effects != null, "Effects is null");
        foreach (var property in Keys) {
            if (effects.ContainsKey(property)) {
                var condition = this[property];
                if (state.ContainsKey(property)) {
                    var value = state[property];
                    // A condition is only relevant if it is not met by the state.
                    if (value.CheckCondition(condition)) continue;
                }
                if (condition.IsRelevant(effects[property])) {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Regress goal to before effects.
    /// </summary>
    public Goal RegressGoal(Effects effects) {
        // Copy goal.
        var goal = new Goal();
        foreach (var item in this) {
            goal[item.Key] = item.Value;
        }
        // Apply change.
        foreach (var effect in effects.Keys) {
            if (!goal.ContainsKey(effect)) {
                // Effect doesn't relate to goal. We trust the action contains a
                // condition to make sure this effect can happen.
                continue;
            }
            var newCondition = goal[effect].Affect(effects[effect]);
            if (newCondition == null) {
                goal.Remove(effect);
            } else {
                goal[effect] = newCondition;
            }
        }
        return goal;
    }

    public override string ToString() {
        var s = new StringBuilder();
        foreach (var kvp in this) {
            s.Append(kvp.Key).Append(':')
                .Append(kvp.Value.comparison).Append(' ')
                .Append(kvp.Value.value).Append(", ");
        }
        return s.ToString();
    }
}

/// <summary>
/// A dictionary of stateful objects and the goal we want them to be at.
/// </summary>
public class WorldGoal : Dictionary<IStateful, Goal>, IGoal {
    private static ObjectPool<WorldGoal> pool = new ObjectPool<WorldGoal>(100, 40);
    private static int lastPoolSize = 100;

    public static void ReportLeaks() {
        var poolSize = pool.Count;
        if (poolSize > lastPoolSize) {
            DebugUtils.LogError("WorldGoal pool size: " + poolSize);
            lastPoolSize = poolSize;
        }
    }

    public static WorldGoal Borrow() {
        var obj = pool.Borrow();
        obj.Clear();
        return obj;
    }

    public virtual void ReturnSelf() {
        pool.Return(this);
    }

    public override string ToString() {
        var s = new StringBuilder();
        foreach (var kvp in this) {
            var target = kvp.Key as Component;
            s.Append(target.name).Append(": ").Append(kvp.Value).Append('\n');
        }
        return s.ToString();
    }
}

public class RegressiveSearchWorldGoal : WorldGoal, IState {
    private static ObjectPool<RegressiveSearchWorldGoal> pool = new ObjectPool<RegressiveSearchWorldGoal>(100, 40);
    private static int lastPoolSize = 100;

    public Vector2? agentGoalPosition;

    public static new void ReportLeaks() {
        var poolSize = pool.Count;
        if (poolSize > lastPoolSize) {
            DebugUtils.LogError("RegressiveSearchWorldGoal pool size: " + poolSize);
            lastPoolSize = poolSize;
        }
    }

    public static new RegressiveSearchWorldGoal Borrow() {
        var obj = pool.Borrow();
        obj.Clear();
        obj.agentGoalPosition = null;
        return obj;
    }

    public static RegressiveSearchWorldGoal Borrow(WorldGoal parent) {
        var obj = Borrow();
        foreach (var item in parent) {
            obj[item.Key] = item.Value;
        }
        return obj;
    }

    public override void ReturnSelf() {
        pool.Return(this);
    }

    public IEqualityComparer<IState> GetComparer() {
        return WorldGoalComparer.instance;
    }

    public bool IsGoalState(IGoal goal, bool returnGoal = true) {
        var worldState = goal as WorldState;
        return worldState.IsGoalState(this, returnGoal);
    }

    public List<ITransition> GetPossibleTransitions(ISearchContext agent) {
        var goapAgent = agent as GoapAgent;
        DebugUtils.Assert(goapAgent != null, "Expected GoapAgent but got " + agent);
        var possibleActions = new List<ITransition>();
        var availableActions = goapAgent.availableActions;
        foreach (var action in availableActions) {
            var targets = action.GetAllTargets(goapAgent);
            foreach (var target in targets) {
                var effects = action.GetDependentEffects(goapAgent, target);
                if ((ContainsKey(goapAgent) && effects.ContainsKey(goapAgent) &&
                    this[goapAgent].AreEffectsRelevantToGoal(effects[goapAgent], goapAgent.GetState())) ||
                    (ContainsKey(target) && effects.ContainsKey(target) &&
                        this[target].AreEffectsRelevantToGoal(effects[target], target.GetState()))) {
                    possibleActions.Add(GoapAction.WithContext.Borrow(action, goapAgent, target));
                }
                effects.ReturnSelf();
            }
        }
        return possibleActions;
    }

    /// <summary>
    /// Calculates the heuristic cost to reach the goal from the given state.
    /// To find the optimal solution, the heuristic has to be admissible - it
    /// should never overestimate the real cost.
    /// It is generally good enough to use an almost admissible heuristic.
    /// </summary>
    public float CalculateHeuristicCost(ISearchContext agent, IGoal goal) {
        var goapAgent = agent as GoapAgent;
        var worldState = goal as WorldState;
        var cost = 0f;
        // Add distance to current position.
        if (agentGoalPosition != null) {
            var agentState = worldState[goapAgent];
            var currentPosition = new Vector2((int)agentState["x"].value, (int)agentState["y"].value);
            var travelVector = (Vector2)agentGoalPosition - currentPosition;
            cost += travelVector.magnitude;
        }
        // Add heuristic cost for regressive search - number of unmet goals.
        cost += worldState.UnmetConditionsCount(this);
        //DebugUtils.Log("Heuristic cost: " + cost);
        return cost;
    }

    /// <summary>
    /// Regress worldGoal to before effects
    /// </summary>
    public RegressiveSearchWorldGoal RegressWorldGoal(WorldEffects worldEffects, bool inPlace = false) {
        var worldGoal = this;
        if (!inPlace) {
            worldGoal = RegressiveSearchWorldGoal.Borrow();
            foreach (var goal in this) {
                worldGoal[goal.Key] = goal.Value;
            }
        }
        // Reverse change.
        foreach (var effects in worldEffects) {
            if (worldGoal.ContainsKey(effects.Key)) {
                worldGoal[effects.Key] = worldGoal[effects.Key].RegressGoal(effects.Value);
            }
        }
        return worldGoal;
    }

    public void AddPreconditions(WorldGoal preconditions, bool returnPreconditions = true) {
        foreach (var stateful in preconditions.Keys) {
            if (!ContainsKey(stateful)) {
                Add(stateful, new Goal());
            }
            foreach (var goalString in preconditions[stateful].Keys) {
                if (this[stateful].ContainsKey(goalString)) {
                    this[stateful][goalString] = preconditions[stateful][goalString].Refine(this[stateful][goalString]);
                } else {
                    this[stateful].Add(goalString, preconditions[stateful][goalString]);
                }
            }
        }
        if (returnPreconditions) {
            preconditions.ReturnSelf();
        }
    }

    public override string ToString() {
        return base.ToString() + "Position:" + (agentGoalPosition == null ? "?" : agentGoalPosition.ToString());
    }
}

/// <summary>
/// This is needed to deep-compare the goals by value and not by reference.
/// </summary>
public class GoalComparer : IEqualityComparer<Goal> {
    public static GoalComparer instance = new GoalComparer();

    public bool Equals(Goal goal1, Goal goal2) {
        if (goal1 == goal2) return true;
        if ((goal1 == null) || (goal2 == null)) return false;

        if (goal1.Count != goal2.Count) return false;

        foreach (var key in goal1.Keys) {
            if (!goal2.ContainsKey(key) || !ConditionComparer.instance.Equals(goal2[key], goal1[key]))
                return false;
        }

        foreach (var key in goal2.Keys) {
            if (!goal1.ContainsKey(key) || !ConditionComparer.instance.Equals(goal2[key], goal1[key]))
                return false;
        }

        return true;
    }

    public int GetHashCode(Goal obj) {
        return obj.ToString().GetHashCode();
    }
}

/// <summary>
/// This is needed to deep-compare the WorldGoal by value and not by reference.
/// </summary>
public class WorldGoalComparer : IEqualityComparer<IState> {
    public static WorldGoalComparer instance = new WorldGoalComparer();

    public bool Equals(IState lhs, IState rhs) {
        if (lhs == rhs) return true;
        if ((lhs == null) || (rhs == null)) return false;

        var worldGoal1 = lhs as WorldGoal;
        var worldGoal2 = rhs as WorldGoal;

        if (worldGoal1.Count != worldGoal2.Count) return false;

        foreach (var key in worldGoal1.Keys) {
            if (!worldGoal2.ContainsKey(key)
                || !GoalComparer.instance.Equals(worldGoal2[key], worldGoal1[key])) {
                return false;
            }
        }

        foreach (var key in worldGoal2.Keys) {
            if (!worldGoal1.ContainsKey(key)
                || !GoalComparer.instance.Equals(worldGoal2[key], worldGoal1[key])) {
                return false;
            }
        }
        return true;
    }

    public int GetHashCode(IState obj) {
        return obj.ToString().GetHashCode();
    }
}
}
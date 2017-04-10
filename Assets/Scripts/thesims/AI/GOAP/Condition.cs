using System;
using System.Collections.Generic;
using System.Text;

namespace Ai.Goap {
public enum CompareType {
    Equal,
    NotEqual,
    MoreThan,
    MoreThanOrEqual,
    LessThan,
    LessThanOrEqual,
}

/// <summary>
/// A condition that a StateValue can meet.
/// </summary>
public class Condition {
    public CompareType comparison;
    public object value;

    public Condition() {}

    public Condition(CompareType comparison, object value) {
        this.comparison = comparison;
        this.value = StateValue.NormalizeValue(value);
    }

    public bool IsRelevant(Effect e) {
        //DebugUtils.LogError("mod: " + e.modifier + " value: " + e.value + " Comp: " + comparison + " tValue: " + value);
        switch (comparison) {
        case CompareType.Equal:
            switch (e.modifier) {
            case ModificationType.Set:
                return (value.Equals(e.value));
            case ModificationType.Add:
                // If we need the value to be specific, we might have to overshoot
                // it in order to add another action that will hit our target.
                // It depends on the actions we have at our disposal.
                return (int)e.value != 0;
            }
            break;
        case CompareType.NotEqual:
            switch (e.modifier) {
            case ModificationType.Set:
                return !(value.Equals(e.value));
            case ModificationType.Add:
                return (int)e.value != 0;
            }
            break;
        case CompareType.MoreThan:
            switch (e.modifier) {
            case ModificationType.Set:
                return ((int)value < (int)e.value);
            case ModificationType.Add:
                return (int)e.value > 0;
            }
            break;
        case CompareType.MoreThanOrEqual:
            switch (e.modifier) {
            case ModificationType.Set:
                return ((int)value <= (int)e.value);
            case ModificationType.Add:
                return (int)e.value > 0;
            }
            break;
        case CompareType.LessThan:
            switch (e.modifier) {
                case ModificationType.Set:
                return ((int)value > (int)e.value);
                case ModificationType.Add:
                return (int)e.value < 0;
            }
            break;
        case CompareType.LessThanOrEqual:
            switch (e.modifier) {
                case ModificationType.Set:
                return ((int)value >= (int)e.value);
                case ModificationType.Add:
                return (int)e.value < 0;
            }
            break;
        }
        return true;
    }

    public Condition Affect(Effect e) {
        switch (comparison) {
        case CompareType.Equal:
            if (e.modifier == ModificationType.Set) return null;
            break;
        case CompareType.NotEqual:
        case CompareType.MoreThan:
        case CompareType.LessThan:
        case CompareType.MoreThanOrEqual:
        case CompareType.LessThanOrEqual:
            switch (e.modifier) {
            case ModificationType.Set:
                return null;
            case ModificationType.Add:
                var newValue = (int)value - (int)e.value;
                //DebugUtils.Log("Regressing condition " + comparison + " " + value + " to before " + e.modifier + " " + e.value + ": " + newValue);
                return newValue < 0 ? null : new Condition(comparison, newValue);
            // Negative quantities of items are not supported
            }
            break;
        }
        return this;
    }

    /// <summary>
    /// Refine the other condition using this condition. Making it more strict.
    /// If the conditions do not overlap, returns the other condition.
    /// </summary>
    public Condition Refine(Condition other) {
        var returnSelf = false;
        switch (comparison) {
        case CompareType.Equal:
            switch (other.comparison) {
            case CompareType.MoreThan:
                returnSelf = (int)value > (int)other.value;
                break;
            case CompareType.LessThan:
                returnSelf = (int)value < (int)other.value;
                break;
            case CompareType.MoreThanOrEqual:
                returnSelf = (int)value >= (int)other.value;
                break;
            case CompareType.LessThanOrEqual:
                returnSelf = (int)value <= (int)other.value;
                break;
            }
            break;
        case CompareType.MoreThan:
        case CompareType.MoreThanOrEqual:
            switch (other.comparison) {
            case CompareType.MoreThan:
            case CompareType.MoreThanOrEqual:
                returnSelf = (int)value > (int)other.value;
                break;
            }
            break;
        case CompareType.LessThan:
        case CompareType.LessThanOrEqual:
            switch (other.comparison) {
            case CompareType.LessThan:
            case CompareType.LessThanOrEqual:
                returnSelf = (int)value < (int)other.value;
                break;
            }
            break;
        }
        return returnSelf ? this : other;
    }

    public override string ToString() {
        var s = new StringBuilder();
        s.Append(comparison.ToString());
        s.Append(value);
        return s.ToString();
    }
}

/// <summary>
/// This is needed to deep-compare the conditions by value and not by reference.
/// </summary>
public class ConditionComparer : IEqualityComparer<Condition> {
    public static ConditionComparer instance = new ConditionComparer();

    public bool Equals(Condition cond1, Condition cond2) {
        if (cond1 == cond2) return true;
        if ((cond1 == null) || (cond2 == null)) return false;
        return cond1.comparison.Equals(cond2.comparison) && cond1.value.Equals(cond2.value);
    }

    public int GetHashCode(Condition obj) {
        return obj.ToString().GetHashCode();
    }
}
}
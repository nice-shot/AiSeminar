using System;
using System.Collections.Generic;
using Infra;
using Infra.Collections;

namespace Ai.Goap {
public enum ModificationType {
    Set,
    // Add is supported only for ints.
    Add,
}

/// <summary>
/// Can be applied to change a StateValue.
/// </summary>
public class Effect {
    public ModificationType modifier;
    public object value;

    public Effect() {
    }

    public Effect(Effect effect) {
        modifier = effect.modifier;
        value = effect.value;
    }

    public Effect(ModificationType modifier, object value) {
        this.modifier = modifier;
        this.value = StateValue.NormalizeValue(value);
    }
}

public class Effects : Dictionary<string, Effect> {
}

/// <summary>
/// A dictionary of stateful objects and how an action might change their state.
/// </summary>
public class WorldEffects : Dictionary<IStateful, Effects>, IPoolableObject {
    private static ObjectPool<WorldEffects> pool = new ObjectPool<WorldEffects>(40, 40);
    private static int lastPoolSize = 40;

    public static void ReportLeaks() {
        var poolSize = pool.Count;
        if (poolSize > lastPoolSize) {
            DebugUtils.LogError("WorldEffects pool size: " + poolSize);
            lastPoolSize = poolSize;
        }
    }

    public static WorldEffects Borrow() {
        var obj = pool.Borrow();
        obj.Clear();
        return obj;
    }

    public void ReturnSelf() {
        pool.Return(this);
    }
}
}
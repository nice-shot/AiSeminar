using UnityEngine;
using System.Collections.Generic;

namespace Ai.Fsm {
/// <summary>
/// Stack-based Finite State Machine.
/// Push and pop states to the FSM.
/// 
/// States should push other states onto the stack and pop themselves off.
/// </summary>
public class FSM {
    private readonly Stack<FSMState> stateStack = new Stack<FSMState>();

    /// <summary>
    /// FSM state function signature.
    /// </summary>
    public delegate void FSMState(FSM fsm);

    public void Update() {
        var state = stateStack.Peek();
        if (state != null) {
            state(this);
        }
    }

    public void PushState(FSMState state) {
        stateStack.Push(state);
    }

    public void PopState() {
        stateStack.Pop();
    }
}
}

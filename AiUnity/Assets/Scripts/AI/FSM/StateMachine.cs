using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Stack-based Finite State Machine.
 * Push and pop states to the FSM.
 * 
 * States should push other states onto the stack 
 * and pop themselves off.
 */
namespace AI.FSM {
public class StateMachine {
    private Stack<FSMState> stateStack = new Stack<FSMState>();

    public delegate void FSMState (StateMachine fsm, GameObject gameObject);

    public void Update (GameObject gameObject) {
        if (stateStack.Peek() != null) {
            stateStack.Peek().Invoke(this, gameObject);
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
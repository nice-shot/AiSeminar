using UnityEngine;

namespace AI.FSM {
public interface FSMState {
    void Update(FSM fsm, GameObject gameObject);
}
}
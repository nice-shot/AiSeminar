using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI.FSM;

namespace AI.Goap {
public sealed class GoapAgent : MonoBehaviour {
    private StateMachine stateMachine;

    private StateMachine.FSMState idleState; // finds something to do
    private StateMachine.FSMState moveToState; // moves to a target
    private StateMachine.FSMState performActionState; // perform an action

    private HashSet<GoapAction> availableActions;
    private Queue<GoapAction> currentActions;

    // this is the implementing class that provides our world data and listens to feedback on planning
    private IGoap dataProvider;

    private GoapPlanner planner;

    void Start () {
        stateMachine = new StateMachine ();
        availableActions = new HashSet<GoapAction> ();
        currentActions = new Queue<GoapAction> ();
        planner = new GoapPlanner ();
        FindDataProvider ();
        CreateIdleState ();
        CreateMoveToState ();
        CreatePerformActionState ();
        stateMachine.PushState (idleState);
        LoadActions ();
    }

    void Update() {
        stateMachine.Update(this.gameObject);
    }

    public void AddAction(GoapAction a) {
        availableActions.Add (a);
    }

    public GoapAction GetAction(Type action) {
        foreach (GoapAction g in availableActions) {
            if (g.GetType().Equals(action) )
                return g;
        }
        return null;
    }

    public void RemoveAcion(GoapAction action) {
        availableActions.Remove(action);
    }

    private bool HasActionPlan() {
        return currentActions.Count > 0;
    }

    private void CreateIdleState() {
        idleState = (fsm, gameObj) => {
            // GOAP planning

            // get the world state and the goal we want to plan for
            HashSet<KeyValuePair<string,object>> worldState = dataProvider.GetWorldState();
            HashSet<KeyValuePair<string,object>> goal = dataProvider.CreateGoalState();

            // Plan
            Queue<GoapAction> plan = planner.Plan(gameObject, availableActions, worldState, goal);
            if (plan != null) {
                // we have a plan, hooray!
                currentActions = plan;
                dataProvider.PlanFound(goal, plan);

                fsm.PopState(); // move to PerformAction state
                fsm.PushState(performActionState);

            } else {
                // ugh, we couldn't get a plan
                Debug.Log("<color=orange>Failed Plan:</color>"+PrettyPrint(goal));
                dataProvider.PlanFailed(goal);
                fsm.PopState (); // move back to IdleAction state
                fsm.PushState (idleState);
            }
        };
    }

    private void CreateMoveToState() {
        moveToState = (fsm, gameObj) => {
            // move the game object

            GoapAction action = currentActions.Peek();
            if (action.RequiresInRange() && action.target == null) {
                string errMsg = "<color=red>Fatal error:</color> Action requires a target but has none.";
                errMsg += " Planning failed. You did not assign the target in your Action.CheckProceduralPrecondition()";
                Debug.Log(errMsg);
                fsm.PopState(); // move
                fsm.PopState(); // perform
                fsm.PushState(idleState);
                return;
            }

            // get the agent to move itself
            if (dataProvider.MoveAgent(action)) {
                fsm.PopState();
            }
        };
    }

    private void CreatePerformActionState() {
        performActionState = (fsm, gameObj) => {
            // perform the action

            if (!HasActionPlan()) {
                // no actions to perform
                Debug.Log("<color=red>Done actions</color>");
                fsm.PopState();
                fsm.PushState(idleState);
                dataProvider.ActionsFinished();
                return;
            }

            GoapAction action = currentActions.Peek();
            if ( action.IsDone() ) {
                // the action is done. Remove it so we can perform the next one
                currentActions.Dequeue();
            }

            if (HasActionPlan()) {
                // perform the next action
                action = currentActions.Peek();
                bool inRange = action.RequiresInRange() ? action.IsInRange() : true;

                if ( inRange ) {
                    // we are in range, so perform the action
                    bool success = action.Perform(gameObj);

                    if (!success) {
                        // action failed, we need to plan again
                        fsm.PopState();
                        fsm.PushState(idleState);
                        dataProvider.planAborted(action);
                    }
                } else {
                    // we need to move there first
                    // push moveTo state
                    fsm.PushState(moveToState);
                }

            } else {
                // no actions left, move to Plan state
                fsm.PopState();
                fsm.PushState(idleState);
                dataProvider.ActionsFinished();
            }

        };
    }

    private void FindDataProvider() {
        foreach (Component comp in gameObject.GetComponents(typeof(Component)) ) {
            if ( typeof(IGoap).IsAssignableFrom(comp.GetType()) ) {
                dataProvider = (IGoap)comp;
                return;
            }
        }
    }

    private void LoadActions ()
    {
        GoapAction[] actions = gameObject.GetComponents<GoapAction>();
        foreach (GoapAction a in actions) {
            availableActions.Add (a);
        }
        Debug.Log("Found actions: "+PrettyPrint(actions));
    }

    public static string PrettyPrint(HashSet<KeyValuePair<string,object>> state) {
        String s = "";
        foreach (KeyValuePair<string,object> kvp in state) {
            s += kvp.Key + ":" + kvp.Value.ToString();
            s += ", ";
        }
        return s;
    }

    public static string PrettyPrint(Queue<GoapAction> actions) {
        String s = "";
        foreach (GoapAction a in actions) {
            s += a.GetType().Name;
            s += "-> ";
        }
        s += "GOAL";
        return s;
    }

    public static string PrettyPrint(GoapAction[] actions) {
        String s = "";
        foreach (GoapAction a in actions) {
            s += a.GetType().Name;
            s += ", ";
        }
        return s;
    }

    public static string PrettyPrint(GoapAction action) {
        String s = ""+action.GetType().Name;
        return s;
    }
}
}
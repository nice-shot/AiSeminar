using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Infra.Utils;
using Ai.Goap;
using Ai.AStar;

namespace RoomEscape {
    public abstract class Survivor : GoapAgent {
        [Tooltip("For 2D movement")]
        public float moveSpeed;
        public ThoughtBubbleController toughtBubble;


        [SerializeField] private Container holding;

        private NavMeshAgent navAgent;
        private readonly State state = new State();
        private ActionBase currentAction;

        // Used for multiple goals
        protected WorldGoal[] worldGoals;
        private int goalIndex = 0;

        protected override void Awake() {
            base.Awake();

            navAgent = GetComponent<NavMeshAgent>();

            if (holding == null) {
                holding = GetComponent<Container>();
            }
        }

        public override State GetState() {
            // Get data from all sensors
            foreach(SensorBase sensor in GetComponents<SensorBase>()) {
                foreach (var stateVal in sensor.GetState()) {
                    state[stateVal.Key] = stateVal.Value;
                }
            }

            if (holding != null) {
                state[States.HELD_ITEM] = new StateValue((int)holding.GetItemType());
            }
            state[States.X] = new StateValue(transform.position.x);
            state[States.Y] = new StateValue(transform.position.y);
            state[States.Z] = new StateValue(transform.position.z);
            return state;
        }

        public override WorldGoal CreateGoalState() {
            return worldGoals[goalIndex];
        }

        public override void PlanFailed(WorldGoal failedGoal) {
            // If this happens for too long, there is probably a bug in the actions,
            // goals or world setup.
            // TODO: Make sure the world state has changed before running the same
            //       goal again.
            // TODO: Support multiple goals and select the next one.
            toughtBubble.SetActionText("...");
            // Debug messages are called in the planner

            // Allow switching between multiple goals
            goalIndex = (goalIndex + 1) % worldGoals.Length;
        }

        public override void PlanFound(WorldGoal goal, Queue<ITransition> actions) {
            // Yay we found a plan for our goal!
            Debug.Log("<color=green>Plan found</color> " + GoapAgent.PrettyPrint(actions));
            
            // Once a plan was found we can come back to the main goal
            goalIndex = 0;
        }

        public override void AboutToDoAction(GoapAction.WithContext action) {
            toughtBubble.SetActionText(action.actionData.name);
            currentAction = action.actionData as ActionBase;
        }

        public override void ActionsFinished() {
            // Everything is done, we completed our actions for this gool. Hooray!
            Debug.Log("<color=blue>Actions completed</color>");
            toughtBubble.SetActionText("Job's Done!");
            if (currentAction != null && currentAction.successMsg != "") {
                toughtBubble.SetExtraText(currentAction.successMsg, true);
            }
        }

        public override void PlanAborted(GoapAction.WithContext aborter) {
            // An action bailed out of the plan. State has been reset to plan again.
            // Take note of what happened and make sure if you run the same goal
            // again that it can succeed.
            Debug.Log("<color=red>Plan Aborted</color> " + aborter);
            toughtBubble.SetActionText("Hmp!");
            if (currentAction.successMsg != "") {
                toughtBubble.SetExtraText(currentAction.failMsg, false);
            }
        }

        public override bool MoveAgent(GoapAction.WithContext nextAction) {
            if (navAgent != null) {
                return Move3D(nextAction);
            }

            return Move2D(nextAction);
        }

        private bool Move2D(GoapAction.WithContext nextAction) { 
            // Move towards the NextAction's target.
            float step = moveSpeed * Time.deltaTime;
            Component target = nextAction.target as Component;
            // NOTE: We must cast to Vector2, otherwise we'll compare the Z coordinate
            //       which does not have to match!
            Vector2 position = (Vector2)target.transform.position;
            // TODO: Move by setting the velocity of a rigid body to allow collisions.
            transform.position = Vector2.MoveTowards(transform.position, position, step);

            if (position == (Vector2)transform.position) {
                // We are at the target location, we are done.
                nextAction.isInRange = true;
                return true;
            }
            return false;
        }

        private bool Move3D(GoapAction.WithContext nextAction) {
            Component target = nextAction.target as Component;
            // Game object with name "NavTarget" marks where to navigate to
            Transform navTarget = target.transform.Find("NavTarget");

            if (navTarget != null) {
                navAgent.SetDestination(navTarget.position);
                //navAgent.SetDestination(target.transform.position);
            } else {
                navAgent.SetDestination(target.transform.position);
            }


            if (!navAgent.pathPending) {
                if (navAgent.remainingDistance <= navAgent.stoppingDistance) {
                    if (!navAgent.hasPath || navAgent.velocity.sqrMagnitude == 0f) {
                        nextAction.isInRange = true;
                        return true;
                    }
                }
            }

            return false;
        }

        protected WorldGoal[] GoalsToWorldGoal(Goal[] goals) {
            WorldGoal[] worldGoals = new WorldGoal[goals.Length];
            for (int i=0; i<goals.Length; i++) {
                WorldGoal worldGoal = new WorldGoal();
                worldGoal[this] = goals[i];
                worldGoals[i] = worldGoal;
            }
            return worldGoals;
        }
    }
}
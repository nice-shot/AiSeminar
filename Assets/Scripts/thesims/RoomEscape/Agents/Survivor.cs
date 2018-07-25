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
        public Text thoughtBubble;

        [SerializeField]
        private Container holding;

        private NavMeshAgent navAgent;
        private readonly State state = new State();

        protected override void Awake() {
            base.Awake();

            navAgent = GetComponent<NavMeshAgent>();

            if (holding == null) {
                holding = GetComponent<Container>();
            }
            thoughtBubble = GetComponentInChildren<Text>();
        }

        public override State GetState() {
            if (holding != null) {
                foreach (ItemType itemType in EnumUtils.EnumValues<ItemType>()) {
                    if (itemType == ItemType.None) continue;
                    state["has" + itemType.ToString()] = new StateValue(itemType == holding.itemType);
                }
            }
            state["x"] = new StateValue(transform.position.x);
            state["y"] = new StateValue(transform.position.y);
            state["z"] = new StateValue(transform.position.z);
            return state;
        }

        public override void PlanFailed(WorldGoal failedGoal) {
            // If this happens for too long, there is probably a bug in the actions,
            // goals or world setup.
            // TODO: Make sure the world state has changed before running the same
            //       goal again.
            // TODO: Support multiple goals and select the next one.
            thoughtBubble.text = "...";
            // Debug messages are called in the planner
        }

        public override void PlanFound(WorldGoal goal, Queue<ITransition> actions) {
            // Yay we found a plan for our goal!
            Debug.Log("<color=green>Plan found</color> " + GoapAgent.PrettyPrint(actions));
        }

        public override void AboutToDoAction(GoapAction.WithContext action) {
            thoughtBubble.text = action.actionData.name;
        }

        public override void ActionsFinished() {
            // Everything is done, we completed our actions for this gool. Hooray!
            Debug.Log("<color=blue>Actions completed</color>");
            thoughtBubble.text = "Job's Done!";
        }

        public override void PlanAborted(GoapAction.WithContext aborter) {
            // An action bailed out of the plan. State has been reset to plan again.
            // Take note of what happened and make sure if you run the same goal
            // again that it can succeed.
            Debug.Log("<color=red>Plan Aborted</color> " + aborter);
            thoughtBubble.text = "Hmp!";
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
    }
}
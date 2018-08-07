using System.Collections.Generic;
using Ai.AStar;
using Ai.Goap;

namespace RoomEscape {
    public class EscapeDude : Survivor {
        private WorldGoal[] worldGoals;
        private int goalIndex;

        public bool hasEscaped;

        protected override void Awake() {
            base.Awake();
            hasEscaped = false;
            Goal mainGoal = new Goal();
            mainGoal[States.ESCAPED] = new Condition(CompareType.Equal, true);
            Goal secondaryGoal = new Goal();
            secondaryGoal[States.EXPLORED] = new Condition(CompareType.Equal, true);

            WorldGoal mainWorldGoal = new WorldGoal();
            mainWorldGoal[this] = mainGoal;
            WorldGoal secondaryWorldGoal = new WorldGoal();
            secondaryWorldGoal[this] = secondaryGoal;

            worldGoals = new WorldGoal[] {
                mainWorldGoal,
                secondaryWorldGoal
            };

            goalIndex = 0;
        }
        public override WorldGoal CreateGoalState() {
            return worldGoals[goalIndex];
        }

        public override void PlanFailed(WorldGoal failedGoal) {
            base.PlanFailed(failedGoal);
            // Allow switching between several goals
            goalIndex = (goalIndex + 1) % worldGoals.Length;
        }

        public override void PlanFound(WorldGoal goal, Queue<ITransition> actions) {
            base.PlanFound(goal, actions);
            // Once a plan was found we can come back to the main goal
            goalIndex = 0;
        }

        public override State GetState() {
            State state = base.GetState();
            state[States.ESCAPED] = new StateValue(hasEscaped);
            return state;
        }
    }
}
using System.Collections.Generic;
using Ai.AStar;
using Ai.Goap;

namespace RoomEscape {
    public class EscapeDude : Survivor {
        private WorldGoal[] worldGoals;
        private int goalIndex;

        protected override void Awake() {
            base.Awake();
            Goal mainGoal = new Goal();
            mainGoal["escaped"] = new Condition(CompareType.Equal, true);
            Goal secondaryGoal = new Goal();
            secondaryGoal["explore"] = new Condition(CompareType.Equal, true);

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
    }
}
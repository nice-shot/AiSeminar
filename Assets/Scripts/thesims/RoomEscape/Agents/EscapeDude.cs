using System.Collections.Generic;
using Ai.AStar;
using Ai.Goap;

namespace RoomEscape {
    public class EscapeDude : Survivor {
        private int goalIndex;

        public bool hasEscaped;
        public bool escapeRoute;

        protected override void Awake() {
            base.Awake();
            hasEscaped = false;
            escapeRoute = false;
            Goal mainGoal = new Goal();
            mainGoal[States.ESCAPED] = new Condition(CompareType.Equal, true);
            Goal secondaryGoal = new Goal();
            secondaryGoal[States.EXPLORED] = new Condition(CompareType.Equal, true);

            worldGoals = GoalsToWorldGoal(new Goal[] { mainGoal, secondaryGoal });
        }
    }
}
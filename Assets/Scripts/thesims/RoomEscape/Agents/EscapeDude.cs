using System.Collections.Generic;
using Ai.AStar;
using Ai.Goap;

namespace RoomEscape {
    public class EscapeDude : Survivor {

        const int MAX_HP = 4;

        protected override void Awake() {
            base.Awake();
            // Survive
            Goal mainGoal = new Goal();
            mainGoal[States.IN_DANGER] = new Condition(CompareType.Equal, false);
            mainGoal[States.HP] = new Condition(CompareType.MoreThan, 0);
            // Avoid survivors healing non stop
            mainGoal[States.HP] = new Condition(CompareType.LessThanOrEqual, MAX_HP);

            // Escape
            Goal secondaryGoal = new Goal();
            secondaryGoal[States.ESCAPED] = new Condition(CompareType.Equal, true);

            // Explore
            Goal thirdGoal = new Goal();
            thirdGoal[States.EXPLORED] = new Condition(CompareType.Equal, true);

            worldGoals = GoalsToWorldGoal(new Goal[] { mainGoal, secondaryGoal, thirdGoal });
        }
    }
}
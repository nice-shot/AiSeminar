﻿using System.Collections.Generic;
using Ai.AStar;
using Ai.Goap;

namespace RoomEscape {
    public class EscapeDude : Survivor {

        protected override void Awake() {
            base.Awake();
            Goal mainGoal = new Goal();
            mainGoal[States.ESCAPED] = new Condition(CompareType.Equal, true);
            Goal secondaryGoal = new Goal();
            secondaryGoal[States.EXPLORED] = new Condition(CompareType.Equal, true);

            worldGoals = GoalsToWorldGoal(new Goal[] { mainGoal, secondaryGoal });
        }
    }
}
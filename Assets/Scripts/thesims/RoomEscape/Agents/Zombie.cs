using UnityEngine;
using System.Collections;
using Ai.Goap;

namespace RoomEscape {
    public class Zombie : Survivor {
        protected override void Awake() {
            base.Awake();

            Goal mainGoal = new Goal();
            mainGoal[States.EAT_BRAINS] = new Condition(CompareType.Equal, true);
            Goal secondaryGoal = new Goal();
            secondaryGoal[States.WANDER] = new Condition(CompareType.Equal, true);

            worldGoals = GoalsToWorldGoal(new Goal[] { mainGoal, secondaryGoal });
        }
    }
}
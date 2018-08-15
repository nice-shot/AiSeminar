using UnityEngine;
using System.Collections;
using Ai.Goap;

namespace RoomEscape {
    public class HealthSensor : SensorBase {

        [SerializeField] private int hitPoints;


        private readonly State state = new State();

        public override State GetState() {
            state[States.HP] = new StateValue(hitPoints);
            return state;
        }

        public void Damage(int damagePoints) {
            hitPoints -= damagePoints;
        }
    }
}
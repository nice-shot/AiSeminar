using UnityEngine;
using System.Collections.Generic;
using Ai.Goap;

namespace RoomEscape {
    /// <summary>
    /// Calculate danger based on threats
    /// </summary>
    [RequireComponent(typeof(SightSensor))]
    public class DangerSensor : SensorBase {

        private SightSensor sight;
        private HashSet<IStateful> threats = new HashSet<IStateful>();
        //private List<IStateful> threats = new List<IStateful>();
        private readonly State state = new State();

        void Awake() {
            sight = GetComponent<SightSensor>();
        }

        public void CheckThreats() {
            List<IStateful> zombies = sight.GetTargets<Zombie>();
            threats.UnionWith(zombies);
        }

        public List<IStateful> GetThreats() {
            return new List<IStateful>(threats);
        }

        public override State GetState() {
            state[States.IN_DANGER] = new StateValue(threats.Count > 0);
            return state;
        }
    }
}
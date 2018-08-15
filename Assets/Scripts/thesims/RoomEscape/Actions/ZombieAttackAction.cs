using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Ai.Goap;

namespace RoomEscape {
    public class ZombieAttackAction : ActionBase {
        public int power;

        void Awake() {
            AddEffect(States.EAT_BRAINS, ModificationType.Set, true);
            AddTargetPrecondition(States.HP, CompareType.MoreThan, 0);
            AddTargetEffect(States.HP, ModificationType.Add, -1);
        }

        public override List<IStateful> GetAllTargets(GoapAgent agent) {
            ZombieTargetSensor targetSensor = agent.GetComponent<ZombieTargetSensor>();
            if (targetSensor != null) {
                return targetSensor.FindTargets();
            }
            return GetTargets<EscapeDude>();
        }

        public override bool RequiresInRange() {
            return true;
        }

        protected override bool OnDone(GoapAgent agent, WithContext context) {
            base.OnDone(agent, context);

            Component target = context.target as Component;
            HealthSensor targetHealth = target.GetComponent<HealthSensor>();

            if (targetHealth != null) {
                targetHealth.Damage(power);
                return true;
            }

            Debug.LogError("No health sensor for target!");
            return false;
        }
    }
}
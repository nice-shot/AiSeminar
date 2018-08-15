using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Infra.Collections;
using Ai.Goap;


namespace RoomEscape {
    public class RunAwayAction : ActionBase {

        public GameObjectPool wanderPool;
        private Dictionary<GoapAgent, WanderPosition> agentPositions = new Dictionary<GoapAgent, WanderPosition>();

        void Awake() {
            AddEffect(States.IN_DANGER, ModificationType.Set, false);
            AddPrecondition(States.IN_DANGER, CompareType.Equal, true);
        }

        public override List<IStateful> GetAllTargets(GoapAgent agent) {
            if (!agentPositions.ContainsKey(agent)) {
                agentPositions[agent] = wanderPool.Borrow<WanderPosition>();
            }

            WanderPosition wanderPos = agentPositions[agent];

            DangerSensor dangerSensor = agent.GetComponent<DangerSensor>();
            List<IStateful> threats = dangerSensor.GetThreats();

            Vector3 targetPosition = Vector3.zero;

            foreach (IStateful threat in threats) {
                Component threatComp = threat as Component;
                targetPosition = targetPosition + (agent.transform.position - threatComp.transform.position);
            }

            wanderPos.transform.position = (agent.transform.position + targetPosition) * 1;
            
            return new List<IStateful> { wanderPos };
        }


        public override bool RequiresInRange() {
            return true;
        }
    }
}
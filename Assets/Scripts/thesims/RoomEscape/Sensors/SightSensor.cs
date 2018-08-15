﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


using Ai.Goap;

namespace RoomEscape {
    /// <summary>
    /// A sensor used to look around and keep track of agents and interactables
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class SightSensor : SensorBase {

#if DEBUG_PLAN
        [SerializeField] List<string> sightAreas;    
#endif


        [SerializeField] private float visionRadius;
        private List<IStateful> pointsOfInterest = new List<IStateful>();
        private List<Collider> foundColliders = new List<Collider>();

        private NavMeshAgent navAgent;
        private DangerSensor dangerSensor;
        private readonly State state = new State();

        void Awake() {
            navAgent = GetComponent<NavMeshAgent>();
            dangerSensor = GetComponent<DangerSensor>();
        }

        void Start() {
            LookAround();    
        }

        /// <summary>
        /// Search the area in a given radius from our location
        /// </summary>
        public bool LookAround() {
            bool foundNew = false;
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, visionRadius);

            foreach (Collider collider in hitColliders) {
                // Ignore objects we've seen before
                if (foundColliders.Contains(collider)) {
                    continue;
                }

                // Check if object is reachable
                NavMeshPath path = new NavMeshPath();
                navAgent.CalculatePath(collider.transform.position, path);
                if (path.status == NavMeshPathStatus.PathComplete) {
                    // Ignore this object in future searches
                    foundColliders.Add(collider);
                    IStateful pointOfInterest = collider.GetComponent<IStateful>();
                    if (pointOfInterest != null) {
                        pointsOfInterest.Add(pointOfInterest);
                        foundNew = true;
                    }
                }
            }

            // Call the danger sensor in case we saw something scary
            if (foundNew && dangerSensor != null) {
                dangerSensor.CheckThreats();
            }

#if DEBUG_PLAN
            sightAreas.Clear();
            foreach (IStateful poi in pointsOfInterest) {
                Component poiComp = poi as Component;
                sightAreas.Add(poiComp.name);
            }
#endif
            return foundNew;
        }

        /// <summary>
        /// Find objects we've seen with the given type
        /// </summary>
        public List<IStateful> GetTargets<T>() where T: Component, IStateful {
            return pointsOfInterest.FindAll(target => target is T);
        }

        public override State GetState() {
            return state;
        }
    }
}
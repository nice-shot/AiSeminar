using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Ai.Goap;

namespace RoomEscape {
    public class Memory : MonoBehaviour {
        public GameObject startingLocation;

        private List<IStateful> pointsOfInterest = new List<IStateful>();

        void Awake() {
            AddLocationToMemory(startingLocation);    
        }

        // Insert the points of interest in a room to memory
        public void AddLocationToMemory(GameObject location) {
            if (location != null) {
                pointsOfInterest.AddRange(location.GetComponentsInChildren<IStateful>() as IStateful[]);
            }
        }

        // Find only targets we've seen before
        public List<IStateful> GetTargets<T>() where T: Component, IStateful {
            return pointsOfInterest.FindAll(target => target is T);
        }
    }
}
using UnityEngine;
using System.Collections;

using Ai.Goap;

namespace RoomEscape {
    public abstract class SensorBase : MonoBehaviour {
        abstract public State GetState();
    }
}
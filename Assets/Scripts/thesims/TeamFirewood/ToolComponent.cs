using UnityEngine;

namespace TeamFirewood {
/// <summary>
/// A tool used for mining ore and chopping wood.
/// Tools have strength that gets used up each time they are used. When their
/// strength is depleted they should be destroyed by the user.
/// </summary>
public class ToolComponent : MonoBehaviour {
    [Range(0f, 1f)]
    [SerializeField] float strength;

    public bool IsDestroyed {
        get {
            return strength <= 0f;
        }
    }

    protected void Start() {
        strength = 1f;
    }

    /// <summary>
    /// Use up the tool by causing damage. 
    /// </summary>
    /// <param name="damage">Should be a percent from 0 to 1, where 1 is 100%.</param>
    public void Use(float damage) {
        strength -= damage;
    }
}
}

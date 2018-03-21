﻿using System.Collections.Generic;
using UnityEngine;

namespace AI.Goap {
public abstract class GoapAction : MonoBehaviour {


    private HashSet<KeyValuePair<string,object>> preconditions = new HashSet<KeyValuePair<string, object>>();
    private HashSet<KeyValuePair<string,object>> effects = new HashSet<KeyValuePair<string, object>>();

    private bool inRange = false;

    /* The cost of performing the action. 
     * Figure out a weight that suits the action. 
     * Changing it will affect what actions are chosen during planning.*/
    public float cost = 1f;

    /**
     * An action often has to perform on an object. This is that object. Can be null. */
    public GameObject target;

    public void DoReset() {
        inRange = false;
        target = null;
        Reset ();
    }

    /**
     * Reset any variables that need to be reset before planning happens again.
     */
    public abstract void Reset();

    /**
     * Is the action done?
     */
    public abstract bool IsDone();

    /**
     * Procedurally check if this action can run. Not all actions
     * will need this, but some might.
     */
    public abstract bool CheckProceduralPrecondition(GameObject agent);

    /**
     * Run the action.
     * Returns True if the action performed successfully or false
     * if something happened and it can no longer perform. In this case
     * the action queue should clear out and the goal cannot be reached.
     */
    public abstract bool Perform(GameObject agent);

    /**
     * Does this action need to be within range of a target game object?
     * If not then the moveTo state will not need to run for this action.
     */
    public abstract bool RequiresInRange ();

    /**
     * Are we in range of the target?
     * The MoveTo state will set this and it gets reset each time this action is performed.
     */
    public bool IsInRange () {
        return inRange;
    }

    public void SetInRange(bool inRange) {
        this.inRange = inRange;
    }


    public void AddPrecondition(string key, object value) {
        preconditions.Add (new KeyValuePair<string, object>(key, value) );
    }


    public void RemovePrecondition(string key) {
        KeyValuePair<string, object> remove = default(KeyValuePair<string,object>);
        foreach (KeyValuePair<string, object> kvp in preconditions) {
            if (kvp.Key.Equals (key)) 
                remove = kvp;
        }
        if ( !default(KeyValuePair<string,object>).Equals(remove) )
            preconditions.Remove (remove);
    }


    public void AddEffect(string key, object value) {
        effects.Add (new KeyValuePair<string, object>(key, value) );
    }


    public void RemoveEffect(string key) {
        KeyValuePair<string, object> remove = default(KeyValuePair<string,object>);
        foreach (KeyValuePair<string, object> kvp in effects) {
            if (kvp.Key.Equals (key)) 
                remove = kvp;
        }
        if ( !default(KeyValuePair<string,object>).Equals(remove) )
            effects.Remove (remove);
    }


    public HashSet<KeyValuePair<string, object>> GetPreconditions() {
        return preconditions;
    }

    public HashSet<KeyValuePair<string, object>> GetEffects() {
        return effects;
    }

}
}

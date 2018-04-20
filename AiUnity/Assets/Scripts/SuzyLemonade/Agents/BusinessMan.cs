using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SuzyLemonade {
public class BusinessMan : Person {
    /**
     * My current goal is to buy lemonade!
     */
    public override HashSet<KeyValuePair<string, object>> CreateGoalState() {
        HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>>();
        goal.Add(new KeyValuePair<string, object>("buyLemonade", true));
        return goal;
    }
}
}
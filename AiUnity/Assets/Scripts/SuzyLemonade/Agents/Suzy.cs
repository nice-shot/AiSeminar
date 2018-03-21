using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuzyLemonade {
public class Suzy : Person {
    /**
     * My current goal is to create lemonade!
     */
    public override HashSet<KeyValuePair<string, object>> CreateGoalState() {
        HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>>();
        goal.Add(new KeyValuePair<string, object>("makeLemonade", true));
        return goal;
    }
}
}
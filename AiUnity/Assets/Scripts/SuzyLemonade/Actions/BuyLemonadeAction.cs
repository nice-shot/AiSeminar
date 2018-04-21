using UnityEngine;
using System.Collections;

using AI.Goap;

namespace SuzyLemonade {
public class BuyLemonadeAction : SmartAction {
    void Awake ()
    {
        AddPrecondition ("hasMoney", true);
        AddPrecondition ("inFrontOfLine", true);
        AddEffect ("buyLemonade", true);
        AddEffect ("inFrontOfLine", false);
        AddEffect ("hasMoney", false);
    }

    protected override void SmartReset () {
    }

    protected override bool SmartPerform (GameObject agent) {
        isDone = true;
        return true;
    }

    public override bool RequiresInRange () {
        return false;
    }

    public override bool CheckProceduralPrecondition (GameObject agent) {
        return true;
    }

}
}
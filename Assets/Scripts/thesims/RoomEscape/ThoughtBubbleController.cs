using UnityEngine;
using System.Collections;

using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ThoughtBubbleController : MonoBehaviour {
    public GameObject agent;
    public float yChange;
    public float xChange;
    public float extraMsgTimeout;


    private Text text;
    private string actionMsg = "";
    private string extraMsg = "";
    private bool extraMsgPositive = true;
    private float actionMsgStartTime;
    private float extraMsgStartTime;

    private const string MSG_FORMAT = 
    "<color=black>{0}</color>\n"
    + "<color={1}>{2}</color>";

    void Awake() {
        text = GetComponent<Text>();
    }

    void Update() {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(agent.transform.position);
        screenPos.x += xChange;
        screenPos.y += yChange;
        transform.position = screenPos;
        if (extraMsg != "" && Time.time - extraMsgStartTime > extraMsgTimeout) {
            extraMsg = "";
            DisplayText();
        }
    }

    void DisplayText() {
        string extraMsgColor = extraMsgPositive ? "green" : "red";
        text.text = string.Format(MSG_FORMAT, actionMsg, extraMsgColor, extraMsg);
    }

    public void SetActionText(string msg) {
        actionMsg = msg;
        actionMsgStartTime = Time.time;
        DisplayText();
    }

    public void SetExtraText(string msg, bool positive = true) {
        extraMsg = msg;
        extraMsgPositive = positive;
        extraMsgStartTime = Time.time;
        DisplayText();
    }
}

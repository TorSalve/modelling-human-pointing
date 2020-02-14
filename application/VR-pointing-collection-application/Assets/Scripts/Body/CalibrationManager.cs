using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using TMPro;

public class CalibrationManager: MonoBehaviour {

    public LogManager logManager;
    public Participant participant;
    public OptiTrackState otState;

    [Header("Override hand size")]
    [Range(1,3)]
    public float scaleHand = -1;

    [Header("Hand references")]
    public GameObject goHandCenter;
    public GameObject goIndexTip;
    public GameObject goHand;

    [Header("Labels")]
    public GameObject calibrationMessage;
    public GameObject continueMessage;

    [Header("Controller input setting")]
    public SteamVR_Input_Sources controllerHand;
    public SteamVR_Action_Boolean calibrationAction;
    private bool done = false;
    private enum CalibrationState { Start, Down, Side, Front, End }
    private CalibrationState state = CalibrationState.Start;
    private TextMeshPro calibrationMessageText;

    void Start() {
        calibrationMessageText = calibrationMessage.GetComponent<TextMeshPro>();
        calibrationAction.AddOnStateDownListener(OnCalibrationClick, controllerHand);
    }

    public bool IsDone() { return done; }

    void OnCalibrationClick(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        if(IsDone() || !otState.IsReady()) { return; }
        Debug.Log(state);
        switch (state) {
            case CalibrationState.Start:
                calibrationMessageText.text = "Point straight down at the floor";
                state = CalibrationState.Down;
                break;
            case CalibrationState.Down:
                logManager.LogCalibration();
                participant.SetHandIndexDist(otState.goHand.transform.position, otState.goIndexfinger.transform.position);
                // SetHandSize();
                calibrationMessageText.text = "Point straight to the right";
                state = CalibrationState.Side;
                break;
            case CalibrationState.Side:
                logManager.LogCalibration();
                calibrationMessageText.text = "Point straight in front of you";
                state = CalibrationState.Front;
                break;
            case CalibrationState.Front:
                logManager.LogCalibration();
                calibrationMessageText.text = "You are now done calibrating";
                state = CalibrationState.End;
                break;
            case CalibrationState.End:
                calibrationMessage.SetActive(false);
                continueMessage.SetActive(true);
                done = true;
                break;
        }
    }

    void SetHandSize() {
        float virtualHandIndexDist = Vector3.Distance(
            goHandCenter.transform.position,
            goIndexTip.transform.position
        ) / goHand.transform.localScale.x;
        float realHandIndexDist = participant.handIndexDist;
        Vector3 scale = Vector3.one * (realHandIndexDist/virtualHandIndexDist);
        goHand.transform.localScale = scale;
        Debug.Log(scale);
    }

    void Update() {
        if(scaleHand > 0f) { ScaleHand(scaleHand); }
    }

    void ScaleHand(float scale) {
        if(goHand.transform.localScale.x == scale) { return; }
        Vector3 scaleVector = Vector3.one * scale;
        goHand.transform.localScale = scaleVector;
        // Debug.Log(scale);
    }



}
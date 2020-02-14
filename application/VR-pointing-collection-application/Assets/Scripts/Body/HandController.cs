using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class HandController : MonoBehaviour
{

    public OptiTrackState otState;
    public FingerRig fingerRig;
    public GameObject hand;
    public int indexFingerIndex = 0;

    private bool runOnce = true;

    // Start is called before the first frame update
    void Start() {
        fingerRig.fingers[indexFingerIndex].weight = 1f;
        fingerRig.fingers[indexFingerIndex].rotationWeight = 1f;       
    }

    // Update is called once per frame
    void Update() {

        if(otState.IsReady() || true) {
            if(runOnce) {
                fingerRig.fingers[indexFingerIndex].target = otState.goIndexfinger.transform;
                runOnce = false;
            }
            hand.transform.position = otState.goHand.transform.position;
            hand.transform.rotation = otState.goHand.transform.rotation;
        }

    }
}

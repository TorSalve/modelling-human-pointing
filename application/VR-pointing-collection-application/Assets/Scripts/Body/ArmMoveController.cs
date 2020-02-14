using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class ArmMoveController : MonoBehaviour
{

    [Header("Avatar")]
    public GameObject arm;
    private CCDIK ccdikArm;
    public SkinnedMeshRenderer armBounds;
    public FingerRig hand;

    [Header("References")]
    public OptiTrackState otState;
    public Participant participant;
    public int indexFingerIndex = 1;
    private bool runOnce = true;
    private float realWorldScale;


    void Start()
    {
        ccdikArm = arm.GetComponent(typeof(CCDIK)) as CCDIK;
        ccdikArm.solver.IKPositionWeight = 1f;

        hand.fingers[indexFingerIndex].weight = 1f;
        hand.fingers[indexFingerIndex].rotationWeight = 1f;

    }

    void Update() {
        if(otState.IsReady()) {
            if(runOnce) {
                float virtualSize = armBounds.bounds.extents.magnitude * 2;
                float armLength = participant.upperarmLength + participant.forearmLength + participant.fingerLength + 7;
                float realSize = armLength / 100;
                realWorldScale = virtualSize / realSize;
                
                Vector3 scaleVector = new Vector3(realWorldScale, realWorldScale, realWorldScale);
                arm.transform.localScale = scaleVector;

                GameObject.Find("__env").transform.localScale = scaleVector;

                ccdikArm.solver.target = otState.goHand.transform;
                hand.fingers[indexFingerIndex].target = otState.goIndexfinger.transform;
                runOnce = false;
            }

            arm.transform.position = otState.goUpperarm.transform.position;
            arm.transform.rotation = otState.goHand.transform.rotation;
        }

    }

    public Vector3 FromRealToVirtualWorld(Vector3 v) { return v * realWorldScale; }
    public float FromRealToVirtualWorld(float f) { return f * realWorldScale; }
    public Vector3 FromVirtualToRealWorld(Vector3 v) { return v / realWorldScale; }
    public float FromVirtualToRealWorld(float f) { return f / realWorldScale; }
}
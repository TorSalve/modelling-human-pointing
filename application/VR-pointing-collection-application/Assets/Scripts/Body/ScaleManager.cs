using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ScaleManager : MonoBehaviour
{
    
    [Header("Compute scale")]
    public Participant participant;
    public SkinnedMeshRenderer avatarMeshRenderer;
    public OptiTrackState otState;

    [Header("Scale arm")]
    public Transform shoulder;
    public Transform elbow;
    public Transform hand;
    public Transform indexRoot;
    public Transform indexTip;

    [Header("Add scale")]
    public GameObject[] toBeScaled;
    public float scale;
    public float realWorldScale;

    private bool runOnce = true;
    private bool ready = false;

    // Update is called once per frame
    void Update() {
        if(runOnce && otState.IsReady()) {
            Calibrate();   
            runOnce = false;
        }
        
        if(Input.GetKeyDown("c") && otState.IsReady()) {
            Calibrate();
            Debug.Log("c pressed");
        }

        realWorldScale = 2f;
    }

    void Calibrate() {
        if(otState.IsReady()) {
            Debug.Log("Calibration");

            // 1m = 2units

            float currentUpperarmLength = Vector3.Distance(shoulder.position, elbow.position);
            float currentForearmLength = Vector3.Distance(elbow.position, hand.position);
            float currentIndexLength = Vector3.Distance(indexRoot.position, indexTip.position);

            float scaleUpperarm = FromRealToVirtualWorld(participant.upperarmLength) / currentUpperarmLength / 2;
            float scaleForearm = FromRealToVirtualWorld(participant.forearmLength) / currentForearmLength / 2;
            float scaleIndex = FromRealToVirtualWorld(participant.fingerLength) / currentIndexLength / 2;

            // shoulder.localScale = new Vector3(scaleUpperarm, 1, 1);
            // elbow.localScale = new Vector3(scaleForearm, 1, 1);
            // indexRoot.localScale = new Vector3(scaleIndex, 1, 1);
            
            float participantHeight = FromRealToVirtualWorld(participant.height);
            float avatarHeight = avatarMeshRenderer.bounds.extents.y * 2;
            scale = participantHeight / avatarHeight;

            Vector3 scaleVector = new Vector3(scale, scale, scale);
            foreach (GameObject go in toBeScaled) {
                // go.transform.localScale = scaleVector;
            }

            ready = true;
        }
    }

    public bool IsReady() { return ready; }

    public Vector3 FromRealToVirtualWorld(Vector3 v) { return (v * realWorldScale)/100; }
    public float FromRealToVirtualWorld(float f) { return (f * realWorldScale)/100; }
    public Vector3 FromVirtualToRealWorld(Vector3 v) { return (v / realWorldScale) * 100; }
    public float FromVirtualToRealWorld(float f) { return (f / realWorldScale) * 100; }

}

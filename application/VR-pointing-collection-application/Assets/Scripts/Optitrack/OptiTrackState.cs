using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptiTrackState : MonoBehaviour
{
    public GameObject rigidBodyParent;
    public Participant participant;

    [Header("OptiTrack Settings")]
    public OptitrackStreamingClient StreamingClient;
    public int ShoulderId1;
    public int ShoulderId2;
    public int UpperArmId;
    public int ForearmId;
    public int HandId;
    public int IndexFingerId;
    public int HmdId;

    [Header("Debug")]
    public bool showOptiTrackRigidBodies = false;
    public bool drawRotation = false;

    private OptitrackRigidBodyState rbState_shoulder1;
    private OptitrackRigidBodyState rbState_shoulder2;
    private OptitrackRigidBodyState rbState_forearm;
    private OptitrackRigidBodyState rbState_hand;
    private OptitrackRigidBodyState rbState_indexfinger;
    public OptitrackRigidBodyState rbState_hmd;
    private OptitrackRigidBodyState rbState_upperarm;

    [HideInInspector]
    public GameObject goShoulder1;
    [HideInInspector]
    public GameObject goShoulder2;
    [HideInInspector]
    public GameObject goForearm;
    [HideInInspector]
    public GameObject goHand;
    [HideInInspector]
    public GameObject goIndexfinger;
    [HideInInspector]
    public GameObject goHmd;
    [HideInInspector]
    public GameObject goUpperarm;

    private bool runOnce = true;
    private bool ready = false;


    // Start is called before the first frame update
    void Start()
    {
        if (StreamingClient == null) {
            StreamingClient = OptitrackStreamingClient.FindDefaultClient();
            // If we still couldn't find one, disable this component.
            if (StreamingClient == null) {
                Debug.LogError(GetType().FullName + ": Streaming client not set, and no " + typeof(OptitrackStreamingClient).FullName + " components found in scene; disabling this component.", this);
                this.enabled = false;
                return;
            }
        }
        InitGO("shoulder1", ref goShoulder1);
        InitGO("shoulder2", ref goShoulder2);
        InitGO("forearm", ref goForearm);
        InitGO("hand", ref goHand);
        InitGO("indexfinger", ref goIndexfinger);
        InitGO("hmd", ref goHmd);
        InitGO("upperarm", ref goUpperarm);
    }

    // Update is called once per frame
    void Update()
    {
        List<bool> readies = new List<bool>();

        UpdateGO(ShoulderId1, goShoulder1, ref rbState_shoulder1, ref readies);
        UpdateGO(ShoulderId2, goShoulder2, ref rbState_shoulder2, ref readies);
        UpdateGO(ForearmId, goForearm, ref rbState_forearm, ref readies);
        UpdateGO(HandId, goHand, ref rbState_hand, ref readies);
        UpdateGO(IndexFingerId, goIndexfinger, ref rbState_indexfinger, ref readies);
        UpdateGO(HmdId, goHmd, ref rbState_hmd, ref readies);
        UpdateGO(UpperArmId, goUpperarm, ref rbState_upperarm, ref readies);
        
        if(rbState_hmd != null && !runOnce) {
            Vector3 camera = GameObject.Find("[CameraRig]").transform.position;
            GameObject.Find("[CameraRig]").transform.position = new Vector3(
                rbState_hmd.Pose.Position.x, camera.y, rbState_hmd.Pose.Position.z
            );
            runOnce = false;
        }
        
        ready = !readies.Contains(false);
    }

    void InitGO(string name, ref GameObject go) {
        if(showOptiTrackRigidBodies) {
            go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        } else {
            go = new GameObject();
        }
        go.transform.parent = rigidBodyParent.transform;
        go.name = name;
    }

    void UpdateGO(int sname, GameObject go, ref OptitrackRigidBodyState otRBState, ref List<bool> readies) {        
        float s = 0.05f;
        otRBState = StreamingClient.GetLatestRigidBodyState(sname);
        readies.Add(otRBState != null);
        go.transform.localScale = new Vector3(s, s, s);
        if(otRBState != null) {
            go.transform.position = otRBState.Pose.Position;
            go.transform.rotation = otRBState.Pose.Orientation;
            if(drawRotation) {
                Debug.DrawRay(go.transform.position, go.transform.TransformDirection(Vector3.forward), Color.blue);
                Debug.DrawRay(go.transform.position, go.transform.TransformDirection(Vector3.up), Color.green);
                Debug.DrawRay(go.transform.position, go.transform.TransformDirection(Vector3.right), Color.red);
            }
        }
    }

    public bool IsReady() { return ready; }
}

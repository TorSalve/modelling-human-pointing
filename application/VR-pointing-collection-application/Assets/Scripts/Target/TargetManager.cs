using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Valve.VR;

public class TargetManager : MonoBehaviour
{

    public LogManager logManager;
    public CalibrationManager calibrationManager;

    [Header("Labels")]
    public GameObject continueMessage;
    public GameObject stopMessage;
    public enum TargetLayout { Grid, FixedHeight, Random };

    [Header("Repetition settings")]
    [Tooltip("How many times should a participant point at each target")]
    public int numberOfRepetitions = 3;
    private List<int> repetitionOrder = new List<int>();

    [Header("Target Layout")]
    public TargetLayout targetLayout;
    [DrawIf("targetLayout", TargetLayout.Grid, ComparisonType.Equals)]
    [Tooltip("what NxNxN the grid should be")]
    public int gridSize = 3;
    [DrawIf("targetLayout", TargetLayout.Grid, ComparisonType.Equals)]
    [Tooltip("what dimensions should the grid have")]
    public Vector3 gridDimensions;
    [DrawIf("targetLayout", TargetLayout.Grid, ComparisonType.Equals)]
    [Tooltip("Grid offset")]
    public Vector3 gridOffset;
    [DrawIf("targetLayout", TargetLayout.FixedHeight, ComparisonType.Equals)]
    [Tooltip("the optitrack state")]
    public OptiTrackState otState;
    [DrawIf("targetLayout", TargetLayout.FixedHeight, ComparisonType.Equals)]
    [Tooltip("number of targets to spawn")]
    public int numberOfTargets;
    [DrawIf("targetLayout", TargetLayout.FixedHeight, ComparisonType.Equals)]
    [Tooltip("Grid offset")]
    public Vector3 fixedHeightOffset;
    [DrawIf("targetLayout", TargetLayout.Random, ComparisonType.Equals)]
    public CreateSample sample;
    [DrawIf("targetLayout", TargetLayout.Random, ComparisonType.Equals)]
    [Tooltip("Grid offset")]
    public Vector3 randomOffset;

    [Header("Target Settings")]
    public GameObject targetParent;
    [Tooltip("If false, do not display all targets that the poisson sampler creates.")]
    public bool onlyShowActiveTarget;
    [Tooltip("Prefab to use for the generated targets.")]
    public GameObject prefab;
    [Tooltip("Color assigned to inactive targets.")]
    public Color defaultColor;
    [Tooltip("Color assigned to active targets.")]
    public Color activeColor;
    [Tooltip("Default alpha for inactive targets. 0 is transparent, 1 is opaque.")]
    [Range(0, 1)]
    public float alpha = 0.5f;
    [Tooltip("Diameter of the targets in cm. Assumes sphericity.")]
    public float diameter = 0.15f;
    private List<GameObject> gameObjects = new List<GameObject>();
    private Dictionary<GameObject, GameObject> targets = new Dictionary<GameObject, GameObject>();
    private bool repetitionIsRunning;
    public GameObject activeTarget { get; private set; }

    [Header("Controller input setting")]
    public SteamVR_Input_Sources controllerHand;
    public SteamVR_Action_Boolean startAction;
    public SteamVR_Action_Boolean stopAction;

    public GameObject SpawnTarget(Vector3 position) {
        GameObject target = Instantiate(prefab, transform);
        target.SetActive(!onlyShowActiveTarget);
        target.transform.localPosition = position;
        target.name = "Target " + gameObjects.Count;
        target.transform.localScale = Vector3.one * diameter;
        target.transform.parent = targetParent.transform;
        SetColor(target.GetComponent<Renderer>(), defaultColor, alpha);
        gameObjects.Add(target);
        return target;
    }

    public void SetNewTarget() {
        int index = repetitionOrder.First();
        activeTarget = gameObjects[index];
        Debug.LogFormat("target no. {0}", index);
        activeTarget.SetActive(true);
        SetColor(activeTarget.GetComponent<Renderer>(), activeColor, 1.0f);
        repetitionOrder.RemoveAt(0);
    }

    public void SetColor(Renderer renderer, Color color, float alpha) {
        Color newColor = color;
        newColor.a = alpha; // NB - target shader changed to opaque.
        renderer.material.color = newColor;
    }

    void OnAllTargetsGenerated() {
        SetNewTarget();
        logManager.Initialize(GetTargetPosition());
        logManager.StartSnapshotting();
    }

    void OnSamplingFinished() {
        OnAllTargetsGenerated();
    }

    GameObject OnNewPointAdded(Vector3 p) {
        return SpawnTarget(p);
    }

    Vector3 GetTargetPosition() {
        Vector3 activeTargetPos = Vector3.zero;
        switch( targetLayout) {
            case TargetLayout.FixedHeight:
                activeTargetPos = activeTarget.transform.position + fixedHeightOffset;
                break;
            case TargetLayout.Grid:
                activeTargetPos = activeTarget.transform.position + gridOffset;
                break;
            case TargetLayout.Random:
                activeTargetPos = activeTarget.transform.position + randomOffset;
                break;
        }
        return activeTargetPos;
    }

    void StartRepetition() {
        if(repetitionOrder.Count == 0) { return; }
        if(repetitionIsRunning) { StopRepetition(); }
        repetitionIsRunning = true;
        switch (targetLayout) {
            case TargetLayout.FixedHeight:
                if(otState.rbState_hmd != null) {
                    // float height = optiTrackState.hmd.y;
                    float[] linspace = LinSpace(1f, 4f, numberOfTargets);
                    Vector3 otPosition = otState.goHmd.transform.localPosition;
                    foreach (var l in linspace) {
                        Vector3 p = new Vector3(l, otPosition.y, otPosition.z) + fixedHeightOffset;
                        SpawnTarget(p);
                    }
                    // logManager.disableLogging = false;
                    OnAllTargetsGenerated();
                }
                break;
            case TargetLayout.Grid:
                float[] linspace_x = LinSpace(0f, gridDimensions.x, gridSize);
                float[] linspace_y = LinSpace(0f, gridDimensions.y, gridSize);
                float[] linspace_z = LinSpace(0f, gridDimensions.z, gridSize);

                foreach (var lx in linspace_x) {
                    foreach (var ly in linspace_y) {
                        foreach (var lz in linspace_z) {
                            Vector3 p = new Vector3(lx, ly, lz);
                            SpawnTarget(p);
                        }
                    }   
                }
                OnAllTargetsGenerated();
                targetParent.transform.position += gridOffset;
                break;
            case TargetLayout.Random:
                // fires onSamplingFinished when ready
                targetParent.transform.position += randomOffset;
                sample.Initialize();
                break;
        }
    }

    void StopRepetition() {
        logManager.StopSnapshotting();
        logManager.LogCollection();
        logManager.Cleanup();
        ClearTargets();
        repetitionIsRunning = false;
        targetParent.transform.position = Vector3.zero;
    }

    void ClearTargets() {
        sample.ClearTargets();
        gameObjects = new List<GameObject>();
        targets = new Dictionary<GameObject, GameObject>();
    }
    
    void Start() {
        switch (targetLayout) {
            case TargetLayout.FixedHeight:
                // logManager.disableLogging = true;
                targetParent.transform.position += new Vector3(1f, 0f, 0f);
                break;
            case TargetLayout.Grid:
                targetParent.transform.position += Vector3.zero;
                int totalNumberOfTargets = gridSize * gridSize * gridSize;
                repetitionOrder = Enumerable.Range(0, totalNumberOfTargets)
                    .Select(x => Enumerable.Repeat(x, numberOfRepetitions).ToList())
                    .SelectMany(x => x)
                    .OrderBy(i => Random.value)
                    .ToList();
                break;
            case TargetLayout.Random:
                sample.newPointAdded = p => OnNewPointAdded(p);
                sample.samplingFinished = i => OnSamplingFinished();
                break;
        }
        startAction.AddOnStateDownListener(OnStartClick, controllerHand);
        startAction.AddOnStateUpListener(OnStartClick, controllerHand);
        stopAction.AddOnStateDownListener(OnStopClick, controllerHand);
    }

    // start a repetition
    void OnStartClick(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        if(!calibrationManager.IsDone()) { return; }
        continueMessage.SetActive(false);
        if(!repetitionIsRunning) {
            StartRepetition();
        }
    }

    // select target and stop a repetition
    void OnStopClick(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        if(!calibrationManager.IsDone()) { return; }
        StopRepetition();
        if(repetitionOrder.Count() > 0) {
            continueMessage.SetActive(true);
        } else {
            stopMessage.SetActive(true);
        }
        Debug.LogFormat("Repetitions to go: {0}", repetitionOrder.Count());

    }

    private float[] LinSpace (float min, float max, int partitions) {
        float[] result = new float[partitions];
        float step = (max - min) / (partitions - 1);
        for (int i = 0; i < partitions; i++) {
            result[i] = min + step * i;
        }
        return result;
    }
}
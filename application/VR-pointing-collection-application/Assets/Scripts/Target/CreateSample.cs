using UnityEngine;
using System.Collections;
using System;

public class CreateSample: MonoBehaviour
{

    public PoissonDiskSampler sampler;

    public float regionWidth = 10f;
    public float regionHeight = 10f;
    public float regionLength = 10f;
    public float minDistance = 0.5f;
    public float rejectionLimit = 30;
    public bool isSpherical;
    public bool drawGizmos;

    public int Dimensions;

    public GameObject pointsholder;

    public Func<Vector3, GameObject> newPointAdded;
    public Action<int> samplingFinished;

    void OnEnable() {
        sampler.NewPointAdded += OnNewPointAdded;
        sampler.SamplingFinished += OnSamplingFinished;
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    public void Initialize() {
        sampler.regionWidth = regionWidth;
        sampler.regionHeight = regionHeight;
        sampler.regionLength = regionLength;
        sampler.minDistance = minDistance;
        sampler.rejectionLimit = rejectionLimit;
        sampler.isSpherical = isSpherical;

        sampler.StartSampling(Dimensions);
    }

    void OnDisable() {
        sampler.NewPointAdded -= OnNewPointAdded;
        sampler.SamplingFinished -= OnSamplingFinished;
    }

    void OnNewPointAdded(Vector3 p) {
        if(p == null) { return; }
        GameObject go = newPointAdded(p);
        go.transform.Translate(pointsholder.transform.position);
        go.transform.parent = pointsholder.transform;
    }

    void OnSamplingFinished() {
        samplingFinished(1);
    }

    public void ClearTargets() {
        int childs = pointsholder.transform.childCount;
        for (int i = childs - 1; i >= 0; i--) {
            Destroy(pointsholder.transform.GetChild(i).gameObject);
        }
    }

    void OnDrawGizmos() {
        if (!Application.isPlaying || !drawGizmos)
            return;

        Gizmos.color = Color.green;
        if (isSpherical) {
            Vector3 center = new Vector3(regionWidth / 2f, regionHeight / 2f, regionLength / 2f);
            Gizmos.DrawWireSphere(center, regionWidth / 2f);
        } else {
            Gizmos.DrawLine(new Vector3(0, 0, 0), new Vector3(0, regionHeight, 0));
            Gizmos.DrawLine(new Vector3(0, 0, 0), new Vector3(regionWidth, 0, 0));
            Gizmos.DrawLine(new Vector3(0, 0, 0), new Vector3(0, 0, regionLength));
            Gizmos.DrawLine(new Vector3(regionWidth, regionHeight, regionLength), new Vector3(0, regionHeight, regionLength));
            Gizmos.DrawLine(new Vector3(regionWidth, regionHeight, regionLength), new Vector3(regionWidth, 0, regionLength));
            Gizmos.DrawLine(new Vector3(regionWidth, regionHeight, regionLength), new Vector3(regionWidth, regionHeight, 0));
        }

    }

}

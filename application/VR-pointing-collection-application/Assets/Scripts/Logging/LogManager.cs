using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using CsvHelper;
using System.Linq;
using CsvHelper.Configuration;

public class LogManager : MonoBehaviour
{

    [Header("Participant information")]
    public Participant participant;

    [Header("Logging")]
    [Tooltip("At which rate to log (sec)")]
    public float logRate = 0.02f;

    [Header("OptiTrack Settings")]
    public OptiTrackState otState;

    [Header("Debug")]

    [Tooltip("The save path for log files")]
    public string path = "Assets/Resources/Log";

    [Tooltip("Disables logging")]
    public bool disableLogging = false;
    private string logPath;
    public Collection collection;
    private List<Snapshot> snapshots = new List<Snapshot>{};    
    private IEnumerator snapshotCoroutine;

    void Start() {
        if(disableLogging) { return; }
        snapshotCoroutine = CreateSnapshots();
        logPath = string.Format("{0}/{1}", path, participant.id);
        // create dir if not exists
        Directory.CreateDirectory(logPath);

        Debug.LogFormat("Logging in {0}", logPath);
    }

    public void Initialize(Vector3 activeTargetPosition) {
        if(disableLogging) { return; }
        InitCollection(activeTargetPosition);
    }

    private Snapshot AddSnapshot(bool addSnapshot = true) {
        Snapshot snapshot = new Snapshot() {
            timestamp = Time.time,
            indexFinger = Position.ToPosition(otState.goIndexfinger.transform.localPosition),
            indexFingerO = Position.ToPosition(otState.goIndexfinger.transform.rotation),
            hand = Position.ToPosition(otState.goHand.transform.localPosition),
            handO = Position.ToPosition(otState.goHand.transform.rotation),
            lowerArm = Position.ToPosition(otState.goForearm.transform.localPosition),
            lowerArmO = Position.ToPosition(otState.goForearm.transform.rotation),
            upperArm = Position.ToPosition(otState.goUpperarm.transform.localPosition),
            upperArmO = Position.ToPosition(otState.goUpperarm.transform.rotation),
            rightShoulder = Position.ToPosition(otState.goShoulder1.transform.localPosition),
            rightShoulderO = Position.ToPosition(otState.goShoulder1.transform.rotation),
            leftShoulder = Position.ToPosition(otState.goShoulder2.transform.localPosition),
            leftShoulderO = Position.ToPosition(otState.goShoulder2.transform.rotation),
            hmd = Position.ToPosition(otState.goHmd.transform.localPosition),
            hmdO = Position.ToPosition(otState.goHmd.transform.rotation),
        };
        if(addSnapshot) { snapshots.Add(snapshot); }
        return snapshot;
    }

    private IEnumerator CreateSnapshots() {
        while(true) {
            AddSnapshot();
            yield return new WaitForSeconds(logRate);
        }
    }

    public void InitCollection(Vector3 activeTarget) {
        if(disableLogging) { return; }
        int collectionNumber = 0;
        string p = string.Format("{0}/collections.csv", logPath);
        if(File.Exists(p)) {
            using (var reader = new StreamReader(p))
            using (var csv = new CsvReader(reader)) {
                csv.Configuration.RegisterClassMap<CollectionMap>();
                List<Collection> items = csv.GetRecords<Collection>().ToList();
                collectionNumber = items.Max(x => x.id) + 1;
            }
        }

        collection = new Collection() {
            id = collectionNumber,
            activeTargetPosition = Position.ToPosition(activeTarget)
        };
    }

    // build a collection and send it here for saving. One collection for each repetition
    public void LogCollection() {
        if(disableLogging || collection == null) { return; }
        string p = string.Format("{0}/collection_{1}.csv", logPath, collection.id);
        using (var writer = new StreamWriter(p))
        using (var csv = new CsvWriter(writer)) {
            csv.Configuration.RegisterClassMap<SnapshotMap>();
            csv.WriteRecords(snapshots);
        }

        p = string.Format("{0}/collections.csv", logPath);
        AppendNewCsvLine<Collection, CollectionMap>(collection, p);
        Debug.LogFormat("collection_{0} (CSV) logged", collection.id);
    }

    public void LogCalibration() {
        if(disableLogging) { return; }
        Directory.CreateDirectory(logPath);
        string p = string.Format("{0}/calibration.csv", logPath);
        Snapshot snapshot = AddSnapshot(false);
        AppendNewCsvLine<Snapshot, SnapshotMap>(snapshot, p);
    }

    public void LogParticipant() {
        if(disableLogging) { return; }
        Directory.CreateDirectory(path);
        string p = string.Format("{0}/participants.csv", path);
        LogParticipant logParticipant = (new LogParticipant()).fromParticipant(participant);
        AppendNewCsvLine<LogParticipant, LogParticipantMap>(logParticipant, p);
        Debug.LogFormat("participant {0} logged", participant.id);
    }

    void AppendNewCsvLine<T, M>(T item, string p) where M: ClassMap<T> {
        List<T> items = new List<T> {};
        if(File.Exists(p)) {
            using (var reader = new StreamReader(p))
            using (var csv = new CsvReader(reader)) {    
                csv.Configuration.RegisterClassMap<M>();
                items = csv.GetRecords<T>().ToList();
            }
        }
        items.Add(item);
        using (var writer = new StreamWriter(p))
        using (var csv = new CsvWriter(writer)) {    
            csv.Configuration.RegisterClassMap<M>();
            csv.WriteRecords(items);
        }
    }

    public void StartSnapshotting() {
        if(disableLogging) { return; }
        StartCoroutine(snapshotCoroutine);
    }

    public void StopSnapshotting() {
        if(disableLogging) { return; }
        StopCoroutine(snapshotCoroutine);
        AddSnapshot();
    }

    public void Cleanup() {
        snapshots = new List<Snapshot>{};
        collection = null;
    }
}

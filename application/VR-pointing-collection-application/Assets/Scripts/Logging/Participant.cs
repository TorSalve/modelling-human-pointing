using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Participant: MonoBehaviour {

    public LogManager logManager;

    [Header("Participant information")]
    [Tooltip("Participant id")]
    public int id;
    [Tooltip("Handedness")]
    public Handedness handedness;

    [Tooltip("Gender")]
    public Gender gender;
    
    [Tooltip("Age")]
    public float age;

    [Header("Forearm")]
    [Tooltip("Distance from wrist to elbow")]
    public float forearmLength;
    [Tooltip("Distance from elbow to middel of markers on forearm")]
    public float forearmMarkerDist;
    
    [Header("Hand")]
    [Tooltip("Length of index finger")]
    public float fingerLength;
    public float handIndexDist;
    public float handLength;
    
    [Header("Upperarm")]
    [Tooltip("Distance from shoulder to elbow")]
    public float upperarmLength;
    [Tooltip("Distance from elbow to middel of markers on upperarm")]
    public float upperarmMarkerDist;

    [Header("Shoulder")]
    [Tooltip("Distance from shoulder marker to shoulder (right)")]
    public float rightShoulderMarkerDistX;
    [Tooltip("Distance from shoulder marker to shoulder (up)")]
    public float rightShoulderMarkerDistY;

    [Header("Height")]
    [Tooltip("distance from floor to head")]
    public float height;
    [Tooltip("distance from shoulder to indexfinger tip")]
    public float armLength;


    void Start() {
        id = PlayerPrefs.GetInt("ParticipantId");
        handedness = GetHandednessFromIndex(PlayerPrefs.GetInt("Handedness"));
        gender = GetGenderFromIndex(PlayerPrefs.GetInt("Gender"));
        age = PlayerPrefs.GetFloat("Age");
        forearmLength = PlayerPrefs.GetFloat("ForearmLength");
        forearmMarkerDist = PlayerPrefs.GetFloat("ForearmMarkerDist");
        fingerLength = PlayerPrefs.GetFloat("IndexfingerLength");
        handLength = PlayerPrefs.GetFloat("HandLength");
        upperarmLength = PlayerPrefs.GetFloat("UpperarmLength");
        upperarmMarkerDist = PlayerPrefs.GetFloat("UpperarmMarkerDist");
        height = PlayerPrefs.GetFloat("Height");
        armLength = PlayerPrefs.GetFloat("ArmLength");
        rightShoulderMarkerDistX = PlayerPrefs.GetFloat("RightShoulderMarkerDist.X");
        rightShoulderMarkerDistY = PlayerPrefs.GetFloat("RightShoulderMarkerDist.Y");

        // height = 141;
        // height = 200;

        logManager.LogParticipant();
    }

    Handedness GetHandednessFromIndex(int handedness) {
        switch (handedness) {
            case 0: return Handedness.Right;
            default: return Handedness.Left;
        }
    }

    Gender GetGenderFromIndex(int gender) {
        switch (gender) {
            case 0: return Gender.Female;
            case 1: return Gender.Male;
            default: return Gender.Other;
        }
    }

    public void SetHandIndexDist(Vector3 handPos, Vector3 indexDist) { 
        handIndexDist = Vector3.Distance(handPos, indexDist);
    }
}
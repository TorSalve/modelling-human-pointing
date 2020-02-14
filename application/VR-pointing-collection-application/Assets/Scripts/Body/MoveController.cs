using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class MoveController : MonoBehaviour
{
    [Header("Avatar")]
    public FullBodyBipedIK avatarFullBody;
    public FingerRig hand;
    
    [Header("References")]
    public OptiTrackState otState;
    public Participant participant;
    public ScaleManager scaleManager;
    public int indexFingerIndex = 1;
    private bool runOnce = true;
    private float scale;

    // Start is called before the first frame update
    void Start()
    {
        avatarFullBody.solver.rightHandEffector.positionWeight = 1f;
        avatarFullBody.solver.rightHandEffector.rotationWeight = 1f;
        avatarFullBody.solver.rightShoulderEffector.positionWeight = 1f;
        avatarFullBody.solver.leftShoulderEffector.positionWeight = 1f;

        hand.fingers[indexFingerIndex].weight = 1f;
        hand.fingers[indexFingerIndex].rotationWeight = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if((otState.IsReady() && scaleManager.IsReady() )|| true) {
            if(runOnce) {
                avatarFullBody.solver.rightHandEffector.target = otState.goHand.transform;
                avatarFullBody.solver.rightShoulderEffector.target = otState.goShoulder1.transform;
                avatarFullBody.solver.leftShoulderEffector.target = otState.goShoulder2.transform;

                hand.fingers[indexFingerIndex].target = otState.goIndexfinger.transform;
                runOnce = false;
            }
            
            // avatarFullBody.transform.position = new Vector3(
            //     otState.goHmd.transform.position.x,
            //     0,// /*otState.head.y*/ 0 - participant.height/100.0f,
            //     otState.goHmd.transform.position.z
            // );
        }

        float AC = scaleManager.FromRealToVirtualWorld(participant.forearmMarkerDist / 100f);
        float BC = scaleManager.FromRealToVirtualWorld(participant.upperarmMarkerDist / 100f);
        float AB = Vector3.Distance(otState.goForearm.transform.localPosition, otState.goUpperarm.transform.localPosition);
        // angle of the elbow
        double elbowAngle = angleBetween(AB, AC, BC);
        double B = angleBetween(AC, AB, BC);
        double A = angleBetween(BC, AB, AC);

        // Debug.LogFormat("AC:{0};BC:{1};AB:{2};<A:{3};<B:{4};<C:{5}", AC, BC, AB, A, B, elbowAngle);
    }

    // angle for point A
    double angleBetween(float a, float b, float c) {
        float numerator = (b * b + c * c - a * a);
        float denominator = (2 * b * c);
        float fraction = numerator / denominator;
        double acos = Math.Acos((double)fraction);
        return radiansToDegrees(acos);
    }

    double angle_triangle(Vector3 v1, Vector3 v2, Vector3 v3) { 
        float x1 = v1.x; float y1 = v1.y; float z1 = v1.z;
        float x2 = v2.x; float y2 = v2.y; float z2 = v2.z;
        float x3 = v3.x; float y3 = v3.y; float z3 = v3.z;
        float num = (x2 - x1) * (x3 - x1) + (y2 - y1) * (y3 - y1) + (z2 - z1) * (z3 - z1); 
    
        double den = Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2) + Math.Pow((z2 - z1), 2)) * 
                     Math.Sqrt(Math.Pow((x3 - x1), 2) + Math.Pow((y3 - y1), 2) + Math.Pow((z3 - z1), 2)); 
    
        double angle = Math.Acos(num / den); 
        return radiansToDegrees(angle); 
    } 

    double radiansToDegrees(double radians) { return (180 / Math.PI) * radians; }
}

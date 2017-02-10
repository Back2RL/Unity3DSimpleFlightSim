using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject CameraPosDummy;
    public GameObject Camera;
    public float cameraSpeed = 5.0f;

    private Vector3 cameraUpVector;


    private float cameraRoll;

    void Start()
    {
    }


    void FixedUpdate() 
    {
        Camera.transform.position = V3Interp(Camera.transform.position, CameraPosDummy.transform.position, Time.fixedDeltaTime, cameraSpeed);

        // Quaternion targetRotation = Quaternion.LookRotation(CameraPosDummy.transform.rotation * new Vector3(0, 0, 1));
        //-- Camera.transform.rotation = CameraPosDummy.transform.rotation;
        Camera.transform.rotation = Quaternion.LookRotation(CameraPosDummy.transform.rotation * new Vector3(0, 0, 1));

        // Camera.transform.rotation = Quaternion.Slerp(Camera.transform.rotation, CameraPosDummy.transform.rotation, 0.1f);

        /*
        float cameraRollInput = Input.GetAxis("Horizontal") * 100;
        cameraRoll = cameraRoll - cameraRollInput * Time.fixedDeltaTime;
        Camera.transform.rotation = Quaternion.LookRotation(Camera.transform.forward, Quaternion.AngleAxis(cameraRoll, Camera.transform.forward) * Camera.transform.up);
        */

        // Camera.transform.rotation = Quaternion.Slerp(CameraPosDummy.transform.rotation, targetRotation, 0.5f);
    }


    float fInterp(float curr, float target, float dt, float speed)
    {
        if (Mathf.Abs(curr - target) < 0.0000001f) return target;
        float diff = target - curr;
        return curr + diff * Mathf.Min(1.0f, dt) * speed;
    }


    Vector3 V3Interp(Vector3 curr, Vector3 target, float dt, float speed)
    {
        if ((curr - target).sqrMagnitude < 0.0000001f) return target;
        Vector3 diff = target - curr;
        return curr + diff * Mathf.Min(1.0f, dt) * speed;
    }
}
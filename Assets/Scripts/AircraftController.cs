using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AircraftController : MonoBehaviour
{

    private float thrustPercentage;


    public float rollSensitivity = 0.5f;
    public float pitchSensitivity = 2.0f;
    public float pitchExponent = 1.5f;

    public bool inputEnabled = true;

    public float maxThrust = 10000.0f;
    public float thrustBlendSpeed = 0.25f; // how fast thrust increases/decreases

    public Rigidbody aircraft;

    public GameObject LeftWing;
    public GameObject RightWing;
    public GameObject Elevator;
    public GameObject Rudder;

    public Camera camera;
    public float upsideDownFlightAngle = 135.0f;

    public GameObject[] thrusters;

    // UI
    public Text thrustInputText;
    public Text currentForwardSpeedText;

    private Vector3 screenCenter;

    // Use this for initialization
    void Start()
    {
        aircraft = GetComponent<Rigidbody>();
        screenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f);
    }

    void Update()
    {
        thrustInputText.text = "Thrust = " + thrustPercentage.ToString();
        Vector3 forwardVel = Vector3.Project(aircraft.velocity, aircraft.transform.forward);
        currentForwardSpeedText.text = "Speed = " + forwardVel.magnitude.ToString();
    }

    void FixedUpdate()
    {
        Vector3 currVel = aircraft.velocity; // current Velocity in Worldspace

        if (inputEnabled)
        {


            Vector3 mousePosition = Input.mousePosition;
            Vector3 mouseDirection = mousePosition - screenCenter;

            mouseDirection.x = Mathf.Clamp(mouseDirection.x / screenCenter.x, -1.0f, 1.0f);
            mouseDirection.y = Mathf.Clamp(mouseDirection.y / screenCenter.y, -1.0f, 1.0f);

            mouseDirection.z = 1.0f;

            Vector3 targetDirection = camera.transform.TransformDirection(mouseDirection);

            Debug.DrawRay(camera.transform.position, targetDirection * 1000, Color.blue);

            targetDirection.Normalize();

            Vector3 movDir = currVel.normalized;

            float dotTargetMov = Vector3.Dot(movDir, targetDirection);

            Debug.DrawRay(camera.transform.position, movDir * 1000, Color.green);
            Debug.DrawRay(camera.transform.position, aircraft.transform.forward * 1000, Color.red);

            float dotTargetAircraftUp = Vector3.Dot(aircraft.transform.up, targetDirection);

            float dotTargetAircraftRight = 0;
            if (dotTargetAircraftUp < Mathf.Cos(upsideDownFlightAngle / 180.0f * Mathf.PI))
            {
                dotTargetAircraftRight = -Vector3.Dot(aircraft.transform.right, targetDirection);
            }
            else
            {
                dotTargetAircraftRight = Vector3.Dot(aircraft.transform.right, targetDirection);
            }


            float pitch = -Mathf.Sign(dotTargetAircraftUp) * Mathf.Pow(Mathf.Abs(dotTargetAircraftUp), pitchExponent);
            float roll = -dotTargetAircraftRight;

            if (Math.Abs(Input.GetAxis("Horizontal")) > Mathf.Abs(roll))
            {
                roll = -Input.GetAxis("Horizontal");
            }

            float yaw = dotTargetAircraftRight;


            Elevator.transform.rotation = Quaternion.LookRotation(aircraft.transform.forward + aircraft.transform.up * pitch, aircraft.transform.up + aircraft.transform.forward * pitch);
            LeftWing.transform.localRotation = Quaternion.LookRotation(new Vector3(0, 1, 0) + new Vector3(0, 0, 1) * roll, new Vector3(0, 0, -1) + new Vector3(0, 1, 0) * roll);
            RightWing.transform.localRotation = Quaternion.LookRotation(new Vector3(0, 1, 0) + new Vector3(0, 0, 1) * -roll, new Vector3(0, 0, -1) + new Vector3(0, 1, 0) * -roll);

            Rudder.transform.localRotation = Quaternion.LookRotation(new Vector3(0, 0, 1) + new Vector3(1, 0, 0) * -yaw, new Vector3(-1, 0, 0));
            // Thrusters
            float thrustDelta = Input.GetAxis("Vertical") * Time.fixedDeltaTime * thrustBlendSpeed;
            thrustPercentage = Mathf.Clamp(thrustPercentage + thrustDelta, 0, 1);

            foreach (GameObject thruster in thrusters)
            {
                aircraft.AddForceAtPosition(thruster.transform.forward * maxThrust * thrustPercentage,
                    thruster.transform.position, ForceMode.Force);
            }





            //    LeftWing.transform.Rotate(rollSensitivity * -Input.GetAxis("Mouse X"), 0, 0, Space.Self);
            //    RightWing.transform.Rotate(rollSensitivity * Input.GetAxis("Mouse X"), 0, 0, Space.Self);

            //    Elevator.transform.Rotate(pitchSensitivity * -Input.GetAxis("Mouse Y"), 0, 0, Space.Self);
            //
        }
    }

    void Awake()
    {
        foreach (WheelCollider w in GetComponentsInChildren<WheelCollider>())
            w.motorTorque = 0.000001f;
    }
}

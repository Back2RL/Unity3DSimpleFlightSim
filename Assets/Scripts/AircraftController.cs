using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AircraftController : MonoBehaviour
{
    // Camera
    public Camera camera;
    private Vector3 _screenCenter;

    // Controls
    public bool inputEnabled = true;
    public float switchToInvertedControlsAngle = 135.0f;
    private float _precalculatedAngle;
    public float rollSensitivity = 0.001f;

    // Aerodynamics
    private Rigidbody _aircraft;

    // Engine
    public GameObject[] thrusters;
    public float maxThrust = 10000.0f;
    public float thrustBlendSpeed = 0.25f; // how fast thrust increases/decreases (default: 4 seconds to max thrust)
    private float _engineThrottle;

    // Wings
    public GameObject LeftWing;
    public GameObject RightWing;
    public float rollExponent = 0.5f;
    public AnimationCurve rollInputCurve;

    // Elevator
    public GameObject Elevator;
    public float pitchExponent = 1.5f;

    // Rudder
    public GameObject Rudder;
    public float yawExponent = 1.5f;

    // UI
    public Text thrustInputText;
    public Text currentForwardSpeedText;


    // DEBUGGING
    public Vector3 localMovementDir;
    public Vector3 localTargetDir;
    public Vector3 localAircraftfoward;
    public Vector3 rightProjection;
    public float rightSlipMagnitude;
    public float direction;


    // Use this for initialization
    void Start()
    {
        _aircraft = GetComponent<Rigidbody>();
        _screenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f);
        _precalculatedAngle = Mathf.Cos(switchToInvertedControlsAngle / 180.0f * Mathf.PI);
    }

    void Update()
    {
        thrustInputText.text = "Thrust = " + _engineThrottle.ToString();
        Vector3 forwardVel = Vector3.Project(_aircraft.velocity, _aircraft.transform.forward);
        currentForwardSpeedText.text = "Speed = " + (forwardVel.magnitude * 3.6f).ToString() + " kph";
    }

    void FixedUpdate()
    {
        Vector3 currVel = _aircraft.velocity; // current Velocity in Globalspace
        Vector3 globalMovementDir = currVel.normalized;

        if (inputEnabled)
        {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 mouseDirection = mousePosition - _screenCenter;

            mouseDirection.x = Mathf.Clamp(mouseDirection.x / _screenCenter.x, -1.0f, 1.0f);
            mouseDirection.y = Mathf.Clamp(mouseDirection.y / _screenCenter.y, -1.0f, 1.0f);

            mouseDirection.z = 1.0f;

            Vector3 targetDirection = camera.transform.TransformDirection(mouseDirection);
            
            // test gravity reduction
            //targetDirection -= Physics.gravity * rollSensitivity;

            targetDirection.Normalize();
            Debug.DrawRay(camera.transform.position, targetDirection * 1000, Color.blue);

            Vector3 movDir = currVel.normalized;

            float dotTargetMov = Vector3.Dot(movDir, targetDirection);

            Debug.DrawRay(camera.transform.position, movDir * 1000, Color.green);
            Debug.DrawRay(camera.transform.position, _aircraft.transform.forward * 1000, Color.red);

            float dotTargetAircraftUp = Vector3.Dot(_aircraft.transform.up, targetDirection);

            float dotTargetAircraftRight = 0;
            if (dotTargetAircraftUp < _precalculatedAngle)
            {
                dotTargetAircraftRight = -Vector3.Dot(_aircraft.transform.right, targetDirection);
            }
            else
            {
                dotTargetAircraftRight = Vector3.Dot(_aircraft.transform.right, targetDirection);
            }

            // --------------
            // project globalMovementDirection to localMovementDirection
            localMovementDir = _aircraft.transform.InverseTransformDirection(globalMovementDir);

            // project globalTargetDirection to localMovementDirection
            localTargetDir = _aircraft.transform.InverseTransformDirection(targetDirection);

            localAircraftfoward = Vector3.forward;
           
            Vector3 movementAxis = Vector3.ProjectOnPlane(localMovementDir, Vector3.forward);

            // check for x-movement (project localMovementDir onto aircraft-right-vector

            Debug.DrawRay(_aircraft.transform.position, _aircraft.transform.TransformDirection(movementAxis * 1000), Color.cyan);

            // direction = Mathf.Sign(Vector3.Dot(rightProjection, localMovementDir));
            Vector3.Dot(movementAxis, Vector3.right);
            rightSlipMagnitude = Vector3.Dot(movementAxis, Vector3.right);

           // rightSlipMagnitude = /*direction * rightProjection.magnitude*/ Mathf.Clamp(movementAxis.x * rollSensitivity, -1.0f, 1.0f);

            //dotTargetAircraftRight = Mathf.Clamp(rightSlipMagnitude, -1.0f, 1.0f);
            //-------------


            float pitch = -Mathf.Sign(dotTargetAircraftUp) * Mathf.Pow(Mathf.Abs(dotTargetAircraftUp), pitchExponent);
            //float roll = rightSlipMagnitude;
            //float roll = Mathf.Sign(rightSlipMagnitude) * Mathf.Pow(Mathf.Abs(rightSlipMagnitude), rollExponent);

            float roll = rollInputCurve.Evaluate(rightSlipMagnitude);

            if (Math.Abs(Input.GetAxis("Horizontal")) > Mathf.Abs(roll))
            {
                roll = -Input.GetAxis("Horizontal");
            }

            float yaw = Mathf.Sign(dotTargetAircraftRight) * Mathf.Pow(Mathf.Abs(dotTargetAircraftRight), yawExponent);


            Elevator.transform.rotation =
                Quaternion.LookRotation(_aircraft.transform.forward + _aircraft.transform.up * pitch,
                    _aircraft.transform.up + _aircraft.transform.forward * pitch);
            LeftWing.transform.localRotation =
                Quaternion.LookRotation(new Vector3(0, 1, 0) + new Vector3(0, 0, 1) * roll,
                    new Vector3(0, 0, -1) + new Vector3(0, 1, 0) * roll);
            RightWing.transform.localRotation =
                Quaternion.LookRotation(new Vector3(0, 1, 0) + new Vector3(0, 0, 1) * -roll,
                    new Vector3(0, 0, -1) + new Vector3(0, 1, 0) * -roll);

            Rudder.transform.localRotation = Quaternion.LookRotation(
                new Vector3(0, 0, 1) + new Vector3(1, 0, 0) * -yaw, new Vector3(-1, 0, 0));
            // Thrusters
            float thrustDelta = Input.GetAxis("Vertical") * Time.fixedDeltaTime * thrustBlendSpeed;
            _engineThrottle = Mathf.Clamp(_engineThrottle + thrustDelta, 0, 1);

            foreach (GameObject thruster in thrusters)
            {
                _aircraft.AddForceAtPosition(thruster.transform.forward * maxThrust * _engineThrottle,
                    thruster.transform.position, ForceMode.Force);
            }
        }
    }

    void Awake()
    {
        foreach (WheelCollider w in GetComponentsInChildren<WheelCollider>())
            w.motorTorque = 0.0000001f;
    }
}
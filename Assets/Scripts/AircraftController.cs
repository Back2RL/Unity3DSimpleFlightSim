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

    public bool gravityCompensation = false;
    private bool _keyPressedPreviousTick = false;

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
    public float xDirectionDrift;
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

            // TESTING
            bool isKeyDown = !Input.GetKeyUp("x");
            if (!isKeyDown && _keyPressedPreviousTick)
            {
                gravityCompensation = !gravityCompensation;
            }
            _keyPressedPreviousTick = isKeyDown;
            // \TESTING
            if (gravityCompensation)
            {
                { // Roll input using yaw-drift compensation to negate the effect of gravity

                    // project globalMovementDirection to localMovementDirection
                    localMovementDir = _aircraft.transform.InverseTransformDirection(globalMovementDir);

                    // project globalTargetDirection to localMovementDirection
                    localTargetDir = _aircraft.transform.InverseTransformDirection(targetDirection);

                    // project localMovementDir to local Right-Up-plane using the Forward-direction as normal
                    Vector3 localDriftVelocity = Vector3.ProjectOnPlane(localMovementDir, Vector3.forward);

                    // Draw Debug ray
                    Debug.DrawRay(_aircraft.transform.position,
                        _aircraft.transform.TransformDirection(localDriftVelocity * 1000), Color.cyan);

                    // calculate the amount of Drift in Right-Direction
                    xDirectionDrift = Vector3.Dot(localDriftVelocity, Vector3.right);
                }
            }
            else
            {
                // direct input using target direction, without gravity compensation
                float directRollInput = -Mathf.Sign(dotTargetAircraftRight) * Mathf.Pow(Mathf.Abs(dotTargetAircraftRight), rollExponent);

                xDirectionDrift = directRollInput;
            }

            // pitch controlinput TODO: add curve
            float pitch = -Mathf.Sign(dotTargetAircraftUp) * Mathf.Pow(Mathf.Abs(dotTargetAircraftUp), pitchExponent);

            // roll controlinput from curve
            float roll = rollInputCurve.Evaluate(xDirectionDrift);

            // yaw controlinput TODO: add curve
            float yaw = Mathf.Sign(dotTargetAircraftRight) * Mathf.Pow(Mathf.Abs(dotTargetAircraftRight), yawExponent);

            // check for override by player (using direct keyinputs)
            if (Math.Abs(Input.GetAxis("Horizontal")) > Mathf.Abs(roll))
            {
                // set player input to be controlinput
                roll = -Input.GetAxis("Horizontal");
            }



            Elevator.transform.rotation = Quaternion.LookRotation(_aircraft.transform.forward + _aircraft.transform.up * pitch, _aircraft.transform.up + _aircraft.transform.forward * pitch);
            LeftWing.transform.localRotation = Quaternion.LookRotation(new Vector3(0, 1, 0) + new Vector3(0, 0, 1) * roll, new Vector3(0, 0, -1) + new Vector3(0, 1, 0) * roll);
            RightWing.transform.localRotation = Quaternion.LookRotation(new Vector3(0, 1, 0) + new Vector3(0, 0, 1) * -roll, new Vector3(0, 0, -1) + new Vector3(0, 1, 0) * -roll);
            Rudder.transform.localRotation = Quaternion.LookRotation(new Vector3(0, 0, 1) + new Vector3(1, 0, 0) * -yaw, new Vector3(-1, 0, 0));

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
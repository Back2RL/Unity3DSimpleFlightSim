using System;
using UnityEngine;
using UnityEngine.UI;

public class AircraftController : MonoBehaviour
{
    // Camera
    public Camera Camera;
    private Vector3 _screenCenter;

    // Controls
    public bool InputEnabled = true;
    public float SwitchToInvertedControlsAngle = 135.0f;
    private float _precalculatedAngle;
    public float RollSensitivity = 0.001f;

    public bool GravityCompensation;
    private bool _keyPressedPreviousTick;

    // Aerodynamics
    private Rigidbody _aircraft;

    // Engine
    public GameObject[] Thrusters;
    public float MaxThrust = 10000.0f;
    public float ThrustBlendSpeed = 0.25f; // how fast thrustMax increases/decreases (default: 4 seconds to max thrustMax)
    private float _engineThrottle;

    // Wings
    public GameObject LeftWing;
    public GameObject RightWing;
    public float RollExponent = 0.5f;
    public AnimationCurve RollInputCurve;

    // Elevator
    public GameObject Elevator;
    public float PitchExponent = 1.5f;

    // Rudder
    public GameObject Rudder;
    public float YawExponent = 1.5f;

    // UI
    public Text ThrustInputText;
    public Text CurrentForwardSpeedText;


    // DEBUGGING
    public Vector3 LocalMovementDir;
    public Vector3 LocalTargetDir;
    public Vector3 LocalAircraftfoward;
    public Vector3 RightProjection;
    public float XDirectionDrift;
    public float Direction;


    // Use this for initialization
    void Start()
    {
        _aircraft = GetComponent<Rigidbody>();
        _screenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f);
        _precalculatedAngle = Mathf.Cos(SwitchToInvertedControlsAngle / 180.0f * Mathf.PI);
    }

    void Update()
    {
        ThrustInputText.text = "Thrust = " + _engineThrottle;
        Vector3 forwardVel = Vector3.Project(_aircraft.velocity, _aircraft.transform.forward);
        CurrentForwardSpeedText.text = "Speed = " + (forwardVel.magnitude * 3.6f) + " kph";
    }

    void FixedUpdate()
    {
        Vector3 currVel = _aircraft.velocity; // current Velocity in Globalspace
        Vector3 globalMovementDir = currVel.normalized;

        if (InputEnabled)
        {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 mouseDirection = mousePosition - _screenCenter;

            mouseDirection.x = Mathf.Clamp(mouseDirection.x / _screenCenter.x, -1.0f, 1.0f);
            mouseDirection.y = Mathf.Clamp(mouseDirection.y / _screenCenter.y, -1.0f, 1.0f);

            mouseDirection.z = 1.0f;

            Vector3 targetDirection = Camera.transform.TransformDirection(mouseDirection);

            targetDirection.Normalize();
            Debug.DrawRay(Camera.transform.position, targetDirection * 1000, Color.blue);

            Vector3 movDir = currVel.normalized;

            float unused = Vector3.Dot(movDir, targetDirection);

            Debug.DrawRay(Camera.transform.position, movDir * 1000, Color.green);
            Debug.DrawRay(Camera.transform.position, _aircraft.transform.forward * 1000, Color.red);

            float dotTargetAircraftUp = Vector3.Dot(_aircraft.transform.up, targetDirection);

            float dotTargetAircraftRight;
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
                GravityCompensation = !GravityCompensation;
            }
            _keyPressedPreviousTick = isKeyDown;
            // \TESTING
            if (GravityCompensation)
            {
                { // Roll input using yaw-drift compensation to negate the effect of gravity

                    // convert globalMovementDirection to localMovementDirection
                    LocalMovementDir = _aircraft.transform.InverseTransformDirection(globalMovementDir);

                    // convert globalTargetDirection to localMovementDirection
                    LocalTargetDir = _aircraft.transform.InverseTransformDirection(targetDirection);

                    // project localMovementDir to local Right-Up-plane using the Forward-direction as normal
                    Vector3 localDriftVelocity = Vector3.ProjectOnPlane(LocalMovementDir, Vector3.forward);

                    // Draw Debug ray
                    Debug.DrawRay(_aircraft.transform.position,
                        _aircraft.transform.TransformDirection(localDriftVelocity * 1000), Color.cyan);

                    // calculate the amount of Drift in Right-Direction
                    XDirectionDrift = Vector3.Dot(localDriftVelocity, Vector3.right);
                }
            }
            else
            {
                // direct input using target direction, without gravity compensation
                float directRollInput = -Mathf.Sign(dotTargetAircraftRight) * Mathf.Pow(Mathf.Abs(dotTargetAircraftRight), RollExponent);

                XDirectionDrift = directRollInput;
            }

            // pitch controlinput TODO: add curve
            float pitch = -Mathf.Sign(dotTargetAircraftUp) * Mathf.Pow(Mathf.Abs(dotTargetAircraftUp), PitchExponent);

            // roll controlinput from curve
            float roll = RollInputCurve.Evaluate(XDirectionDrift);

            // yaw controlinput TODO: add curve
            float yaw = Mathf.Sign(dotTargetAircraftRight) * Mathf.Pow(Mathf.Abs(dotTargetAircraftRight), YawExponent);

            // add player Keyinput
            roll = Mathf.Clamp(roll - 2 * Input.GetAxis("Horizontal"), -1.0f, 1.0f);

            Elevator.transform.rotation = Quaternion.LookRotation(_aircraft.transform.forward + _aircraft.transform.up * pitch, _aircraft.transform.up + _aircraft.transform.forward * pitch);
            LeftWing.transform.localRotation = Quaternion.LookRotation(new Vector3(0, 1, 0) + new Vector3(0, 0, 1) * roll, new Vector3(0, 0, -1) + new Vector3(0, 1, 0) * roll);
            RightWing.transform.localRotation = Quaternion.LookRotation(new Vector3(0, 1, 0) + new Vector3(0, 0, 1) * -roll, new Vector3(0, 0, -1) + new Vector3(0, 1, 0) * -roll);
            Rudder.transform.localRotation = Quaternion.LookRotation(new Vector3(0, 0, 1) + new Vector3(1, 0, 0) * -yaw, new Vector3(-1, 0, 0));

            // Thrusters
            float thrustDelta = Input.GetAxis("Vertical") * Time.fixedDeltaTime * ThrustBlendSpeed;
            _engineThrottle = Mathf.Clamp(_engineThrottle + thrustDelta, 0, 1);
            foreach (GameObject thruster in Thrusters)
            {
                _aircraft.AddForceAtPosition(thruster.transform.forward * MaxThrust * _engineThrottle,
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
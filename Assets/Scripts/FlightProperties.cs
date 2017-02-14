using UnityEngine;
using System.Collections;

// input of the controls over the plane
public class FlightProperties : MonoBehaviour
{

    public BoxPhysics boxPhysics;

    public float thrustMin = 50;
    public float thrustMax;
    public float turnSpeed = .03f;
    Rigidbody rb;
    public float fwdSpeed;
    public float turnCutoff = 50;

    public float angularSpeed;
    public Vector2 pointer;
    Vector3 screenCenter;
    public Vector3 finalRotation;
    public float deltaAngle;
    public float roll0 = 0; // maximum roll value
    public float roll = 0; // current roll value variable


    public float angle;



    // Use this for initialization
    void Start()
    {
        screenCenter = new Vector3((Screen.width / 2), (Screen.height / 2), 0);

    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Steering()
    {
        finalRotation = new Vector3(-pointer.x, pointer.y, 0) * turnSpeed;
        finalRotation = Vector3.ClampMagnitude(finalRotation, turnCutoff);
        rb.AddRelativeTorque(finalRotation); // mouse steering

    }

    void Control()
    {
        //deltaAngle = Mathf.DeltaAngle(boxPhysics.localPlaneSpeedProjection, pointer);
        //        angle = Vector3.AngleBetween(boxPhysics.localPlaneSpeedProjection, pointer);
        angle = Vector3.Angle(boxPhysics.localPlaneSpeedProjection, pointer);

    }



    void AxisInput()
    {
        roll = roll0 * Input.GetAxis("Horizontal");
        rb.AddRelativeTorque(Vector3.forward * -roll,ForceMode.Acceleration);

        thrustMax = thrustMin + (thrustMin * Input.GetAxis("Vertical"));
        rb.AddForce(rb.transform.forward * thrustMax, /* by Leo */ ForceMode.Acceleration);

    }

    void Update()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        pointer = new Vector2(Input.mousePosition.y - screenCenter.y, Input.mousePosition.x - screenCenter.x);
        AxisInput();
        Steering();
        Control();
        angularSpeed = finalRotation.magnitude;
        fwdSpeed = Vector3.Dot(rb.velocity, transform.forward);

    }
}

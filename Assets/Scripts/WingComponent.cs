using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingComponent : MonoBehaviour
{

    public Rigidbody aircraftBody;

    // Helper opbjects to calculate area of lift/drag
    public GameObject edge0;
    public GameObject edge1;
    public GameObject edge3;

    public AnimationCurve liftCurve;
    public AnimationCurve dragCurve;

    // temporary solution for lift/drag coefficients
    public float lift = 2;    // liftmultiplier * density/2
    public float drag = 0.2f;     // dragmultiplier * density/2

    // area of lift/drag
    private float wingArea;

    void Start()
    {
        // temporary wing area calculation (A = a * b)
        float a = (edge0.transform.position - edge1.transform.position).magnitude;
        float b = (edge1.transform.position - edge3.transform.position).magnitude;
        wingArea = a * b;
        print("Wingarea = " + wingArea.ToString() + " m²");

        Destroy(edge0);
        Destroy(edge1);
        Destroy(edge3);

    }

    void FixedUpdate()
    {
        Vector3 currVel = aircraftBody.GetPointVelocity(transform.position);    // current Velocity in Worldspace

        Vector3 movForwardDir = currVel.normalized;                             // current Movement Direction in Worldspace
        Vector3 movRightDir = Vector3.Cross(transform.up, movForwardDir);       // Movement Right-Direction Vector (from Movement Direction and Aircraft Up-Direction)
        Vector3 movUpDir = Vector3.Cross(movForwardDir, movRightDir);           // Movement Up-Direction Vector (from Movement Direction and Movement Right-Direction)

        // Wing Lift
        float liftCoefficient = Vector3.Dot(transform.up, movForwardDir);                                               // cl: temporary lift Coefficient from angle of attack
        float liftForce = -liftCoefficient * lift * 1.2041f * 0.5f * currVel.sqrMagnitude * wingArea;                   // cl * density / 2 * v² * A (lift = multiplier * density/2)
        aircraftBody.AddForceAtPosition(movUpDir * liftForce, transform.position, ForceMode.Force);

        // direction of airresistance
        Vector3 dragDir = -movForwardDir;

        // Wing airresistance
        float resistanceCoefficient = Mathf.Abs(Vector3.Dot(transform.up, movForwardDir));                              // cw: temporary resistance Coefficient from angle of attack
        float resistanceForceMagnitude = resistanceCoefficient * drag * currVel.sqrMagnitude * wingArea;                // cw * density / 2 * v² * A (drag = multiplier * density/2)
        aircraftBody.AddForceAtPosition(dragDir * resistanceForceMagnitude, transform.position, ForceMode.Force);
    }
}

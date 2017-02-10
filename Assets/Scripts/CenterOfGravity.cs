using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterOfGravity : MonoBehaviour
{

    public GameObject cg;
    public Rigidbody rb;
   
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = cg.transform.localPosition;
    }
}

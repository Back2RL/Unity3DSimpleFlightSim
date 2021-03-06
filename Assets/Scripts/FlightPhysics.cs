﻿using UnityEngine;
using System.Collections;
//aerodynamics of the plane
public class FlightPhysics : MonoBehaviour {
	Rigidbody rb;
	public float globalDragCoeff = 1.0f; //15    
	public float globalTorqCoeff = 1.0f;
	public Vector3 dragCoeff = new Vector3(0.2f, 1f, 0.1f);
	public Vector3 torqCoeff = new Vector2 (1f, 1f);
	public Vector3 dragForce;
	public Vector3 torqForce;
	public Vector3 localSpeed;
//	public Vector3 velocity;

	// Use this for initialization

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

	void Start () {
		dragCoeff = dragCoeff * globalDragCoeff;
		torqCoeff = torqCoeff * globalTorqCoeff;
	}
	void Drag () {
        //dragForce = Vector3.Scale (localSpeed, dragCoeff);
        //rb.AddRelativeForce (-dragForce);

        // by Leo
       
        // calculate velocity reduction vector using drag coefficients
        dragForce = Vector3.Scale(-localSpeed, dragCoeff);
        // apply velocity reduction as change in velocity to simulate drag
        rb.AddRelativeForce(dragForce, ForceMode.VelocityChange);

	}
	void Torque () {
		torqForce = Vector3.Scale (localSpeed, torqCoeff);
		rb.AddRelativeTorque (new Vector2(-torqForce.y, torqForce.x)*globalTorqCoeff);
	}

	void FixedUpdate(){
        // convert global Velocity to local Velocity
        localSpeed = transform.InverseTransformVector(rb.velocity);
//		velocity = rb.velocity;
		Drag();
		Torque();
		//DrawLine();

	}
}

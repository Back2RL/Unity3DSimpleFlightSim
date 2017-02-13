using UnityEngine;
using System.Collections;
// 
public class BoxPhysics : MonoBehaviour {
	public Rigidbody plane;
	/*public Rigidbody box;
	/*public Vector3 dragCoeff = new Vector3(0.2f, 1f, 0.1f);
	public Vector3 torqCoeff = new Vector2 (1f, 1f);
	public Vector3 dragForce;
	public Vector3 torqForce;
	public Vector3 localPlaneSpeed;*/
	public Vector3 localPlaneSpeedProjection;

	// Use this for initialization
	void Start () {
	
	}
	void DrawRay () {
		Debug.DrawRay (transform.position, plane.velocity, Color.green); // local? velocity
		Debug.DrawRay (transform.position, localPlaneSpeedProjection, Color.red); // local? velocity 2d projection
		Debug.DrawRay (transform.position, transform.forward, Color.yellow); // forward
	}
	
	// Update is called once per frame
	void Update () {
	
	}

/*	void Drag () {
		dragForce = Vector3.Scale (localPlaneSpeed, dragCoeff);
		dragForce = transform.TransformPoint (transform.forward);
		box.AddForce (-dragForce);
	}

	void Torque () {
		torqForce = Vector3.Scale (localPlaneSpeed, torqCoeff);
		torqForce = transform.TransformPoint (transform.forward);
		box.AddTorque (new Vector2(-torqForce.y, torqForce.x));
	}*/

	void FixedUpdate () {
		//localPlaneSpeed = transform.InverseTransformVector(plane.velocity);
		localPlaneSpeedProjection = Vector3.ProjectOnPlane (plane.velocity, transform.forward);
		DrawRay();
		
	}
}

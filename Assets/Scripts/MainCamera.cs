using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour {
    public GameObject Plane;
    //public FlightProperties flightProperties;
    //public BoxPhysics boxPhysics;
    Vector3 moveCamTo;
    public float cameraRotation;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		moveCamTo = Plane.transform.position - Plane.transform.forward * 40.0f + Vector3.up * 2.0f;
		//float bias = 0.9f;
		//Camera.main.transform.position = Camera.main.transform.position * bias + moveCamTo * (1.0f - bias);
		Camera.main.transform.position = Vector3.Slerp(Camera.main.transform.position, moveCamTo, .1f);
        //transform.LookAt(Plane.transform.position, transform.up);
        transform.rotation = Quaternion.LookRotation(Plane.transform.forward, transform.up);
        //cameraRotation = Vector3.Angle (boxPhysics.localPlaneSpeedProjection, flightProperties.pointer);

        //Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, Plane.transform.rotation, .3f);


    }
}

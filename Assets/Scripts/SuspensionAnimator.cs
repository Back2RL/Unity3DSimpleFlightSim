using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuspensionAnimator : MonoBehaviour
{

    private WheelCollider wheelCollider;

	// Use this for initialization
	void Start ()
	{
	    wheelCollider = GetComponent<WheelCollider>();

	}
	
	// Update is called once per frame
	void Update () {
  

        // wheel movements
        if (wheelCollider.transform.childCount == 0)
        {
            return;
        }

        Transform visualWheel = wheelCollider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;
        wheelCollider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    

}
}

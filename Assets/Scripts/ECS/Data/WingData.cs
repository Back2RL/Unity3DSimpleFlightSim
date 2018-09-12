using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;


/**
 * requires a WingInitializedData Component to be initialized by the WingInitializerSystem
 */
[RequireComponent(typeof(GameObjectEntity))]
public class WingData : MonoBehaviour
{
	
	// Helper opbjects to calculate area of lift/drag
	public GameObject edge0;
	public GameObject edge1;
	public GameObject edge3;

	
	public AnimationCurve liftCurve;
	public AnimationCurve dragCurve;

	public Rigidbody Rigidbody;

	// temporary solution for lift/drag coefficients
	public float lift = 2;    // liftmultiplier * density/2
	public float drag = 0.2f;     // dragmultiplier * density/2
	// area of lift/drag
	public float wingArea;

}
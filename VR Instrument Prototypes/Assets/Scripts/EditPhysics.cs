using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditPhysics : MonoBehaviour {

    public Vector3 gravity = new Vector3( 0, -9.8f, 0 );

	// Use this for initialization
	void Start()
    {
	    Physics.gravity = gravity;	
	}
	
}

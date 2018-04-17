using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairBaseFollowController : MonoBehaviour {

    public Transform thingToFollow;
    private Rigidbody myRB;

	void Start()
    {
	    transform.position = thingToFollow.position;
        myRB = GetComponent<Rigidbody>();
	}
	
	void FixedUpdate()
    {
		//myRB.position = thingToFollow.position;
        myRB.MovePosition( thingToFollow.position );
	}
}

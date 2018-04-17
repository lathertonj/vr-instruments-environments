using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairBaseFollowController2 : MonoBehaviour {

    public Transform thingToFollow;

	void Start()
    {
        // move my things over to the thing to follow
	    transform.position = thingToFollow.position;
        // move my base onto the thing to follow
        Transform myBase = GetComponentInChildren<HairFirstComponentController>().transform;
        myBase.parent = thingToFollow;
        // attach my second component to that thing
        HairSecondComponentController second = GetComponentInChildren<HairSecondComponentController>();
        second.Attach( thingToFollow.GetComponent<Rigidbody>() );
	}

}

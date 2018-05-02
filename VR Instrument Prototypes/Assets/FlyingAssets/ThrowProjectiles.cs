using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowProjectiles : MonoBehaviour
{

    // VR controller preamble
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    private void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        myRigidbody = GetComponent<Rigidbody>();
    }

    public Rigidbody projectilePrefab;
    private Rigidbody myRigidbody;
    private Rigidbody myCurrentProjectile;
	private Vector3 controllerOffset = new Vector3( 0, -0.04f, 0.2f );

	// Update is called once per frame
	void Update()
    {
		if( Controller.GetPressDown( SteamVR_Controller.ButtonMask.Trigger ) )
        {
            // spawn
            myCurrentProjectile = Instantiate( 
                projectilePrefab, 
                transform.position + (transform.rotation * controllerOffset), 
                transform.rotation * Quaternion.AngleAxis( 90, Vector3.up )
            );
            // add fixed joint
            FixedJoint attachment = myCurrentProjectile.gameObject.AddComponent<FixedJoint>();
            attachment.connectedBody = myRigidbody;
            attachment.breakForce = float.PositiveInfinity;
            attachment.breakTorque = float.PositiveInfinity;
        }


        if( Controller.GetPressUp( SteamVR_Controller.ButtonMask.Trigger ) )
        {
            // destroy the attachment
            Destroy( myCurrentProjectile.GetComponent<FixedJoint>() );
            // and launch it away!
            // myCurrentProjectile.velocity = Controller.velocity; // this makes things come out at the wrong angle. 
            // myCurrentProjectile.angularVelocity = Controller.angularVelocity;
            ThrowObject( myCurrentProjectile );

            myCurrentProjectile = null;
        }
	}

    void ThrowObject( Rigidbody toThrow )
    {
        // see reddit post https://www.reddit.com/r/vrdev/comments/51l5dy/unity_physics_problem_with_vive_thrown_objects
        Transform origin = trackedObj.origin ? trackedObj.origin : trackedObj.transform.parent;
        if( origin != null )
        {
            toThrow.velocity = origin.TransformVector( toThrow.velocity );
            toThrow.GetRelativePointVelocity( origin.TransformVector( Controller.angularVelocity ) );
        }

        toThrow.AddForce( Controller.velocity, ForceMode.VelocityChange );
        toThrow.angularVelocity = Controller.angularVelocity;
    }
}

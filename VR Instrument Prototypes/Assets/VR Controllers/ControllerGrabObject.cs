using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerGrabObject : MonoBehaviour {

    // controller preamble
    private SteamVR_TrackedObject trackedObj;

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        objectInHeadIsKinematic = new Dictionary<Rigidbody, bool>();
    }


    private GameObject collidingObject; 
    private GameObject objectInHand;
    private bool objectInHandWasKinematic;
    private Dictionary< Rigidbody, bool > objectInHeadIsKinematic;
    private GameObject head;

    private GameObject FindTheRigidBody( GameObject start )
    {
        Rigidbody potentialRB = start.GetComponent<Rigidbody>();
        if( potentialRB != null )
        {
            return potentialRB.gameObject;
        }
        
        potentialRB = start.GetComponentInParent<Rigidbody>();
        if( potentialRB != null )
        {
            return potentialRB.gameObject;
        }
        return null;
    }

    private void SetCollidingObject( Collider col )
    {
        if( collidingObject != null )
        {
            return;
        }

        collidingObject = FindTheRigidBody( col.gameObject );
    }

    public void OnTriggerEnter( Collider other )
    {
        if( other.gameObject.CompareTag( "Head" ) )
        {
            head = other.gameObject;
        }
        else
        {
            SetCollidingObject(other);
        }

    }

    public void OnTriggerStay( Collider other )
    {
        SetCollidingObject(other);
    }

    public void OnTriggerExit( Collider other )
    {
        if( FindTheRigidBody( other.gameObject ) == collidingObject )
        {
            collidingObject = null;
        }

        if( other.gameObject == head )
        {
            head = null;
        }
    }

    private void GrabObject()
    {
        objectInHand = collidingObject;
        collidingObject = null;
        Rigidbody rbToPickUp = objectInHand.GetComponent<Rigidbody>();
        
        // if I'm trying to detach something from my head, detach it
        if( head != null )
        {
            FixedJoint headJoint = head.GetComponent<FixedJoint>();
            if( headJoint != null && headJoint.connectedBody == rbToPickUp )
            {
                Destroy( headJoint );
                // restore isKinematic
                rbToPickUp.isKinematic = objectInHeadIsKinematic[rbToPickUp];
            }
        }

        FixedJoint joint = AddFixedJoint();
        joint.connectedBody = rbToPickUp;
        // turn off isKinematic so that we can move the thing we picked up
        objectInHandWasKinematic = joint.connectedBody.isKinematic;
        joint.connectedBody.isKinematic = false;
    }

    private FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = float.PositiveInfinity;
        fx.breakTorque = float.PositiveInfinity;
        return fx;
    }

    private void ReleaseObject()
    {
        FixedJoint maybeFixedJoint = GetComponent<FixedJoint>();
        if (maybeFixedJoint)
        {
            Rigidbody rbInHand = maybeFixedJoint.connectedBody;
            maybeFixedJoint.connectedBody = null;
            Destroy(maybeFixedJoint);
            // restore isKinematic
            rbInHand.isKinematic = objectInHandWasKinematic;
            // add controller velocity and angular velocity for throwing
            rbInHand.velocity = Controller.velocity;
            rbInHand.angularVelocity = Controller.angularVelocity;

            if( head != null )
            {
                // affix it to head
                FixedJoint fx = head.AddComponent<FixedJoint>();
                fx.breakForce = float.PositiveInfinity;
                fx.breakTorque = float.PositiveInfinity;
                fx.connectedBody = rbInHand;
                // store for later
                objectInHeadIsKinematic[rbInHand] = rbInHand.isKinematic;
                rbInHand.isKinematic = false; 
            }
        }
        objectInHand = null;
    }

	// Update is called once per frame
	void Update() 
    {
        if (Controller.GetHairTriggerDown())
        {
            if (collidingObject)
            {
                GrabObject();
            }
        }

        if (Controller.GetHairTriggerUp())
        {
            if (objectInHand)
            {
                ReleaseObject();
            }
        }
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerSlingshot : MonoBehaviour {
    
    // VR preamble
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    private void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    // =====================================================================

    public Transform projectilePrefab;
    public Material lineMaterial;
    public float throwStrength = 10f;


    private bool collidingWithOtherController = false;
    private bool pullingBack = false;
    private ControllerSlingshot otherController = null;
    private ControllerSlingshot pullbackOtherController = null;
    private Transform currentProjectile;
    private LaserArc currentLaserArc;
    private Vector3 currentProjectileVelocity;

    private Vector3 controllerOffset = new Vector3( 0, -0.04f + 0.05f, 0.09f );


	// Use this for initialization
	void Start()
    {
		
	}
	
	// Update is called once per frame
	void Update()
    {
		if( collidingWithOtherController && !pullingBack && Controller.GetPressDown( SteamVR_Controller.ButtonMask.Trigger ) )
        {
            // initiate draw-back!
            pullingBack = true;
            pullbackOtherController = otherController;

            // instantiate the projectile prefab at the tip of the controller
            currentProjectile = Instantiate( 
                projectilePrefab, 
                GetControllerTipLocation(), 
                transform.rotation * Quaternion.AngleAxis( 90, Vector3.up ),
                transform
            );
            // set some properties
            // TODO: does the collider need to be trigger?
            // currentProjectile.GetComponent<Collider>().isTrigger = true;
            Rigidbody projectile = currentProjectile.GetComponent<Rigidbody>();
            projectile.useGravity = false;
            projectile.isKinematic = true;

            // add a laser arc
            currentLaserArc = currentProjectile.gameObject.AddComponent<LaserArc>();
            // this added a lineRenderer -- set its material
            currentLaserArc.GetComponent<LineRenderer>().material = lineMaterial;


            // TODO: draw some slingshot model hooked onto the other controller
        }

        if( pullingBack && Controller.GetPress( SteamVR_Controller.ButtonMask.Trigger ) )
        {
            // velocity: from our pullback controller to the other controller
            currentProjectileVelocity = throwStrength * ( 
                pullbackOtherController.GetControllerTipLocation() 
                - GetControllerTipLocation() 
            );
            currentLaserArc.SetVelocity( currentProjectileVelocity );

            // TODO: update the slingshot model
        }

        if( pullingBack && Controller.GetPressUp( SteamVR_Controller.ButtonMask.Trigger ) )
        {
            // launch!
            

            // stop showing the LaserArc
            Destroy( currentLaserArc );
            Destroy( currentProjectile.GetComponent<LineRenderer>() );

            // unparent it -- be free!
            currentProjectile.transform.parent = null;
            // TODO: consider setting its position to where the other controller is
            // TODO: if I don't do this, then do I no longer need the 1.08 factor on the laser arc?
            /// currentProjectile.position = pullbackOtherController.GetControllerTipLocation(); 
            // rigidbody: make it a gravity-obeying non kinematic projectile
            Rigidbody projectile = currentProjectile.GetComponent<Rigidbody>();
            projectile.useGravity = true;
            projectile.isKinematic = false;
            // velocity in direction of pull back
            projectile.AddForce( currentProjectileVelocity, ForceMode.VelocityChange );
            // TODO: no longer a trigger: collide away!?
            // currentProjectile.GetComponent<Collider>().isTrigger = false;

            pullingBack = false;
            pullbackOtherController = null;

            // TODO: remove the slingshot model
        }
	}

    private void OnTriggerEnter( Collider other )
    {
        // do no checks if we're already colliding with controller
        if( collidingWithOtherController ) return;

        // check if other is the other controller: then we are colliding with controller
        ControllerSlingshot maybeController = other.GetComponent<ControllerSlingshot>();
        if( maybeController != null )
        {
            otherController = maybeController;
            collidingWithOtherController = true;
        }
    }

    private void OnTriggerStay( Collider other )
    {
        // repeat functionality of entering while currently within collider
        // (just in case we finished a prior action while already within)
        OnTriggerEnter( other );
    }

    private void OnTriggerExit( Collider other )
    {
        collidingWithOtherController = false;
        otherController = null;
    }

    private Vector3 GetControllerTipLocation()
    {
        return transform.position + ( transform.rotation * controllerOffset );
    }
}

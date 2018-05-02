using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour {
    // see http://fusedvr.com/building-the-labs-slingshot-from-scratch/

    public Transform projectilePrefab;
    public Transform sling;
    public Transform forwardSling;
    public float strength = 10f;
    private Vector3 slingStart;
    private Vector3 controllerStart;
    private Vector3 projectileStart;
    private Transform currentProjectile;
    private LineRenderer currentProjectileRenderer;
    private LaserArc currentProjectileLaser;

    private bool ready = true;
    private SteamVR_TrackedObject trackedController;
    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedController.index); }
    }
    private bool inSlingshot = false;

	// Use this for initialization
	void Start()
    {
	    slingStart = sling.position;	
	}
	
	// Update is called once per frame
	void Update()
    {
		if( ready )
        {
            currentProjectile = Instantiate( projectilePrefab, sling );
            // set it to be in the sling
            currentProjectile.localPosition = -0.005f * Vector3.up; 
            projectileStart = currentProjectile.position;
            // store
            currentProjectileRenderer = currentProjectile.GetComponent<LineRenderer>();
            currentProjectileLaser = currentProjectile.GetComponent<LaserArc>();
            // no longer ready
            ready = false;
        }

        // I took this code from a tutorial and modified it slightly
        // but it's not really how I would do it :|
        if( inSlingshot )
        {
            // when press trigger down, record where the controller started
            if( Controller.GetPressDown( SteamVR_Controller.ButtonMask.Trigger ) )
            {
                controllerStart = trackedController.transform.position;
                // show (TODO do I need to wait one frame before showing?)
                currentProjectileRenderer.enabled = true;
            }

            // when holding trigger down, move the slingshot by how much the controller has moved
            // and show the projectile arc
            if( Controller.GetPress( SteamVR_Controller.ButtonMask.Trigger ) )
            {
                sling.position = slingStart + trackedController.transform.position - controllerStart;
                currentProjectile.LookAt( projectileStart );
                Vector3 velocity = strength * ( slingStart - sling.position );
                currentProjectileLaser.SetVelocity( velocity );
            }

            // when release the trigger, launch the particle
            if( Controller.GetPressUp( SteamVR_Controller.ButtonMask.Trigger ) )
            {
                // launch projectile
                // delete the components for showing the laser arc
                Destroy( currentProjectileLaser );
                Destroy( currentProjectileRenderer );
                // unparent it -- be free!
                currentProjectile.transform.parent = null;
                // set its position back to where it was, because the band is going to snap back to there
                currentProjectile.position = projectileStart; 
                // rigidbody: make it a gravity-obeying non kinematic projectile
                Rigidbody rb = currentProjectile.GetComponent<Rigidbody>();
                rb.useGravity = true;
                rb.isKinematic = false;
                // velocity in direction of pull back
                Vector3 velocity = strength * ( slingStart - sling.position );
                rb.AddForce( velocity, ForceMode.VelocityChange );
                // no longer a trigger: collide away!
                currentProjectile.GetComponent<Collider>().isTrigger = false;
                // get a new projectile next update frame
                ready = true;
                
                ReleaseSling();
                sling.position = slingStart;
            }
        }

	}

    private void OnTriggerEnter(Collider other)
    {
        SteamVR_TrackedObject newController = other.GetComponent<SteamVR_TrackedObject>();
        if( trackedController == null && newController != null )
        {
            trackedController = newController;
            inSlingshot = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if( trackedController == other.GetComponent<SteamVR_TrackedObject>() )
        {
            ReleaseSling();
        }
    }

    private void ReleaseSling()
    {
        inSlingshot = false;
        trackedController = null;
    }
}

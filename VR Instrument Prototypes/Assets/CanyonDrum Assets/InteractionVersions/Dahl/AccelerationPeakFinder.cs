using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelerationPeakFinder : MonoBehaviour {
    
    // VR controller preamble
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    private void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    // Controller input: select my drum
    private LaserPointer myLaserPointer;
    private PlayBongo myBongo = null;
    void Start()
    {
	    myLaserPointer = GetComponent<LaserPointer>();
        // need to press the button to show the laser
        myLaserPointer.laserOnTouchpadButtonPress = true;
	}
	
	void LateUpdate () {
	    if( Controller.GetPressUp( SteamVR_Controller.ButtonMask.Touchpad ) )
        {
            Collider collidingObject = myLaserPointer.GetFoundCollider();
            if( collidingObject != null )
            {
                Debug.Log( collidingObject.gameObject.name );
                // check for PlayBongo and play it if possible
                PlayBongo bongo = collidingObject.GetComponentInParent<PlayBongo>();
                myBongo = bongo;  // null or not
            }
        }
	}


    // Check for acceleration hits and play the bongo!
    private void Update()
    {
        // short-circuit if have no bongo
        if( myBongo == null ) { return; }

        if( CheckForHit() )
        {
            Debug.Log( "hit with acceleration " + prevAcceleration.magnitude );
            // map roughly to 0,1 and play
            float intensity = Mathf.Clamp01( velocity.magnitude / 2.5f );
            intensity = Mathf.Clamp01( prevAcceleration.magnitude / 0.5f );
            Vector3 location = 0.5f * Vector3.forward;
            if( amLeftHand ) { location *= -1; }
            myBongo.PlayLocalLocation( intensity, location );
        }
    }

    public float accelerationThreshold = 0.5f;
	public float debounceTime = 0.166666667f;
    public bool amLeftHand;

    private Vector3 velocity = Vector3.zero;
    private Vector3 acceleration = Vector3.zero;
    private Vector3 jerk = Vector3.zero;
    private float magnitudeJerk = 0f;
    private Vector3 prevVelocity = Vector3.zero;
    private Vector3 prevAcceleration = Vector3.zero;
    private Vector3 prevJerk = Vector3.zero;
    private float prevMagnitudeJerk = 0f;
    private float prevHitTime = float.NegativeInfinity;
	// Update is called once per frame
	private bool CheckForHit()
    {
        bool hit = false;

	    velocity = Controller.velocity;
        acceleration = velocity - prevVelocity;
        jerk = acceleration - prevAcceleration;
        magnitudeJerk = acceleration.magnitude - prevAcceleration.magnitude;

        // peak when:
        // (1) Acceleration magnitude above a threshold
        // (2) Acceleration magnitude was increasing last time (the top of a peak)
        // (3) Acceleration magnitude is decreasing this time (crested the peak)
        // (4) Velocity is downward (45 degree cone)
        // (5) We've waited beyond our debounce time
        if( ( acceleration.magnitude >= accelerationThreshold || prevAcceleration.magnitude >= accelerationThreshold ) 
            && prevMagnitudeJerk > 0
            && magnitudeJerk < 0
            && WithinCone( velocity, Vector3.down, 45 )
            && (Time.time - prevHitTime) > debounceTime )
        {
            hit = true;
            prevHitTime = Time.time;
        }


        // bookkeeping
        prevVelocity = velocity;
        prevAcceleration = acceleration;
        prevJerk = jerk;
        prevMagnitudeJerk = magnitudeJerk;

        // was there a hit?
        return hit;
	}

    private bool WithinCone( Vector3 vectorToTest, Vector3 coneBasis, float coneAngle )
    {
        float coneThreshold = Mathf.Cos( coneAngle * Mathf.Deg2Rad );
        return ( Vector3.Dot( coneBasis, vectorToTest.normalized ) > coneThreshold );
    }
}

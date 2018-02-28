using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MalletSelector : MonoBehaviour , ColliderBridgeListener
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
    }



    // todo: refactor to private + selector
    public Rigidbody myMallet;
    public Transform myMalletPosition;


    private void Start()
    {
        // set up link to receive haptic feedback on collision
        myMallet.GetComponent<ColliderBridgeSource>().Initialize( this );
    }

    private void FixedUpdate()
    {
        // 1. Set velocity equivalent. Too insensitive but works a tiny amount!
        // myMallet.velocity = Controller.velocity;
        
        // 2. Set velocity much bigger. Worked sort of but got out of sync if I moved to wildly! 
        // Mallet movement needs to be restricted more. 
        // NOW mostly works, just feels a bit out of sync.
        // THIS ONE WORKS THE BEST.
        myMallet.velocity = 200 * Controller.velocity;
        
        // 3. Add force in the direction of velocity. Doesn't appear to work.
        // myMallet.AddForce( Controller.velocity );
        
        // 4. Add lots of force.... Feels impossible to control. Makes it go up more than down.
        // myMallet.AddForce( Controller.velocity * 100 );
        // same:
        // myMallet.AddForceAtPosition( Controller.velocity * 100, myMalletPosition.position );
        
        // 5. Angular velocity? Wrong direction and a bit too slow.
        // myMallet.angularVelocity = Controller.angularVelocity;

        // 6. Angular velocity opposite and larger?
        // myMallet.angularVelocity = -20 * Controller.angularVelocity;
    }

    void ColliderBridgeListener.BridgeCollisionEnter( Collision collision )
    {
        // on a collision, cause a haptic feedback pulse
        // NOTE: this was consistently arriving before the audio broadcast and so
        // I turned down the buffer size to reduce latency

        // Longer feedback for stronger collision?
        int minFeedback = 50;
        int maxFeedback = 50000;
        ushort hapticDuration = (ushort) Mathf.Lerp( minFeedback, maxFeedback, 
            Mathf.Clamp01( collision.relativeVelocity.magnitude / 500 ) );
        Controller.TriggerHapticPulse( durationMicroSec: hapticDuration );
    }

    // don't care about these
    void ColliderBridgeListener.BridgeCollisionStay( Collision collision ) {}
    void ColliderBridgeListener.BridgeCollisionExit( Collision collision ) {}
    void ColliderBridgeListener.BridgeTriggerEnter( Collider other ) {}
    void ColliderBridgeListener.BridgeTriggerStay( Collider other ) {}
    void ColliderBridgeListener.BridgeTriggerExit( Collider other ) {}
}

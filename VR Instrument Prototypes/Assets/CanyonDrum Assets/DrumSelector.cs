using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumSelector : MonoBehaviour {

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

    [Tooltip("If checked, use touchpad clicks. If unchecked, use trigger presses.")]
    public bool useTouchpad = true;
    private LaserPointer myLaserPointer;

    // Use this for initialization
    void Start()
    {
	    myLaserPointer = GetComponent<LaserPointer>();
        // don't need to press the button to show the laser
        myLaserPointer.laserOnTouchpadButtonPress = false;
	}
	
	void LateUpdate () {
	    if( ( useTouchpad && Controller.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad ) )
            || ( !useTouchpad && Controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger ) ) )
        {
            Collider collidingObject = myLaserPointer.GetFoundCollider();
            if( collidingObject != null )
            {
                Debug.Log( collidingObject.gameObject.name );
                // check for PlayBongo and play it if possible
                PlayBongo bongo = collidingObject.GetComponentInParent<PlayBongo>();
                if( bongo != null )
                {
                    bongo.Play( myLaserPointer.GetHitPoint() );
                }
            }
        }
	}
}

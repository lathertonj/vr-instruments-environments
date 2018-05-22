using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerOneHandedSlingshot : MonoBehaviour
{

    // VR preamble
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input( (int) trackedObj.index ); }
    }

    private void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    // =====================================================================

    public EndpointConnector bandPrefab;
    public string pullbackSound = "";


    private bool collidingWithSlingshotable = false;
    private bool pullingBack = false;
    private IOneHandedSlingshotable slingshotable = null;
    private IOneHandedSlingshotable pullbackSlingshotable = null;
    private Transform slingshotableTransform = null; 
    private Transform pullbackSlingshotableTransform = null;
    private Vector3 pointPullbackStartedFrom;

    private Transform myLeftBandAnchor;
    private Transform myRightBandAnchor;
    private Transform myLeftBandStationaryAnchor;
    private Transform myRightBandStationaryAnchor;
    private EndpointConnector myLeftBand = null;
    private EndpointConnector myRightBand = null;

    private int currentClickAmount = 0;
    private float clickRate = 0.03f;



    // Use this for initialization
    void Start()
    {
        // create anchors for bands
        myLeftBandAnchor = AddControllerBandLocation( slingshotBandOffset, left: true );
        myRightBandAnchor = AddControllerBandLocation( slingshotBandOffset, left: false );

        myLeftBandAnchor.gameObject.name = "Left Band Anchor";
        myRightBandAnchor.gameObject.name = "Right Band Anchor";
    }

    // Update is called once per frame
    void Update()
    {
        if( collidingWithSlingshotable && !pullingBack && Controller.GetPressDown( SteamVR_Controller.ButtonMask.Trigger ) )
        {
            // initiate draw-back!
            pullingBack = true;
            pullbackSlingshotable = slingshotable;
            pullbackSlingshotableTransform = slingshotableTransform;

            // tell it to be grabbed!
            pullbackSlingshotable.BeGrabbed();

            // remember where the grab started
            pointPullbackStartedFrom = pullbackSlingshotableTransform.position;
            myLeftBandStationaryAnchor = AddWorldBandLocation( pointPullbackStartedFrom, slingshotBandOffset, left: true );
            myRightBandStationaryAnchor = AddWorldBandLocation( pointPullbackStartedFrom, slingshotBandOffset, left: false );

            // draw some slingshot model hooked onto the other controller
            myLeftBand = Instantiate( bandPrefab );
            myLeftBand.myStart = myLeftBandAnchor;
            myLeftBand.myEnd = myLeftBandStationaryAnchor;
            myRightBand = Instantiate( bandPrefab );
            myRightBand.myStart = myRightBandAnchor;
            myRightBand.myEnd = myRightBandStationaryAnchor;

            currentClickAmount = CalculateClickAmount();
        }

        if( pullingBack && Controller.GetPress( SteamVR_Controller.ButtonMask.Trigger ) )
        {
            // inform of position
            pullbackSlingshotable.BeMovedToward( GetControllerTipLocation() );

            // make noises for current click amount
            int newClickAmount = CalculateClickAmount();
            if( newClickAmount != currentClickAmount && pullbackSound != "" )
            {
                float bufRate = 1.0f + ( newClickAmount * 1.0f / 30 );
                TheChuck.instance.RunCode( string.Format( @"
                    SndBuf buf => dac;
                    me.dir() + ""{1}"" => buf.read;
                    {0} => buf.rate;
                
                    buf.length() / buf.rate() => now;

                ", bufRate, pullbackSound ) );
            }
            currentClickAmount = newClickAmount;
        }

        if( pullingBack && Controller.GetPressUp( SteamVR_Controller.ButtonMask.Trigger ) )
        {
            // launch!
            pullbackSlingshotable.BeLetGoOf( CalculateClickAmount() );

            // reset
            pullingBack = false;
            pullbackSlingshotable = null;
            pullbackSlingshotableTransform = null;

            // remove the slingshot model
            Destroy( myLeftBand.gameObject );
            Destroy( myRightBand.gameObject );

            // remove band locations
            Destroy( myLeftBandStationaryAnchor.gameObject );
            Destroy( myRightBandStationaryAnchor.gameObject );
        }
    }

    private void OnTriggerEnter( Collider other )
    {
        // do no checks if we're already colliding with slingshotable
        if( collidingWithSlingshotable ) return;

        // check if other is slingshotable
        IOneHandedSlingshotable maybeSlingshotable = (IOneHandedSlingshotable) other.GetComponent( typeof( IOneHandedSlingshotable ) );
        if( maybeSlingshotable != null )
        {
            slingshotable = maybeSlingshotable;
            slingshotableTransform = other.transform;
            collidingWithSlingshotable = true;
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
        IOneHandedSlingshotable maybeSlingshotable = (IOneHandedSlingshotable) other.GetComponent( typeof( IOneHandedSlingshotable ) );
        if( maybeSlingshotable == slingshotable )
        {
            collidingWithSlingshotable = false;
            slingshotable = null;
            slingshotableTransform = null;
        }
    }

    private Vector3 controllerOffset = new Vector3( 0, -0.04f + 0.05f, 0.09f );
    private Vector3 GetControllerTipLocation()
    {
        return transform.position + ( transform.rotation * controllerOffset );
    }

    private Vector3 slingshotBandOffset = new Vector3( 0.0507f, -0.0321f, -0.0113f );
    private Transform AddControllerBandLocation( Vector3 offset, bool left )
    {
        GameObject emptyChild = new GameObject();
        emptyChild.transform.parent = transform;
        offset.x = ( left ? 1 : -1 ) * Mathf.Abs( offset.x );
        emptyChild.transform.localPosition = transform.localRotation * offset;

        return emptyChild.transform;
    }

    private Transform AddWorldBandLocation( Vector3 position, Vector3 offset, bool left )
    {
        GameObject emptyObject = new GameObject();
        offset.x = ( left ? -1 : 1 ) * Mathf.Abs( offset.x );
        emptyObject.transform.position = position + offset;

        return emptyObject.transform;
    }

    private int CalculateClickAmount()
    {
        return (int) ( ( transform.position - pointPullbackStartedFrom ).magnitude / clickRate );
    }

}

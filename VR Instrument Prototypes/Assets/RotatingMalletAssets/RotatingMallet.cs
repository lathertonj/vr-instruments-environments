using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingMallet : MonoBehaviour , IOneHandedSlingshotable
{
    public Transform thingToRotate;

    private ChuckSubInstance myChuck;
    private ChuckFloatSyncer myRotationAmountSyncer;
    private string startRotating, stopRotating, playNote, currentRotation, rotationSpeedMS;
    private bool amRotating = false;

    // Use this for initialization
    void Start()
    {
        myChuck = GetComponent<ChuckSubInstance>();
        startRotating = myChuck.GetUniqueVariableName();
        stopRotating = myChuck.GetUniqueVariableName();
        playNote = myChuck.GetUniqueVariableName();
        currentRotation = myChuck.GetUniqueVariableName();
        rotationSpeedMS = myChuck.GetUniqueVariableName();

        myChuck.RunCode( string.Format( @"
            global Event {0}; // startRotating
            global Event {1}; // stopRotating
            global Event {2}; // playNote
            global float {3}; // currentRotation
            global float {4}; // rotationSpeedMS

            fun void Rotate()
            {{
                while( true )
                {{
                    0 => {3};
                    100 => float numIterations;
                    for( int i; i < numIterations; i++ )
                    {{
                        // currentRotation is the fraction we are currently on
                        i $ float / numIterations => {3};
                        // wait fraction of the rotation speed
                        {4}::ms / numIterations => now;
                    }}
                    {2}.broadcast();
                }}
            }}

            Shred rotateShred;
            while( true )
            {{
                {0} => now;
                spork ~ Rotate() @=> rotateShred;

                {1} => now;
                rotateShred.exit();
            }}
            
        ", startRotating, stopRotating, playNote, currentRotation, rotationSpeedMS ) );

        myChuck.RunCode( string.Format( @"
            global Event {0};

            fun void PlayNote()
            {{
                SndBuf buf => dac;
                me.dir() + ""bongo_hits/bongo1.wav"" => buf.read;
                buf.length() / buf.rate() => now;
                buf =< dac;
            }}

            while( true )
            {{
                {0} => now;
                spork ~ PlayNote();
            }}
        ", playNote ) );


        myChuck.SetFloat( rotationSpeedMS, 250 );


        myRotationAmountSyncer = gameObject.AddComponent<ChuckFloatSyncer>();
        myRotationAmountSyncer.SyncFloat( myChuck, currentRotation );
    }

    // TODO: BE ABLE TO RESET PHASE

    // Update is called once per frame
    void Update()
    {
        if( Input.GetKeyDown( "space" ) )
        {
            BeLetGoOf( 28 );
        }

        if( amRotating )
        {
            thingToRotate.rotation = Quaternion.AngleAxis( -90 + 360 * myRotationAmountSyncer.GetCurrentValue(), Vector3.left );
        }

    }

    public void BeLetGoOf( int numClicks )
    {
        // map num clicks to a rotation speed
        // 0 click = slowest.  24bpm. 
        // 30 click = fastest. 800bpm. 
        float minBPM = 24;
        float maxBPM = 800;
        float tempoBPM = Mathf.Clamp01( numClicks * 1.0f / 30 ) * ( maxBPM - minBPM ) + minBPM;
        float tempoMS = 60000f / tempoBPM;

        myChuck.SetFloat( rotationSpeedMS, tempoMS );
        myChuck.BroadcastEvent( startRotating );
        amRotating = true;
    }

    public void BeGrabbed()
    {
        myChuck.BroadcastEvent( stopRotating );
        amRotating = false;
    }

    public void BeMovedToward( Vector3 position )
    {
        // TODO: calculate the correct angle for me to be pointing at the position
        Vector3 localLookDirection = transform.InverseTransformPoint( position );
        // don't rotate in x
        localLookDirection.x = 0;

        thingToRotate.rotation = Quaternion.LookRotation( localLookDirection );
    }
}

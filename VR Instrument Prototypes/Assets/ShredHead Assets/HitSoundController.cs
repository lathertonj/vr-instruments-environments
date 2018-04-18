using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitSoundController : MonoBehaviour 
{
    private ChuckSubInstance myChuck;
    private Rigidbody myRB;
    private MeshRenderer myRenderer;
    private string myEndHitEvent;
    private string myAmplitudeNumber;
    public float myNoteNumber;
    private bool playingACollision = false;

	// Use this for initialization
	void Start()
    {
		myChuck = GetComponent<ChuckSubInstance>();
        myEndHitEvent = myChuck.GetUniqueVariableName();
        myAmplitudeNumber = myChuck.GetUniqueVariableName();

        myRB = GetComponent<Rigidbody>();
        myRenderer = GetComponentInChildren<MeshRenderer>();
	}

    private void OnCollisionEnter( Collision collision )
    {
        if( playingACollision ) { return; }
        if( !collision.rigidbody.gameObject.CompareTag( "Hammer" ) ) { return; }

        playingACollision = true;

        // do a chuck thing
        myChuck.RunCode( string.Format( @"
            external Event {0};
            1 => external float {1};

            TriOsc t => Gain g => dac;
            {2} => Math.mtof => t.freq;
            0.2 => g.gain; // lower volume to 20% overall
               
            fun void ControlVolume()
            {{
                while( true )
                {{
                    {1} => t.gain;
                    10::ms => now;
                }}
            }}
            spork ~ ControlVolume();

            {0} => now;
            t =< dac;
            
        ", myEndHitEvent, myAmplitudeNumber, myNoteNumber ));
    }

    void Update()
    {
		if( playingACollision )
        {
            float magnitude = Mathf.Clamp01( myRB.velocity.magnitude );
            myChuck.SetFloat( myAmplitudeNumber, magnitude );
            myRenderer.material.color = magnitude * Color.blue + ( 1 - magnitude ) * Color.white;
            if( magnitude < 0.01 )
            {
                myRenderer.material.color = Color.white;
                myChuck.BroadcastEvent( myEndHitEvent );
                playingACollision = false;
            }
        }
        else
        {
            // just in case! aggressively end things all the time
            // if we aren't currently playing a collision
            myChuck.BroadcastEvent( myEndHitEvent );
        }

	}
}

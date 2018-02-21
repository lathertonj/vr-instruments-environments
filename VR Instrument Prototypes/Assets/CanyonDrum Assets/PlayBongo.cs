using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayBongo : MonoBehaviour {

    private ChuckSubInstance myChuck;
    private string myBongoIntensity;
    private string myBongoPlayEvent;

	// Use this for initialization
	void Start()
    {
	    myChuck = GetComponent<ChuckSubInstance>();
        myBongoIntensity = myChuck.GetUniqueVariableName();
        myBongoPlayEvent = myChuck.GetUniqueVariableName();
        myChuck.RunCode( string.Format( @"
            NRev n => dac;
            0.03 => n.mix;
            
            fun void PlayHit( float intensity )
            {{
                SndBuf buf => n;
                me.dir() + ""bongo_hits/bongo1.wav"" => buf.read;
                intensity => buf.gain;
    
                0.5 - 0.25 * intensity => buf.rate;

                buf.length() / buf.rate() => now;

                buf =< n;
            }}

            external float {0};
            external Event {1};
   
            while( true )
            {{
                {1} => now;
                spork ~ PlayHit( {0} );
            }}
        
        ", myBongoIntensity, myBongoPlayEvent ) );
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter( Collision collision )
    {
        // set intensity then play impact
        myChuck.SetFloat( myBongoIntensity, collision.relativeVelocity.magnitude );
        myChuck.BroadcastEvent( myBongoPlayEvent );
    }
}

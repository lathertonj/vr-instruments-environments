using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayBongo : MonoBehaviour {

    // ChucK
    private ChuckSubInstance myChuck;
    private string myBongoIntensity;
    private string myBongoPlayEvent;

    // Unity
    private ParticleSystem myEmitter;

	// Use this for initialization
	void Start()
    {
        // ChucK 
	    myChuck = GetComponent<ChuckSubInstance>();
        myBongoIntensity = myChuck.GetUniqueVariableName();
        myBongoPlayEvent = myChuck.GetUniqueVariableName();
        myChuck.RunCode( string.Format( @"
            NRev n => dac;
            0.03 => n.mix;
//SinOsc foo => n;
            
            fun void PlayHit( float intensity )
            {{
                SndBuf buf => n;
                me.dir() + ""bongo_hits/bongo1.wav"" => buf.read;
                intensity * 3 => buf.gain;
//<<< intensity >>>;
    
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


        // Unity
        myEmitter = GetComponentInChildren<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    private void OnCollisionEnter( Collision collision )
    {
        Play( collision.relativeVelocity.magnitude, /*collision.collider.transform.position*/ // don't use collider position
            collision.contacts[0].point ); // use contact point
    }

    public void Play()
    {
        Play( this.transform.position );
    }

    public void Play( Vector3 location )
    {
        Play( Random.Range( 0.5f, 0.9f ), location );
    }
    
    private void Play( float intensity, Vector3 location )
    {
        // ChucK: set intensity then play impact
        Debug.Log("intensity: " + intensity.ToString() );
        myChuck.SetFloat( myBongoIntensity, Mathf.Min( 1.0f, intensity / 200.0f ) );
        myChuck.BroadcastEvent( myBongoPlayEvent );

        // Unity: emit particle

        // override position of particle
        ParticleSystem.EmitParams newParams = new ParticleSystem.EmitParams();
        // location relative to the location of the emitter
        newParams.position = myEmitter.transform.position - location ;

        // override velocity of particle
        newParams.velocity = new Vector3( 0.0f, 15.0f, 0.0f );

        myEmitter.Emit( newParams, 1 );
    }
}

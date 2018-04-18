using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingSoundController : MonoBehaviour
{
    private ChuckSubInstance myChuck;
    private string myVolumeLevel;
    private Rigidbody myRB;
    private MeshRenderer myRenderer;
    public float myNoteNumber;

	// Use this for initialization
	void Start()
    {
	    myChuck = GetComponent<ChuckSubInstance>();
        myVolumeLevel = myChuck.GetUniqueVariableName();
        myRB = GetComponent<Rigidbody>();
        myRenderer = GetComponentInChildren<MeshRenderer>();

        myChuck.RunCode( string.Format( @"
            TriOsc t => Gain g => dac;
            0.05 => g.gain;
            {1} => Math.mtof => t.freq;
            
            external float {0};
            float goalVolume;
            float actualVolume;
            0.1 => float volumeSlew;

            while( true )
            {{
                {0} => goalVolume;
                actualVolume + volumeSlew * ( goalVolume - actualVolume ) => actualVolume;
                actualVolume => t.gain;
                10::ms => now;
            }}
        ", myVolumeLevel, myNoteNumber ));
	}
	
	// Update is called once per frame
	void Update()
    {
        // floor = 0.06f
		float velocity = Mathf.Clamp01( myRB.velocity.magnitude - 0.06f );
        if( velocity > 0f )
        {
            myChuck.SetFloat( myVolumeLevel, velocity );
            Debug.Log( velocity );
            myRenderer.material.color = velocity * Color.cyan + ( 1 - velocity ) * Color.white;
        }
        else
        {
            myRenderer.material.color = Color.white;
            myChuck.SetFloat( myVolumeLevel, 0 );
        }
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluteEnvelopeFollower : MonoBehaviour {

    private ChuckSubInstance myChuck;
    public TwistAngleMeasurer myController;
    private FluteEndpointConnector myFluteBody;

	// Use this for initialization
	void Start()
    {
        myFluteBody = GetComponentInChildren<FluteEndpointConnector>();
		myChuck = GetComponent<ChuckSubInstance>();
        // Compute envelope
        myChuck.RunCode(@"
            // see smelt.cs.princeton.edu/code/microphone/follower.ck
            external float micLevel;
            1::ms => dur micLevelUpdateRate;
            
            // patch
            adc => Gain g => OnePole p => blackhole;
            // square the input, by chucking adc to g a second time
            adc => g;
            // set g to multiply its inputs
            3 => g.op;

            // threshold
            .05 => float threshold;
            // set pole position, influences how closely the envelope follows the input
            //   : pole = 0 -> output == input; 
            //   : as pole position approaches 1, follower will respond more slowly to input
            0.999 => p.pole;

            // infinite time loop
            while( true )
            {
                p.last() => micLevel;
                micLevelUpdateRate => now;
            }
        ");

        myChuck.RunCode(@"
            external float micLevel;
            external float flutePitch;
            external float fluteWarpedness;
            TriOsc t => dac;
            0 => t.gain;

            // lag behind the envelope follower by 1 samp
            1::samp => now;

            float currentGain;
            float goalGain;
            0.01 => float gainSlewDown;
            0.1 => float gainSlewUp;

            while( true )
            {
                micLevel * 20 => goalGain;
                if( currentGain < goalGain )
                {
                    currentGain + gainSlewUp * ( goalGain - currentGain ) => currentGain;
                }
                else
                {
                    currentGain + gainSlewDown * ( goalGain - currentGain ) => currentGain;
                }


                currentGain => t.gain;
                flutePitch => Math.mtof => t.freq;
                1::ms => now;
            }
        ");
	}
	
	// Update is called once per frame
	void Update()
    {
        // Midi note 40-100 based on distance between hands, capped at 2
        float maxSize = 2;
        float pitch = 100 - 60 * Mathf.Clamp01( myFluteBody.GetMySize() / maxSize );
        Debug.Log( pitch );
		myChuck.SetFloat( "flutePitch", pitch );
	}
}

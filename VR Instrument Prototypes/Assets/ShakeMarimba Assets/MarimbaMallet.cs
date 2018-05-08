﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarimbaMallet : MonoBehaviour
{
    // VR preamble
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    private void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    // =====================================================================

    ChuckSubInstance myChuck;
    string myLisa;
    public int myDifferentiationNum;
    private string
        spawnRate,
        spawnRateVariation,
        spawnRateVariationRate,
        grainPosition,
        grainPositionRandomness,
        grainLength,
        grainLengthRandomness,
        grainRate,
        grainRateRandomness,
        rampUp,
        rampDown,
        lisaLoudness;

    // Use this for initialization
    void Start()
    {
        myChuck = GetComponent<ChuckSubInstance>();
        myLisa = myChuck.GetUniqueVariableName();

        spawnRate = "grainSpawnRateMS" + myDifferentiationNum;
        spawnRateVariation = "grainSpawnRateVariationMS" + myDifferentiationNum;
        spawnRateVariationRate = "grainSpawnRateVariationRateHZ" + myDifferentiationNum;
        grainPosition = "grainPosition" + myDifferentiationNum;
        grainPositionRandomness = "grainPositionRandomness" + myDifferentiationNum;
        grainLength = "grainLengthMS" + myDifferentiationNum;
        grainLengthRandomness = "grainLengthRandomnessMS" + myDifferentiationNum;
        grainRate = "grainRate" + myDifferentiationNum;
        grainRateRandomness = "grainRateRandomness" + myDifferentiationNum;
        rampUp = "rampUpMS" + myDifferentiationNum;
        rampDown = "rampDownMS" + myDifferentiationNum;
        lisaLoudness = "gainMultiplier" + myDifferentiationNum;

        myChuck.RunCode( string.Format( @"
            //-----------------------------------------------------------------------------
            // name: LiSa-Sndbuf2.ck
            // desc: Live sampling utilities for ChucK
            //
            // author: Jack Atherton
            //
            // Combining Dan Trueman's various helper scripts for Lisa
            // https://github.com/ccrma/music220a/blob/master/chuck-examples/special/LiSa-SndBuf.ck
            // http://chuck.cs.princeton.edu/doc/examples/special/LiSa-munger2.ck
            //-----------------------------------------------------------------------------

            // PARAMS

            // spawn rate: how often a new grain is spawned (ms)
            25 => global float grainSpawnRateMS{0};
            0.0 => global float grainSpawnRateVariationMS{0};
            0.1 => global float grainSpawnRateVariationRateHZ{0};

            // position: where in the file is a grain (0 to 1)
            0.66 => global float grainPosition{0};
            0.1 => global float grainPositionRandomness{0};

            // grain length: how long is a grain (ms)
            300 => global float grainLengthMS{0};
            100 => global float grainLengthRandomnessMS{0};

            // grain rate: how quickly is the grain scanning through the file
            1.7 => global float grainRate{0};
            0.6 => global float grainRateRandomness{0};

            // ramp up/down: how quickly we ramp up / down
            2 => global float rampUpMS{0};
            10 => global float rampDownMS{0};

            // gain: how loud is everything overall
            1 => global float gainMultiplier{0};

            global Event longFadeOut{0};



            LiSa {1} => Gain fadeoutGain => dac;

            // LiSa params
            100 => {1}.maxVoices;
            0.1 => {1}.gain;
            true => {1}.loop;
            false => {1}.record;


            // modulate
            SinOsc freqmod => blackhole;
            0.1 => freqmod.freq;



            0.5 => float maxGain;

            fun void SetGain()
            {{
                while( true )
                {{
                    maxGain * gainMultiplier{0} => {1}.gain;
                    1::ms => now;
                }}
            }}
            spork ~ SetGain();


            2.7::second => dur fadeOutTime;

            fun void DoFadeOuts()
            {{
                while( true )
                {{
                    longFadeOut{0} => now;
                    ( fadeOutTime / 1::ms ) $ int => float numSteps;
                    for( int i; i < numSteps; i++ )
                    {{
                        1 - ( i / numSteps ) => fadeoutGain.gain;
                        1::ms => now;
                    }}
                    0 => gainMultiplier{0};
                    1 => fadeoutGain.gain;
                }}
            }}
            spork ~ DoFadeOuts();


            // create grains
            while( true )
            {{
                // grain length
                ( grainLengthMS{0} + Math.random2f( -grainLengthRandomnessMS{0} / 2, grainLengthRandomnessMS{0} / 2 ) )
                    * 1::ms => dur grainLength;

                // grain rate
                grainRate{0} + Math.random2f( -grainRateRandomness{0} / 2, grainRateRandomness{0} / 2 ) => float grainRate;
    
                // grain position
                ( grainPosition{0} + Math.random2f( -grainPositionRandomness{0} / 2, grainPositionRandomness{0} / 2 ) )
                    * {1}.duration() => dur playPos;
    
                // grain: grainlen, rampup, rampdown, rate, playPos
                spork ~ PlayGrain( grainLength, rampUpMS{0}::ms, rampDownMS{0}::ms, grainRate, playPos );

                // advance time (time per grain)
                // PARAM: GRAIN SPAWN RATE
                grainSpawnRateMS{0}::ms  + freqmod.last() * grainSpawnRateVariationMS{0}::ms => now;
                grainSpawnRateVariationRateHZ{0} => freqmod.freq;
            }}


            // sporkee
            fun void PlayGrain( dur grainlen, dur rampup, dur rampdown, float rate, dur playPos )
            {{
                {1}.getVoice() => int newvoice;

                if(newvoice > -1)
                {{
                    {1}.rate( newvoice, rate );
                    {1}.playPos( newvoice, playPos );
                    {1}.rampUp( newvoice, rampup );
                    ( grainlen - ( rampup + rampdown ) ) => now;
                    {1}.rampDown( newvoice, rampdown );
                    rampdown => now;
                }}
            }}




        ", myDifferentiationNum, myLisa ) );
    }

    float hitNote = 0;
    // Update is called once per frame
    void Update()
    {
        if( hitNote > 0 )
        {

        }
    }

    private void OnTriggerEnter( Collider other )
    {
        NoteNumber maybeN = other.GetComponent<NoteNumber>();
        if( maybeN != null )
        {
            hitNote = maybeN.myNote;
            myChuck.RunCode( string.Format( @"
                global LiSa {1};
                SndBuf buf => dac;
                me.dir() + ""impact.wav"" => buf.read;
                Math.pow( 2, 1.0 / 12 ) => float twelfthRootOfTwo;
                Math.pow( twelfthRootOfTwo, {0} - 60 ) => buf.rate;

                now + buf.length() / buf.rate() => time endTime;
                0 => int i;
                
                // turn lisa off
                0 => {1}.gain;

                // reset lisa length
                buf.length() / buf.rate() => {1}.duration;

                // play sound file and copy it into lisa
                while( now < endTime )
                {{
                    1::samp => now;
                    {1}.valueAt( buf.last(), i::samp );
                    i++;
                }}

                // turn lisa back on
                1 => {1}.gain;
            ", hitNote, myLisa ) );

            GetComponent<HapticFeedback>().TriggerHapticFeedback( intensity: 1, timeDelay: 0.05f );
        }
    }
}

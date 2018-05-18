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
25 => global float grainSpawnRateMS;
0.0 => global float grainSpawnRateVariationMS;
0.1 => global float grainSpawnRateVariationRateMS;

// position: where in the file is a grain (0 to 1)
0.66 => global float grainPosition;
0.1 => global float grainPositionRandomness;

// grain length: how long is a grain (ms)
300 => global float grainLengthMS;
100 => global float grainLengthRandomnessMS;

// grain rate: how quickly is the grain scanning through the file
1.7 => global float grainRate;
0.6 => global float grainRateRandomness;

// ramp up/down: how quickly we ramp up / down
2 => global float rampUpMS;
10 => global float rampDownMS;

// gain: how loud is everything overall
0 => global float gainMultiplier;

global Event longFadeOut;



LiSa lisa => Gain fadeoutGain => dac;
SndBuf buf; 
me.dir() + me.arg(0) => buf.read;
buf.length() => lisa.duration;
// copy samples in
for( int i; i < buf.samples(); i++ )
{
    lisa.valueAt( buf.valueAt( i ), i::samp );
}


buf.length() => dur bufferlen;

// LiSa params
100 => lisa.maxVoices;
0.1 => lisa.gain;
true => lisa.loop;
false => lisa.record;


// modulate
SinOsc freqmod => blackhole;
0.1 => freqmod.freq;



0.5 => float maxGain;

fun void SetGain()
{
    while( true )
    {
        maxGain * gainMultiplier => lisa.gain;
        1::ms => now;
    }
}
spork ~ SetGain();


2.7::second => dur fadeOutTime;

fun void DoFadeOuts()
{
    while( true )
    {
        longFadeOut => now;
        ( fadeOutTime / 1::ms ) $ int => float numSteps;
        for( int i; i < numSteps; i++ )
        {
            1 - ( i / numSteps ) => fadeoutGain.gain;
            1::ms => now;
        }
        0 => gainMultiplier;
        1 => fadeoutGain.gain;
    }
}
spork ~ DoFadeOuts();


// create grains
while( true )
{
    // grain length
    ( grainLengthMS + Math.random2f( -grainLengthRandomnessMS / 2, grainLengthRandomnessMS / 2 ) )
        * 1::ms => dur grainLength;

    // grain rate
    grainRate + Math.random2f( -grainRateRandomness / 2, grainRateRandomness / 2 ) => float grainRate;
    
    // grain position
    ( grainPosition + Math.random2f( -grainPositionRandomness / 2, grainPositionRandomness / 2 ) )
        * bufferlen => dur playPos;
    
    // grain: grainlen, rampup, rampdown, rate, playPos
    spork ~ PlayGrain( grainLength, rampUpMS::ms, rampDownMS::ms, grainRate, playPos);

    // advance time (time per grain)
    // PARAM: GRAIN SPAWN RATE
    grainSpawnRateMS::ms  + freqmod.last() * grainSpawnRateVariationMS::ms => now;
    grainSpawnRateVariationRateMS => freqmod.freq;
}


// sporkee
fun void PlayGrain( dur grainlen, dur rampup, dur rampdown, float rate, dur playPos )
{
    lisa.getVoice() => int newvoice;

    if(newvoice > -1)
    {
        lisa.rate( newvoice, rate );
        lisa.playPos( newvoice, playPos );
        lisa.rampUp( newvoice, rampup );
        ( grainlen - ( rampup + rampdown ) ) => now;
        lisa.rampDown( newvoice, rampdown) ;
        rampdown => now;
    }
}




using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountBoxEntrances : MonoBehaviour {

    public BoxEntranceResponder boxToTell;
    public int minEntrances;
    public int maxEntrances;
    private int numEntrances;
    private bool updateGranular = true;

    // names of external variables in the script
    private string 
        spawnRate = "grainSpawnRateMS", 
        spawnRateVariation = "grainSpawnRateVariationMS", 
        spawnRateVariationRate = "grainSpawnRateVariationRateMS",
        grainPosition = "grainPosition", 
        grainPositionRandomness = "grainPositionRandomness", 
        grainLength = "grainLengthMS", 
        grainLengthRandomness = "grainLengthRandomnessMS",
        grainRate = "grainRate", 
        grainRateRandomness = "grainRateRandomness",
        rampUp = "rampUpMS", 
        rampDown = "rampDownMS",
        lisaLoudness = "gainMultiplier";

    private void Start()
    {
        TheChuck.instance.RunFile( "LiSa-Sndbuf2", "whee.wav" );
        
        // defaults
        TheChuck.instance.SetFloat( spawnRate, 25 );
        TheChuck.instance.SetFloat( spawnRateVariation, 7 );
        TheChuck.instance.SetFloat( spawnRateVariationRate, 0.1 );

        TheChuck.instance.SetFloat( grainPosition, 0.7 ); // 0.7 to 0.1
        TheChuck.instance.SetFloat( grainPositionRandomness, 0.05 ); // 0.05 to 0.35

        TheChuck.instance.SetFloat( grainLength, 80 ); // 80 to 300
        TheChuck.instance.SetFloat( grainLengthRandomness, 50 );

        TheChuck.instance.SetFloat( grainRate, 1.7 );
        TheChuck.instance.SetFloat( grainRateRandomness, 0.1 ); // 0.1 to 0.6

        TheChuck.instance.SetFloat( rampUp, 2 );
        TheChuck.instance.SetFloat( rampDown, 10 );

        TheChuck.instance.SetFloat( lisaLoudness, 0 ); // 0 to 1

        ResetGranular();
    }

    private void UpdateLisaParams( int numEntered, int max )
    {
        float fraction = numEntered * 1f / max;

        TheChuck.instance.SetFloat( grainPosition, Mathf.Lerp( 0.7f, 0.1f, fraction ) );
        TheChuck.instance.SetFloat( grainPositionRandomness, Mathf.Lerp( 0.05f, 0.35f, fraction ) );
        TheChuck.instance.SetFloat( grainLength, Mathf.Lerp( 80f, 300f, fraction ) );
        TheChuck.instance.SetFloat( grainRateRandomness, Mathf.Lerp( 0.1f, 0.6f, fraction ) );
        TheChuck.instance.SetFloat( lisaLoudness, Mathf.Clamp01( fraction ) );
    }

    private void OnTriggerEnter( Collider other )
    {
        numEntrances++;
        int numEntrancesToWaitBeforeSound = 3;
        if( updateGranular && numEntrances > numEntrancesToWaitBeforeSound )
        {
            UpdateLisaParams( numEntrances - numEntrancesToWaitBeforeSound, maxEntrances );
        }

        if( numEntrances >= maxEntrances )
        {
            InformAndReset();
            
        }
        else if( numEntrances >= minEntrances )
        {
            float probOfInform = ( numEntrances - minEntrances ) * 1.0f / ( maxEntrances - minEntrances );
            if( Random.Range( 0f, 1f ) <= probOfInform )
            {
                InformAndReset();
            }
        }
    }

    private void InformAndReset()
    {
        boxToTell.RespondToBoxEntrances();
        numEntrances = 0;
        // sound 
        // immediately ramp up to max
        UpdateLisaParams( maxEntrances, maxEntrances );
        // do a long fade out
        TheChuck.instance.BroadcastEvent( "longFadeOut" );
        // ignore UpdateLisaParams() for now
        updateGranular = false;
        // stop ignoring 3 sconds later
        Invoke( "ResetGranular", 3 ); // sync with the length of fadeout in the file
    }

    private void ResetGranular()
    {
        updateGranular = true;
        UpdateLisaParams( 0, maxEntrances );
    }

    
}

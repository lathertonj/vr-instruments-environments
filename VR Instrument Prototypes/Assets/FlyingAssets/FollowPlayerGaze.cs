using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerGaze : MonoBehaviour {

    public Transform player;
    public Transform playerHand1;
    public Transform playerHand2;
    public float maxSpeed = 5;

    public static float currentSpeed = 0;

    private void Start()
    {
        //StartNeow();
        StartWhoosh();
    }

    void StartWhoosh()
    {
        TheChuck.instance.RunCode( @"
            Noise n => LPF f => HPF hf => dac;
            0.05 => hf.gain;
            SinOsc lfo => blackhole;
            1200 => hf.freq;

            500 => external float baseWhooshLPFFreq;
            8 => external float lfoWhooshRate;

            while( true )
            {
                Math.max( baseWhooshLPFFreq + 200 * lfo.last(), 100 ) => f.freq;
                lfoWhooshRate => lfo.freq;
                1::ms => now;
            }
        ");
    }

    void UpdateWhoosh()
    {
        float speedAmount = currentSpeed / maxSpeed;
        // 320 - 1000
        TheChuck.instance.SetFloat( "baseWhooshLPFFreq", 320 + speedAmount * (1000 - 320) );
        // 0.3 - 8
        // or: 0.3-1
        TheChuck.instance.SetFloat( "lfoWhooshRate", 0.3 + speedAmount * 0.7 );
    }

    void StartNeow()
    {
        // play neowwww
        TheChuck.instance.RunCode(@"
            SndBuf neowBuf => dac;
            me.dir() + ""neow.wav"" => neowBuf.read;
            1 => external float neowRate;

            fun void ApplyNeowRate()
            {
                while( true )
                {
                    neowRate => neowBuf.rate;
                    1::ms => now;
                }
            }
            spork ~ ApplyNeowRate();

            while( true )
            {
                // play
                0 => neowBuf.pos;
                // wait
                Math.random2f( 1.5, 1.8 )::second => now;
            }

        ");
    }

    void UpdateNeowRate()
    {
        // lowest speed: 0.5x
        // highest speed: 1.5x
        TheChuck.instance.SetFloat( "neowRate", 0.5 + currentSpeed / maxSpeed );
    }

    // Update is called once per frame
    void Update()
    {
        // sfx
        //UpdateNeowRate();
        UpdateWhoosh();

        // movement
        // idea 1: fly in direction of gaze (including up/down)
        // MoveInGazeDirection();
        MoveInGazeDirection2();
		
        // idea 2: fly in direction of gaze, without moving up/down
        // ConstantHeight();

        // idea 3: fly in direction of gaze, and remain a fixed height above the ground
        // UpdateHeightByLandHeight();

	}

    private void MoveInGazeDirection()
    {
        // idea 1: fly in direction of gaze (including up/down)
        Vector3 updatePosition = maxSpeed * Time.deltaTime * player.forward;
        transform.position += updatePosition;
    }

    private void MoveInGazeDirection2()
    {
        // idea 1b: tuned flying in direction of gaze.
        // first: speed determined by outstretched hands. (TODO)
        float speedMultiplier = Mathf.Clamp01( (
            DistanceToHead( playerHand1.position ) 
            + DistanceToHead( playerHand2.position )
        ) * 1.5f ); // clamp faster so I don't have to reach out as far
        currentSpeed = speedMultiplier * maxSpeed;
        Vector3 updatePosition = currentSpeed * Time.deltaTime * player.forward;
        transform.position += updatePosition;

        // second: enforce min distance 1m above ground.
        // TODO: make it seem a little less "thunk"y
        // TODO: bug where if you go straight at the ground, you'll get shot upwards...
        float groundHeight = FindVerticalIntersectionPoint( player.position + 20 * Vector3.up );
        
        // min head height: feels weird when you're at minimum, 
        // you try to bend down, and the ground moves away from you.
        /*float minHeadHeight = groundHeight + 1f;
        if( player.position.y < minHeadHeight )
        {
            transform.position += ( minHeadHeight - player.position.y ) * Vector3.up;
        }*/

        // instead, allow user to be "knee deep" up to 0.6f
        float minFloorLevel = groundHeight - 0.6f;
        if( transform.position.y < minFloorLevel )
        {
            transform.position = new Vector3( 
                transform.position.x,
                minFloorLevel,
                transform.position.z
            );
        }
    }

    private void ConstantHeight()
    {     
        // idea 2: fly in direction of gaze, without moving up/down
        Vector3 updatePosition = maxSpeed * Time.deltaTime * player.forward;
        updatePosition.y = 0;
        transform.position += updatePosition;
    }

    private void UpdateHeightByLandHeight()
    {
        // idea 3: fly in direction of gaze, and remain a fixed height above the ground
        Vector3 updatePosition = maxSpeed * Time.deltaTime * player.forward;
		
        float heightAboveGround = 1;
        updatePosition.y = (
            FindVerticalIntersectionPoint( transform.position + updatePosition ) 
            + heightAboveGround 
        ) - transform.position.y;

        transform.position += updatePosition;
    }
    
    
    private float FindVerticalIntersectionPoint( Vector3 pointToLookFrom )
    {
        int terrainMask = 1 << LayerMask.NameToLayer( "Terrain" );
        RaycastHit hit;
        if( Physics.Raycast( pointToLookFrom, Vector3.down, out hit, maxDistance: Mathf.Infinity, layerMask: terrainMask ) )
        {
            return hit.point.y;
        }
        return 0;
    }

    private float DistanceToHead( Vector3 position )
    {
        Vector3 difference = player.position - position;
        // ignore vertical difference
        difference.y = 0; 
        return difference.magnitude;
    }
}

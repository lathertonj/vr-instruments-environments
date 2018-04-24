using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerGaze : MonoBehaviour {

    public Transform player;
    public Transform playerHand1;
    public Transform playerHand2;
    public float maxSpeed = 5;

	// Update is called once per frame
	void Update()
    {
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
        Vector3 updatePosition = speedMultiplier * maxSpeed * Time.deltaTime * player.forward;
        transform.position += updatePosition;

        // second: enforce min distance 1m above ground.
        // TODO: make it seem a little less "thunk"y
        // TODO: bug where if you go straight at the ground, you'll get shot upwards...
        float groundHeight = FindVerticalIntersectionPoint( player.position + 20 * Vector3.up );
        float minHeadHeight = groundHeight + 1f;
        if( player.position.y < minHeadHeight )
        {
            transform.position += ( minHeadHeight - player.position.y ) * Vector3.up;
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

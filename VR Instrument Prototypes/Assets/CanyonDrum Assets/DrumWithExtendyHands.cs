using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumWithExtendyHands : MonoBehaviour {

    public bool isLeftHand = true;
    public Transform handPrefab;
    public Transform myHead;
    public float controllerSpeedFactor = 2;

    private Transform myHand;

	// Use this for initialization
	void Start()
    {
	    myHand = Instantiate( handPrefab, this.transform.position, Quaternion.AngleAxis( -90, Vector3.up ), this.transform );
        if( isLeftHand )
        {
            Vector3 scale = myHand.localScale;
            scale.x = -1;
            myHand.localScale = scale;
            Vector3 boxColliderSize = myHand.GetComponent<BoxCollider>().size;
            boxColliderSize.x *= -1;
            myHand.GetComponent<BoxCollider>().size = boxColliderSize;
        }
	}
	
	// Update is called once per frame
	void Update()
    {
		// set z according to a smooth function mapping (head-->controller) --> (controller-->hand)
        Vector2 head = GetXZCoords( myHead );
        Vector2 controller = GetXZCoords( transform );
        float headControllerDist = ( head - controller ).magnitude;
        float controllerHandDist = 0.15f; // default
        
        float cutoff = 0.25f;
        if( headControllerDist > cutoff )
        {
            controllerHandDist += Mathf.Pow( controllerSpeedFactor, headControllerDist - cutoff ) - 1;
        }


        Vector3 localHandPos = myHand.localPosition;
        localHandPos.z = controllerHandDist;
        myHand.localPosition = localHandPos;
	}

    private Vector2 GetXZCoords( Transform t )
    {
        return new Vector2( t.position.x, t.position.z );
    }
}

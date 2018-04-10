using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluteEndpointConnector : MonoBehaviour 
{
    public Transform myStart;
    public Transform myEnd;
    public float startOvershoot = 0.2f;
    public float endOvershoot = 0.5f;

    private float myXScale, myZScale;
    private MeshRenderer myRenderer;
    private float mySize = 0;

	// Use this for initialization
	void Start()
    {
		myXScale = transform.localScale.x;
        myZScale = transform.localScale.z;
        myRenderer = GetComponent<MeshRenderer>();
	}
	
	// Update is called once per frame
	void Update()
    {
		// update to stay connected to myStart and myEnd
        SetEndpoints( myStart.position, myEnd.position );
	}

    public float GetMySize()
    {
        return mySize;
    }

    private void SetEndpoints( Vector3 start, Vector3 end )
    {
        // compute directional vector
        Vector3 offset = end - start;

        // redefine start and end to be a little beyond that point, in the same direction
        start -= offset.normalized * startOvershoot;
        end += offset.normalized * endOvershoot;

        // redefine offset based on new endpoints
        offset = end - start;
        mySize = offset.magnitude;

        // set angle
        transform.up = offset;

        // set position
        transform.position = start + offset / 2;

        // set scale
        transform.localScale = new Vector3( myXScale, offset.magnitude / 2, myZScale );
        UpdateTextureTiling();
    }

    private void UpdateTextureTiling()
    {
        myRenderer.material.mainTextureScale = new Vector2( 1, transform.localScale.y * 50 );
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserArc : MonoBehaviour 
{
    public int numPoints = 1000;
    public float pointGranularity = 0.01f;
    public float lineWidth = 0.1f;
    public float overestimationCorrection = 1.0f;
    private LineRenderer lineRenderer;
    private Vector3 velocity;

	// Use this for initialization
	void Start()
    {
		lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
	}

    public void SetVelocity( Vector3 v )
    {
        velocity = overestimationCorrection * v;
    }
	
	// Update is called once per frame
	void Update()
    {
	    Vector3[] points = new Vector3[ numPoints + 1 ];
        lineRenderer.positionCount = points.Length;

        for( int t = 0; t < numPoints; t++ )
        {
            // displacement = velocity * time - 1/2 * gravity * (time^2)
            points[t] = transform.position
                + ( velocity * pointGranularity * t )
                + ( 0.5f * Physics.gravity * Mathf.Pow( t * pointGranularity, 2 ) );
            lineRenderer.SetPosition( t, points[t] );

            // end early if we hit something (TODO fix; seems to be ending way before colliders)
            /*RaycastHit hit;
            if( t > 0 && Physics.Linecast( points[t-1], points[t], out hit ) )
            {
                lineRenderer.positionCount = t + 1;
                points[t] = hit.point;
                lineRenderer.SetPosition( t, points[t] );
                break;
            }*/
        }

	}
}

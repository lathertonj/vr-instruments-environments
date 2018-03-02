using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshDeformer : MonoBehaviour {

    // see catlikecoding.com/unity/tutorials/mesh-deformation
    public float springForce = 20f;
    public float damping = 5f;

    Mesh deformingMesh;
    Vector3[] originalVertices, displacedVertices;
    Vector3[] vertexVelocities;
    float uniformScale = 1f;

	// Use this for initialization
	void Start()
    {
        // copy out mesh reference and vertices
	    deformingMesh = GetComponent<MeshFilter>().mesh;
        originalVertices = deformingMesh.vertices;
        displacedVertices = new Vector3[ originalVertices.Length ];
        vertexVelocities = new Vector3[ originalVertices.Length ];
        for( int i = 0; i < originalVertices.Length; i++ )
        {
            displacedVertices[i] = originalVertices[i];
        }
	}

    private void Update()
    {
        uniformScale = transform.localScale.x;

        for( int i = 0; i < displacedVertices.Length; i++ )
        {
            UpdateVertex( i );
        }
        deformingMesh.vertices = displacedVertices;
        deformingMesh.RecalculateNormals();
    }

    private void UpdateVertex( int i )
    {
        Vector3 velocity = vertexVelocities[i];
        Vector3 displacement = displacedVertices[i] - originalVertices[i];
        displacement *= uniformScale;
        // apply spring force and damping
        velocity -= displacement * springForce * Time.deltaTime;
        velocity *= 1f - damping * Time.deltaTime;
        // store back
        vertexVelocities[i] = velocity;
        displacedVertices[i] += velocity * ( Time.deltaTime / uniformScale );
    }

    // NOTE: should call this with "point" ACTUALLY slightly OFFSET from the surface.
    public void AddDeformingForce( Vector3 point, float force )
    {
        // world space to local space:
        point = transform.InverseTransformPoint( point );
        for( int i = 0; i < displacedVertices.Length; i++ )
        {
            AddForceToVertex( i, point, force );
        }
    }

    private void AddForceToVertex( int i, Vector3 point, float force )
    {
        Vector3 pointToVertex = displacedVertices[i] - point;
        pointToVertex *= uniformScale;
        // attenuate force with inverse-square law
        float attenuatedForce = force / ( 1f + pointToVertex.sqrMagnitude );
        // convert "force" to "speed" delta (but not direction -- that's pointToVertex direction)
        float speed = attenuatedForce * Time.deltaTime;
        // apply velocity = speed * direction
        vertexVelocities[i] += pointToVertex.normalized * speed;
    }
}

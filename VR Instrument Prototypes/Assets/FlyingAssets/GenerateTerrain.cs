using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTerrain : MonoBehaviour {

    public float heightScale = 5;
    public float detailScale = 5f;

	void Start()
    {
		Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        for( int i = 0; i < vertices.Length; i++)
        {
            vertices[i].y = Mathf.PerlinNoise( ( vertices[i].x + transform.position.x ) / detailScale,
                                               ( vertices[i].z + transform.position.z ) / detailScale )
                            * heightScale;
        }

        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        gameObject.AddComponent<MeshCollider>();
	}
	
}

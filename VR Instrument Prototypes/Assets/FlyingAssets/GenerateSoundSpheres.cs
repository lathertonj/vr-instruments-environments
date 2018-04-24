using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateSoundSpheres : MonoBehaviour
{
    public Transform soundSpherePrefab;
    public int numSpheres = 10;

	// Use this for initialization
	void Start () {
		for( int i = 0; i < numSpheres; i++ )
        {
            Vector3 newPosition = new Vector3(
                Random.Range( -4.5f, 4.5f ),
                Random.Range( 1f, 6f ),
                Random.Range( -4.5f, 4.5f )
            ) + transform.position;
            Instantiate( soundSpherePrefab, newPosition, Quaternion.identity, transform );
        }
	}
}

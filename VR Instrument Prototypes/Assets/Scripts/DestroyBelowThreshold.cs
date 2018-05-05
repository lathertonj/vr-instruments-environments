using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBelowThreshold : MonoBehaviour {

    public float minY = -200f;
	
	// Update is called once per frame
	void Update()
    {
	    if( transform.position.y < minY )
        {
            Destroy( gameObject );
        }
	}
}

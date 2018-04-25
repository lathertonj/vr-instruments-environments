using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleFadeOut : MonoBehaviour
{
    MeshRenderer myRenderer;
    public float myFadeOutTime = 3; 
    public float mySizeIncrease = 3;
    float myStartTime;
    float myStartSize;
    float myStartBubbleStrength;

	// Use this for initialization
	void Start ()
    {
	    myRenderer = GetComponent<MeshRenderer>();
        myStartTime = Time.time;
        myStartBubbleStrength = myRenderer.material.GetFloat( "_Strength" );
        myStartSize = transform.localScale.x;
	}
	
	// Update is called once per frame
	void Update()
    {
        float elapsedTime = Time.time - myStartTime;
        if( elapsedTime > myFadeOutTime )
        {
            Destroy( gameObject );
        }
		// scale up
        transform.localScale = ( ( elapsedTime / myFadeOutTime ) * ( mySizeIncrease ) + 1 ) * myStartSize * Vector3.one;

        // fade out visually 10 times faster: this didn't help
        //float fadeFraction = 1 - Mathf.Clamp01( elapsedTime * 10 / myFadeOutTime );
        float fadeFraction = 1 - Mathf.Clamp01( elapsedTime / myFadeOutTime );
        myRenderer.material.SetFloat( "_Strength", fadeFraction * myStartBubbleStrength );
	}
}

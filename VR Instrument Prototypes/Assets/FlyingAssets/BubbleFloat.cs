using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleFloat : MonoBehaviour
{
    float myFloatAmount;
    float myFloatRate;
    Vector3 myStartPos;

	// Use this for initialization
	void Start ()
    {
	    myStartPos = transform.position;
        myFloatAmount = Random.Range( 0.01f, 0.08f );
        myFloatRate = Random.Range( 0.05f, 0.5f );
	}
	
	// Update is called once per frame
	void Update()
    {
        transform.position = myStartPos + Vector3.up * myFloatAmount * Mathf.Sin( Mathf.PI * myFloatRate * Time.time );
	}
}

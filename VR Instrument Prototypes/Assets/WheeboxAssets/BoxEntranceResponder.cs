using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxEntranceResponder : MonoBehaviour {

    private bool shouldEmptySelf = false;
    public float landingThreshold = 0.5f;
    public float emptyTime = 5f;
    public float resetTime = 2f;
    public void RespondToBoxEntrances()
    {
        shouldEmptySelf = true;
    }

    private SpringJoint mySpringConnection;
    private float normalSpringValue;
    private float minSpringValue = 1f;


	void Start()
    {
	    mySpringConnection = GetComponent<SpringJoint>();
        normalSpringValue = mySpringConnection.spring;
	}

    private void OnCollisionEnter( Collision collision )
    {
        if( shouldEmptySelf && collision.relativeVelocity.magnitude >= landingThreshold )
        {
            shouldEmptySelf = false;
            StartEmpty();
            StartCoroutine( "ResetEmpty" );
        }
    }

    private void StartEmpty()
    {
        mySpringConnection.spring = minSpringValue;
    }

    private IEnumerator ResetEmpty()
    {
        yield return new WaitForSeconds( emptyTime );

        float endValue = normalSpringValue;
        float startValue = mySpringConnection.spring;
        float endTime = Time.time + resetTime;
        while( Time.time < endTime )
        {
            float fractionComplete = 1 - ( endTime - Time.time ) / resetTime;
            // 1. slam shut
            // mySpringConnection.spring = SpringCalculateSlamShut( startValue, endValue, fractionComplete );
            // 2. more gradual.
            mySpringConnection.spring = SpringCalculateEasy( startValue, endValue, fractionComplete, 8 );
            // wait until next frame
            yield return null;
        }
        mySpringConnection.spring = normalSpringValue;
    }

    private float SpringCalculateSlamShut( float startValue, float endValue, float fractionComplete )
    {
        return startValue + ( endValue - startValue ) * fractionComplete;
    }

    private float SpringCalculateEasy( float startValue, float endValue, float fractionComplete, float easeFactor )
    {
        // polynomial.
        fractionComplete = Mathf.Pow( fractionComplete, easeFactor );

        // fancy exp thing also works but is more expensive sooooo
        //fractionComplete = ( Mathf.Exp( fractionComplete * easeFactor ) - 1 )
        //    / ( Mathf.Exp( easeFactor ) - 1 );
        return startValue + ( endValue - startValue ) * fractionComplete;
    }
}

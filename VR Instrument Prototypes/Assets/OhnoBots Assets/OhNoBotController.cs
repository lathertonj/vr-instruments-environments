using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OhNoBotController : MonoBehaviour
{
    public float jumpForce = 10f;
    ChuckSubInstance myChuck;
    Rigidbody myRB;
    bool amTouchingGround = false;
    Vector3 forceToAdd;

    // Use this for initialization
    void Start()
    {
        myChuck = GetComponent<ChuckSubInstance>();
        myRB = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if( forceToAdd != Vector3.zero )
        {
            myRB.AddForce( forceToAdd );
            forceToAdd = Vector3.zero;
        }
    }

    private void OnTriggerEnter( Collider other )
    {
        // When a controller hits me, jump back
        if( amTouchingGround && other.gameObject.CompareTag( "Controller" ) )
        {
            string filename = Random.Range( 0f, 1f ) < 0.5f ? "oops.wav" : "ohno.wav";
            float rate = Random.Range( 1.5f, 2.5f );
            myChuck.RunCode( string.Format(
            @"
                SndBuf buf => dac;
                me.dir() + ""{0}"" => buf.read;
                {1} => buf.rate;
                buf.length() / buf.rate() => now;
            
            ", filename, rate
            ) );

            // away from the thing I collided with
            Vector3 jumpDirection = ( transform.position - other.transform.position ).normalized;
            // but with a certain up direction
            jumpDirection.y = 0.5f;
            // renormalize
            jumpDirection.Normalize();
            forceToAdd = jumpDirection * jumpForce;
        }

    }

    private void OnCollisionEnter( Collision collision )
    {
        // keep track of whether touching ground
        if( collision.gameObject.CompareTag( "Ground" ) )
        {
            amTouchingGround = true;
        }
    }

    private void OnCollisionStay( Collision collision )
    {
        // keep track of whether touching ground
        if( collision.gameObject.CompareTag( "Ground" ) )
        {
            amTouchingGround = true;
        }
    }

    private void OnCollisionExit( Collision collision )
    {
        // keep track of whether touching ground
        if( collision.gameObject.CompareTag( "Ground" ) )
        {
            amTouchingGround = false;
        }
    }
}

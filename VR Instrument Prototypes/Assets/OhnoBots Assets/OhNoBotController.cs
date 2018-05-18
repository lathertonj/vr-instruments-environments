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
    Renderer[] myRenderers;
    Color origColor;

    // Use this for initialization
    void Start()
    {
        myChuck = GetComponent<ChuckSubInstance>();
        myRB = GetComponent<Rigidbody>();
        myRenderers = GetComponentsInChildren<Renderer>();
        origColor = myRenderers[0].material.color;
    }

    private void FixedUpdate()
    {
        if( forceToAdd != Vector3.zero )
        {
            myRB.AddForce( forceToAdd );
            forceToAdd = Vector3.zero;
        }
    }



    private void ReactToObject( GameObject other )
    {
        // Only react if I am on the ground
        if( !amTouchingGround )
        {
            return;
        }

        // Only react to VR controllers, other bots, and projectiles
        if( !other.CompareTag( "Controller" ) &&
            !other.CompareTag( "Bot" ) &&
            !other.CompareTag( "Throwable" ) )
        {
            return;
        }

        // When something hits me, jump back
        string filename = Random.Range( 0f, 1f ) < 0.5f ? "oops.wav" : "ohno.wav";
        float rate = Random.Range( 1.5f, 2.5f );
        myChuck.RunCode( string.Format( @"
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

        // show an animation
        StartCoroutine( "FadeColor" );
    }

    IEnumerator FadeColor()
    {
        Color tempColor = Color.magenta;
        for( float f = 1f; f >= 0; f -= 0.05f )
        {
            SetColor( origColor + f * ( tempColor - origColor ) );
            yield return null;
        }

        // set back to normal
        SetColor( origColor );
    }

    private void SetColor( Color c )
    {
        for( int i = 0; i < myRenderers.Length; i++ )
        {
            myRenderers[i].material.color = c;
        }
    }

    private void OnTriggerEnter( Collider other )
    {
        ReactToObject( other.gameObject );
    }

    private void OnCollisionEnter( Collision collision )
    {
        // keep track of whether touching ground
        if( collision.gameObject.CompareTag( "Ground" ) )
        {
            amTouchingGround = true;
        }

        // maybe react
        ReactToObject( collision.gameObject );
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

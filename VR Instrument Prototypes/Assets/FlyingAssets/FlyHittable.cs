using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyHittable : MonoBehaviour {

	private bool haveBeenHit = false;

    private void OnTriggerEnter( Collider other )
    {
        if( !haveBeenHit && other.gameObject.CompareTag( "Controller" ) )
        {
            haveBeenHit = true; 

            ChuckSubInstance newChuck = gameObject.AddComponent<ChuckSubInstance>();
            newChuck.spatialize = true;

            // TheChuck.Instance.RunCode
            newChuck.RunCode( string.Format( @"
                SndBuf buf => dac;
                ""special:dope"" => buf.read;
                0.5 + {0} => buf.rate;
                buf.length() / buf.rate() => now;
                buf =< dac;
            ", (FollowPlayerGaze.currentSpeed/5).ToString("0.000") ) );

            // destroy self when done
            // Invoke( "HideSelf", 0.5f );
            HideSelf();
            Invoke( "DestroySelf", 5 );
        }
    }

    void HideSelf()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }

    void DestroySelf()
    {
        Destroy( this.gameObject );
    }
}

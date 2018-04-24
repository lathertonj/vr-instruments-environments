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

            //ChuckSubInstance newChuck = gameObject.AddComponent<ChuckSubInstance>();
            //newChuck.spatialize = true;

            TheChuck.Instance.RunCode(@"
                SndBuf buf => dac;
                ""special:dope"" => buf.read;
                buf.length() => now;
                buf =< dac;
            ");

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

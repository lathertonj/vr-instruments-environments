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

            HapticFeedback controller = other.GetComponent<HapticFeedback>();
            float speed = controller.GetVelocity().magnitude;

            // TheChuck.Instance.RunCode
            newChuck.RunCode( string.Format( @"
                SndBuf buf => dac;
                2 => buf.gain;
                me.dir() + ""bubble.wav"" => buf.read; // not special:dope???
                0.5 + {0} => buf.rate;
                buf.length() / buf.rate() => now;
                buf =< dac;
            ", (speed/2).ToString("0.000") ) );

            // trigger haptic feedback after 20ms
            float timeDelay = 0.02f;
            controller.TriggerHapticFeedback( intensity: 0.5f, timeDelay: timeDelay );
            // destroy self when done
            Invoke( "HideSelf", timeDelay );
            //Invoke( "DestroySelf", 5 );
        }
    }

    void HideSelf()
    {
        //GetComponent<MeshRenderer>().enabled = false;
        BubbleFadeOut fadeOut = gameObject.AddComponent<BubbleFadeOut>();
        fadeOut.mySizeIncrease = 2f;
        fadeOut.myFadeOutTime = 0.35f;
    }

    void DestroySelf()
    {
        Destroy( this.gameObject );
    }
}

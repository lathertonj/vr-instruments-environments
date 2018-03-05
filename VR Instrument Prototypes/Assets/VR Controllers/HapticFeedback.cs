using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HapticFeedback : MonoBehaviour {

    // VR controller preamble
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    private void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    
    public Vector3 GetVelocity()
    {
        return Controller.velocity;
    }

    public void TriggerHapticFeedback()
    {
        TriggerHapticFeedback( Mathf.Clamp01( Controller.velocity.magnitude / 4 ) );
    }

    public void TriggerHapticFeedback( float intensity )
    {
        intensity = Mathf.Clamp01( intensity ); 
        int minFeedback = 50;
        int maxFeedback = 50000;
        ushort hapticDuration = (ushort) Mathf.Lerp( minFeedback, maxFeedback, 
            intensity );
        Controller.TriggerHapticPulse( durationMicroSec: hapticDuration );
    }

    public void TriggerHapticFeedback( float intensity, float timeDelay )
    {
        StartCoroutine( DelayedHapticFeedback( intensity, timeDelay ) );
    }

    IEnumerator DelayedHapticFeedback( float intensity, float timeDelay )
    {
        yield return new WaitForSeconds( timeDelay );
        TriggerHapticFeedback( intensity );
    }
}

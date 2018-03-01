using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumSurfaceHitter : MonoBehaviour {

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
        int minFeedback = 50;
        int maxFeedback = 50000;
        ushort hapticDuration = (ushort) Mathf.Lerp( minFeedback, maxFeedback, 
            Mathf.Clamp01( Controller.velocity.magnitude / 4 ) );
        Controller.TriggerHapticPulse( durationMicroSec: hapticDuration );
    }
}

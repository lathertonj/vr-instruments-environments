using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumSurface : MonoBehaviour {

    public PlayBongo myDrum;
    private ParticleSystem myEmitter;

	// Use this for initialization
	void Start()
    {
		myEmitter = GetComponentInChildren<ParticleSystem>();
	}

    private void OnTriggerEnter( Collider other )
    {
        HapticFeedback controller = other.GetComponent<HapticFeedback>();
        if( controller != null )
        {
            // get contact point: center of controller box (controller local space) + controller position (world space)
            Vector3 contactPoint = other.GetComponent<BoxCollider>().center + other.transform.position;

            // transform contact point into flat disc-local-space
            contactPoint -= transform.position;
            contactPoint.y = 0;

            // now that contact point is in flat disc-local-space, emit visual feedback
            // visual feedback: emit particle
            // override position of particle
            ParticleSystem.EmitParams newParams = new ParticleSystem.EmitParams();
            // location relative to the location of the emitter
            newParams.position = contactPoint;
            // override velocity of particle
            newParams.velocity = new Vector3( 0.0f, /*0.25f*/ 0.0f, 0.0f );
            // emit
            myEmitter.Emit( newParams, 1 );




            // transfer contact point to drum:
            // give contact point unit radius instead of local disc 0.4 radius
            contactPoint /= 0.4f;

            // get intensity
            float intensity = Mathf.Clamp01( controller.GetVelocity().magnitude / 3 );

            // play the drum
            myDrum.PlayLocalLocation( intensity, contactPoint );

            // haptic feedback
            controller.TriggerHapticFeedback( intensity );
        }
    }
}

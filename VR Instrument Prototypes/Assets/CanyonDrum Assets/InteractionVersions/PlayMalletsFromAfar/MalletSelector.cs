using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MalletSelector : MonoBehaviour {

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



    // todo: refactor to private + selector
    public Rigidbody myMallet;
    public Transform myMalletPosition;
	
	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate()
    {
        // 1. Set velocity equivalent. Too insensitive but works a tiny amount!
        // myMallet.velocity = Controller.velocity;
        
        // 2. Set velocity much bigger. Worked sort of but got out of sync if I moved to wildly! 
        // Mallet movement needs to be restricted more. 
        // NOW mostly works, just feels a bit out of sync.
        // THIS ONE WORKS THE BEST.
        myMallet.velocity = 200 * Controller.velocity;
        
        // 3. Add force in the direction of velocity. Doesn't appear to work.
        // myMallet.AddForce( Controller.velocity );
        
        // 4. Add lots of force.... Feels impossible to control. Makes it go up more than down.
        // myMallet.AddForce( Controller.velocity * 100 );
        // same:
        // myMallet.AddForceAtPosition( Controller.velocity * 100, myMalletPosition.position );
        
        // 5. Angular velocity? Wrong direction and a bit too slow.
        // myMallet.angularVelocity = Controller.angularVelocity;

        // 6. Angular velocity opposite and larger?
        // myMallet.angularVelocity = -20 * Controller.angularVelocity;
    }
}

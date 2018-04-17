using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairSecondComponentController : MonoBehaviour {

	public void Attach( Rigidbody rbToAttachMyselfTo )
    {
        CharacterJoint myJoint = GetComponent<CharacterJoint>();
        myJoint.connectedBody = rbToAttachMyselfTo;
    }
}

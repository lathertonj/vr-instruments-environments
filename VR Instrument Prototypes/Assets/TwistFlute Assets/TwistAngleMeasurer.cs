using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwistAngleMeasurer : MonoBehaviour {

    public Transform otherController;

	public float GetTwistAmount()
    {
        return Mathf.DeltaAngle( transform.localEulerAngles.z, otherController.localEulerAngles.z );
    }

    public float GetControllerDistance()
    {
        return Vector3.Distance( transform.position, otherController.position );
    }
}

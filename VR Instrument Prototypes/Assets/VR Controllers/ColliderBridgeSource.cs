using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderBridgeSource : MonoBehaviour {

    private ColliderBridgeListener myListener = null;

    public void Initialize( ColliderBridgeListener listener )
    {
        myListener = listener;
    }

    private void OnCollisionEnter( Collision collision )
    {
        if( myListener != null )
        {
            myListener.BridgeCollisionEnter( collision );
        }
    }

    private void OnCollisionStay( Collision collision )
    {
        if( myListener != null )
        {
            myListener.BridgeCollisionStay( collision );
        }
    }

    private void OnCollisionExit( Collision collision )
    {
        if( myListener != null )
        {
            myListener.BridgeCollisionExit( collision );
        }
    }

    private void OnTriggerEnter( Collider other )
    {
        if( myListener != null )
        {
            myListener.BridgeTriggerEnter( other );
        }
    }

    private void OnTriggerStay( Collider other )
    {
        if( myListener != null )
        {
            myListener.BridgeTriggerStay( other );
        }
    }

    private void OnTriggerExit( Collider other )
    {
        if( myListener != null )
        {
            myListener.BridgeTriggerExit( other );
        }
    }
}

public interface ColliderBridgeListener
{
    void BridgeCollisionEnter( Collision collision );
    void BridgeCollisionStay( Collision collision );
    void BridgeCollisionExit( Collision collision );

    void BridgeTriggerEnter( Collider other );
    void BridgeTriggerStay( Collider other );
    void BridgeTriggerExit( Collider other );
}

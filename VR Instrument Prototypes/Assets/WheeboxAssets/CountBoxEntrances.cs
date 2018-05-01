using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountBoxEntrances : MonoBehaviour {

    public BoxEntranceResponder boxToTell;
    public int minEntrances;
    public int maxEntrances;
    private int numEntrances;

    private void OnTriggerEnter( Collider other )
    {
        numEntrances++;
        if( numEntrances >= maxEntrances )
        {
            InformAndReset();
        }
        else if( numEntrances >= minEntrances )
        {
            float probOfInform = ( numEntrances - minEntrances ) * 1.0f / ( maxEntrances - minEntrances );
            if( Random.Range( 0f, 1f ) <= probOfInform )
            {
                InformAndReset();
            }
        }
    }

    private void InformAndReset()
    {
        boxToTell.RespondToBoxEntrances();
        numEntrances = 0;
    }
}

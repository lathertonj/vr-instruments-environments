using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OhNoBotGenerator : MonoBehaviour
{
    public Transform ohnoBotPrefab;
    public int numBots;

    // Use this for initialization
    void Start()
    {
        for( int i = 0; i < numBots; i++ )
        {
            Vector3 newPos = new Vector3(
                Random.Range( -1f, 1f ),
                0.1f,
                Random.Range( -1f, 1f )
            );

            Instantiate( ohnoBotPrefab, newPos, Quaternion.identity, transform );
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}

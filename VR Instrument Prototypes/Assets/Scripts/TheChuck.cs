using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheChuck : MonoBehaviour {

    public static ChuckMainInstance Instance;

	// Use this for initialization
	void Awake()
    {
	    if( Instance == null)
        {
            Instance = GetComponent<ChuckMainInstance>();
        }
	}
}

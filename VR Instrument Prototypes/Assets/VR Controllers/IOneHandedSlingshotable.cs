using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOneHandedSlingshotable
{
    void BeLetGoOf( int numClicks );
    void BeGrabbed();
    void BeMovedToward( Vector3 position );
}

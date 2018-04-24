using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Tile
{
    public GameObject theTile;
    public float creationTime;

    public Tile( GameObject t, float ct )
    {
        theTile = t;
        creationTime = ct;
    }
}

public class GenerateInfiniteTerrain : MonoBehaviour
{
    public GameObject player;
    public GameObject planePrefab;
    int planeSize = 10;
    public int tileRadius = 8;

    private Vector3 lastPlayerPos;
    Dictionary<string, Tile> tiles = new Dictionary<string, Tile>();

	// Use this for initialization
	void Start()
    {
		transform.position = Vector3.zero;
        lastPlayerPos = player.transform.position;
        float updateTime = Time.realtimeSinceStartup;

        for( int x = -tileRadius; x < tileRadius; x++ )
        {
            for( int z = -tileRadius; z < tileRadius; z++ )
            {
                Vector3 pos = new Vector3(
                    x * planeSize + lastPlayerPos.x,
                    0,
                    z * planeSize + lastPlayerPos.z 
                );

                GameObject t = Instantiate( planePrefab, pos, Quaternion.identity, transform );
                string tilename = "Tile_" + ((int) pos.x).ToString() + "_" + ((int) pos.z).ToString();
                t.name = tilename;
                Tile tile = new Tile( t, updateTime );
                tiles[tilename] = tile;
            }
        }
	}
	
	// Update is called once per frame
	void Update()
    {
	    int xMove = (int) ( player.transform.position.x - lastPlayerPos.x );
        int zMove = (int) ( player.transform.position.z - lastPlayerPos.z );

        // check if we've moved more than a plane away from the center
        if( Mathf.Abs( xMove ) >= planeSize || Mathf.Abs( zMove ) >= planeSize )
        {
            // find nearest tile size
            int playerX = (int) ( Mathf.Floor( player.transform.position.x / planeSize ) * planeSize );
            int playerZ = (int) ( Mathf.Floor( player.transform.position.z / planeSize ) * planeSize );

            float updateTime = Time.realtimeSinceStartup;

            for( int x = -tileRadius; x < tileRadius; x++ )
            {
                for( int z = -tileRadius; z < tileRadius; z++ )
                {
                    Vector3 pos = new Vector3(
                        x * planeSize + playerX,
                        0,
                        z * planeSize + playerZ
                    );
                    string tilename = "Tile_" + ((int) pos.x).ToString() + "_" + ((int) pos.z).ToString();

                    if( ! tiles.ContainsKey( tilename ) )
                    {
                        // create new tile
                        GameObject t = Instantiate( planePrefab, pos, Quaternion.identity, transform );
                        t.name = tilename;
                        Tile tile = new Tile( t, updateTime );
                        tiles[tilename] = tile;
                    }
                    else
                    {
                        // update the time to prevent this tile being marked for deletion
                        tiles[tilename].creationTime = updateTime;
                    }
                }
            }

            // destroy all tiles not just created or updated
            Dictionary<string, Tile> newTiles = new Dictionary<string, Tile>();
            foreach( Tile t in tiles.Values)
            {
                // delete old objects
                if( t.creationTime != updateTime )
                {
                    Destroy( t.theTile );
                }
                // transfer new objects
                else
                {
                    newTiles[t.theTile.name] = t;
                }
            }
            // copy the new dictionary
            tiles = newTiles;

            // update last player pos
            lastPlayerPos = player.transform.position;
        }
	}
}

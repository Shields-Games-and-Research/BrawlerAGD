using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using System.IO;

public class LevelLoader : MonoBehaviour
{

    // Laid out like keyboard keys
    public Tile Q; // represents platform top right corner
    public Tile W;
    public Tile E;
    public Tile A;
    public Tile S;
    public Tile D;
    public Tile Z;
    public Tile X;
    public Tile C;
    public GameGenerator gamegen;

    // Start is called before the first frame update
    void Start()
    {
        var tilemap = GetComponentInChildren<Tilemap>();
        Debug.Log(tilemap.cellBounds);
        foreach (var platform in gamegen.platforms.platformList)
        {
            //TODO: mirroring
            MakePlatform(tilemap, platform.ToBounds());
        }
    }

    private void MakePlatform(Tilemap tilemap, BoundsInt bounds)
    {
        foreach (var p in bounds.allPositionsWithin)
        {
            var tile = GetTile(p, bounds);
            Debug.Log(p);
            //Debug.Log(tile);
            tilemap.SetTile(p, tile);
        }
    }

    private Tile GetTile(Vector3Int p, BoundsInt bounds)
    {
        var tile = S;
        // Tiles at the top
        if (p.y == bounds.yMax - 1)
        {
            tile = W;
            if (p.x == bounds.x)
            {
                tile = Q;
            }
            if (p.x == bounds.xMax - 1)
            {
                tile = E;
            }
        }
        // Tiles at the bottom
        else if (p.y == bounds.y)
        {
            tile = X;
            if (p.x == bounds.x)
            {
                tile = Z;
            }
            if (p.x == bounds.xMax - 1)
            {
                tile = C;
            }
        }
        // Tiles in the middle
        else
        {
            if (p.x == bounds.x)
            {
                tile = A;
            }
            if (p.x == bounds.xMax - 1)
            {
                tile = D;
            }
        }
        return tile;
    }

}

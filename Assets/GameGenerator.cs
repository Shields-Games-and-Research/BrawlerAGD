using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;


[Serializable]
public class Platform
{
    public int v1;
    public int v2;
    public int v3;
    public int v4;
    public int v5;
    public int v6;

    public Platform(int av1, int av2, int av3, int av4, int av5, int av6)
    {
        v1 = av1;
        v2 = av2;
        v3 = av3;
        v4 = av4;
        v5 = av5;
        v6 = av6;
    }

    public BoundsInt ToBounds()
    {
        return new BoundsInt(v1, v2, v3, v4, v5, v6);
    }
}

[Serializable]
public class Platforms
{
    public List<Platform> platformList;

    public Platforms(List<Platform> l)
    {
        platformList = l;
    }
}

public class GameGenerator : MonoBehaviour
{
    /** Player 1 Constants:
     *  SPAWN_LOCATION_P1 = ; // where the first spawn of the first player is
     *  LEFT_KEY_P1 = left; //left key movement
     *  RIGHT_KEY_P1 = right; //right key movement
     *  JUMP_KEY_P1 = jump; //jump key
     *  FALL_KEY_P1 = fall; //fall key (TODO)
     *  MOVE_1_KEY_P1 = move1Key; //attack key
     *  
     *  Player 1 Move 1 Constants:
     *  
     *  
     *  Player 2 Constants:
     *  SPAWN_LOCATION_P2 = ; // where the first spawn of the second player is
     *  LEFT_KEY_P2 = ; //left key movement
     *  RIGHT_KEY_P2 = ; //right key movement
     *  JUMP_KEY_P2 = ; //jump key
     *  FALL_KEY_P2 = ; //fall key (TODO)
     *  MOVE_1_KEY_P2 = ; //attack key
     */

    public Player player;
    public Platforms platforms;
    private static string level_path = "Assets\\Game\\level.json";

    // Start is called before the first frame update
    void Start()
    {
        // Generate / Load Map
        Platform platform1 = new Platform(-8, -5, 0, 16, 4, 1);
        List<Platform> platformList = new List<Platform>()
        {
            platform1
        };
        platforms = new Platforms(platformList);

        // Write to file
        if (!File.Exists(level_path))
        {
            var test = JsonUtility.ToJson(platform1);
            Debug.Log(test);
            var level_string = JsonUtility.ToJson(platforms);
            Debug.Log(platforms);
            Debug.Log(level_string);
            File.Create(level_path).Dispose();
            File.WriteAllText(level_path, level_string);
        }
        // If the file exists, read from it
        else
        {
            var inputString = File.ReadAllText(level_path);
            platforms = JsonUtility.FromJson<Platforms>(inputString);
        }

        //TODO: Fetch from document - replace with constants set above during setup
        
        //Player 1 Instantiation
        Vector3 spawnLocationP1 = new Vector3(-2, 0, 0);
        Player player1 = Instantiate(player, spawnLocationP1, Quaternion.identity);
        //Player 1 Controls
        player1.leftKey = KeyCode.A;
        player1.rightKey = KeyCode.D;
        player1.jumpKey = KeyCode.W;
        player1.fallKey = KeyCode.S;
        player1.move1Key = KeyCode.Space;
        //player 1 Move 1 Definition
        player1.move1.center = player1.transform.position + new Vector3(-1, 0, 0);
        

        //Player 2 Instantiation
        Vector3 spawnLocationP2 = new Vector3(2, 0, 0);
        Player player2 = Instantiate(player, spawnLocationP2, Quaternion.identity);
        //Player 2 Controls
        player2.leftKey = KeyCode.J;
        player2.rightKey = KeyCode.L;
        player2.jumpKey = KeyCode.I;
        player2.fallKey = KeyCode.K;
        player2.move1Key = KeyCode.Return;

        // Generate / Load constants for each player

        // Generate / Load constants for the player's moves 

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class MapGenerator
{
    //
}

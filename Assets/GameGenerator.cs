using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Random = System.Random;
using System.Linq;

[Serializable]
public class Platform
{
    public int x;
    public int y;
    public int xSize;
    public int ySize;

    // 2d Platform
    public static int z = 0;
    public static int zSize = 1;

    public Platform(int av1, int av2, int av3, int av4)
    {
        x = av1;
        y = av2;
        xSize = av3;
        ySize = av4;
    }

    public BoundsInt ToBounds()
    {
        return new BoundsInt(x, y, z, xSize, ySize, zSize);
    }

    public Platform xMirror()
    {
        int newX = -x - xSize;
        return new Platform(newX, y, xSize, ySize);
    }
}

[Serializable]
public class Platforms
{
    public List<Platform> platformList;
    public int player1x;
    public int player1y;
    public int player2x;
    public int player2y;

    public Platforms(List<Platform> l, int p1x, int p1y, int p2x, int p2y)
    {
        platformList = l;
        player1x = p1x;
        player1y = p1y;
        player2x = p2x;
        player2y = p2y;
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
        // Initialize RNG
        Random rand = new Random();

        // Generate / Load Map
        Platform platform1 = new Platform(-8, -5, 8, 4);
        List<Platform> platformList = new List<Platform>()
        {
            platform1,
            platform1.xMirror()
        };
        //platforms = new Platforms(platformList);

        MapGenerator mapGen = new MapGenerator(2, 2, 3, 6, rand);
        platforms = mapGen.generate();
        //Debug.Log(platforms.platformList[0].x);

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
            //TODO: Temporarily disabled for ease of iteration.
            //platforms = JsonUtility.FromJson<Platforms>(inputString);
        }

        //TODO: Fetch from document - replace with constants set above during setup
        
        //Player 1 Instantiation
        Vector3 spawnLocationP1 = new Vector3(platforms.player1x, platforms.player1y, 0);
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
        Vector3 spawnLocationP2 = new Vector3(platforms.player2x, platforms.player2y, 0);
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
    public int jumpHeight;
    public int jumpLength;
    public int nPlatforms;
    public int maxPlatformSize;
    Random rand;

    public static int minWidth = 1;
    public static int maxWidth = 2;
    public static int initialY = -3;

    public MapGenerator(int _jumpHeight, int _jumpLength, int _nPlatforms, int _maxPlatformSize, Random _rand)
    {
        jumpHeight = _jumpHeight;
        jumpLength = _jumpLength;
        nPlatforms = _nPlatforms;
        maxPlatformSize = _maxPlatformSize;
        rand = _rand;
    }

    public Platforms generate()
    {
        List<Platform> allPlatforms = new List<Platform>();
        List<Platform> stack = new List<Platform>();
        // Create initial platform
        Platform initialPlatform = initial();
        allPlatforms.Add(initialPlatform);
        stack.Add(initialPlatform);
        // For each platform, create 0-2 children
        // Stop when the length reaches nPlatforms
        // Mirror everything around y = 0
        List<Platform> mirrorPlatforms = new List<Platform>();
        foreach (Platform platform in allPlatforms)
        {
            mirrorPlatforms.Add(platform.xMirror());
        }
        allPlatforms = allPlatforms.Concat(mirrorPlatforms).ToList();
        // Player 1 spawns on the initial platform
        int p1x = rand.Next(initialPlatform.x, initialPlatform.x + initialPlatform.xSize + 1);
        int p1y = initialY + initialPlatform.ySize + 1;
        // Mirror Player 2's spawn relative to Player 1's
        int p2x = -p1x;
        int p2y = p1y;
        return new Platforms(allPlatforms, p1x, p1y, p2x, p2y);
    }

    public Platform initial()
    {
        int y = initialY;
        int ySize = rand.Next(minWidth, maxWidth + 1);
        int x = rand.Next(-maxPlatformSize, -1);
        int midGap = jumpLength / 2;
        int xSize = -x + rand.Next(-midGap, 0);
        return new Platform(x, y, xSize, ySize);
    }

    public Platform above(Platform platform)
    {
        // Generate y values
        int yMin = platform.y + 1;
        int yMax = platform.y + jumpHeight;
        int y = rand.Next(yMin, yMax + 1);
        int ySize = rand.Next(minWidth, maxWidth + 1);
        // Generate x values
        int xMin = platform.x + 1;
        int xMax = platform.x + platform.xSize;
        int x = rand.Next(xMin, xMax);
        int xSize = rand.Next(1, platform.xSize - x + 1);
        return new Platform(x, y, ySize, xSize);
    }

    public Platform left(Platform platform)
    {
        return null;
    }
}

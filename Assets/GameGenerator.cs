using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Random = System.Random;
using System.Linq;
using UnityEngine.UI;

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

    public Player player;
    public Platforms platforms;
    private static string levelPath = "Assets\\Game\\level.json";
    private static string player1Path = "Assets\\Game\\player1.json";
    private static string player2Path = "Assets\\Game\\player2.json";
    private static string player1Move1Path = "Assets\\Game\\p1move1.json";
    private static string player2Move1Path = "Assets\\Game\\p2move1.json";

    //UI components for each player
    public GameObject p1HUD;
    public GameObject p2HUD;

    /** Assignment of values from the Serialized Object. TODO: Static evaluators
     */
    public void InitializePlayerFromSerializedObj(SerializedPlayer serializedPlayer, Player player)
    {  
        //assigns controls of player
        player.leftKey = serializedPlayer.leftKey;
        player.rightKey = serializedPlayer.rightKey;
        player.jumpKey = serializedPlayer.jumpKey;
        player.move1Key = serializedPlayer.attackKey;
        //Player name
        player.playerName = serializedPlayer.playerName;
        //Starting Lives
        player.stocks = serializedPlayer.stocks;
        //Speed applied on left/right
        player.velocity = serializedPlayer.velocity;
        //Force applied on initial jump
        player.groundJumpForce = serializedPlayer.groundJumpForce;
        //Force applied on a second jump
        player.airJumpForce = serializedPlayer.airJumpForce;
        //The scale of hitstun applied to player based on damage
        player.hitstunDamageScalar = serializedPlayer.hitstunDamageScalar;
        //Player's respawn location
        player.respawnLoc = new Vector2(serializedPlayer.respawnX, serializedPlayer.respawnY);
        //Player's width and height
        player.transform.localScale = new Vector2(serializedPlayer.widthScalar, serializedPlayer.heightScalar);
        //Player physics settings
        player.rb.gravityScale = serializedPlayer.gravityScalar;
        player.rb.mass = serializedPlayer.mass;
        player.rb.drag = serializedPlayer.drag;
    }

    /** Assignment of values from the Serialized Object. Requires a player as a parameter as a dependency
     */
    public void InitializeMoveFromSerializedObj(SerializedMove serializedMove, Move move, Player player) 
    {
        //assigns move to a player
        player.move1 = move;
        //sets location relative to assigned player
        player.move1.center = player.transform.position + new Vector3(serializedMove.moveLocX, serializedMove.moveLocY);
        //Sets width and height of move
        player.move1.transform.localScale = new Vector2(serializedMove.widthScalar, serializedMove.heightScalar);
        //Move Phase Durations
        player.move1.warmUpDuration = serializedMove.warmUpDuration;
        player.move1.executionDuration = serializedMove.executionDuration;
        player.move1.coolDownDuration = serializedMove.coolDownDuration;
        //Damage
        player.move1.damageGiven = serializedMove.damageGiven;
        //Knockback Scalar
        player.move1.knockbackScalar = serializedMove.knockbackScalar;
        //Directional Knockback Modifier
        player.move1.knockbackDirection = new Vector2(serializedMove.knockbackModX, serializedMove.knockbackModY).normalized;
        //Base Hitstun duration
        player.move1.hitstunDuration = serializedMove.hitstunDuration;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Initialize RNG
        Random rand = new Random();

        // Generate / Load Map
        MapGenerator mapGen = new MapGenerator(2, 2, 3, 6, rand);
        platforms = mapGen.generate();
        // Load from or write to file
        platforms = ReadJson<Platforms>(levelPath, platforms);

        //Serialized Player 1 Setup
        SerializedPlayer serializedPlayer1 = new SerializedPlayer("Player 1", KeyCode.W, KeyCode.A, KeyCode.D, KeyCode.S, rand);
        serializedPlayer1 = ReadJson<SerializedPlayer>(player1Path, serializedPlayer1);
        serializedPlayer1.respawnX = platforms.player1x;
        serializedPlayer1.respawnY = platforms.player1y;

        //Serialized Player 1, Move 1 Setup
        SerializedMove serializedMove1Player1 = new SerializedMove(rand);
        serializedMove1Player1 = ReadJson<SerializedMove>(player1Move1Path, serializedMove1Player1);
        
        //Player 1 Instantiation
        Vector3 spawnLocationP1 = new Vector3(platforms.player1x, platforms.player1y, 0);
        Player player1 = Instantiate(player, spawnLocationP1, Quaternion.identity);
        player1.playerDetails = p1HUD.GetComponent<Text>();

        InitializePlayerFromSerializedObj(serializedPlayer1, player1);
        InitializeMoveFromSerializedObj(serializedMove1Player1, player1.move1, player1);

        //Serialized Player 2 Setup
        SerializedPlayer serializedPlayer2 = new SerializedPlayer("Player 2", KeyCode.I, KeyCode.J, KeyCode.L, KeyCode.K, rand);
        serializedPlayer2 = ReadJson<SerializedPlayer>(player2Path, serializedPlayer2);
        serializedPlayer2.respawnX = platforms.player2x;
        serializedPlayer2.respawnY = platforms.player2y;

        //Serialized Player 2 Move 1 Setup
        SerializedMove serializedMove1Player2 = new SerializedMove(rand);
        serializedMove1Player2 = ReadJson<SerializedMove>(player2Move1Path, serializedMove1Player2);

        //Player 2 Instantiation
        Vector3 spawnLocationP2 = new Vector3(platforms.player2x, platforms.player2y, 0);
        Player player2 = Instantiate(player, spawnLocationP2, Quaternion.identity);
        player2.playerDetails = p2HUD.GetComponent<Text>();

        InitializePlayerFromSerializedObj(serializedPlayer2, player2);
        InitializeMoveFromSerializedObj(serializedMove1Player2, player2.move1, player2);

}

    // Update is called once per frame
    void Update()
    {
        
    }

    T ReadJson<T>(string filename, T ifFileMissing)
    {
        // Write to file
        if (!File.Exists(filename))
        {
            var objStr = JsonUtility.ToJson(ifFileMissing);
            File.Create(filename).Dispose();
            File.WriteAllText(filename, objStr);
            return ifFileMissing;
        }
        // If the file exists, read from it
        else
        {
            var inputString = File.ReadAllText(filename);
            T obj = JsonUtility.FromJson<T>(inputString);
            //return obj;
            //TODO: temporarily disabled for iteration
            return ifFileMissing;
        }
    }
}

[Serializable]
public class SerializedPlayer
{
    //Name
    public String playerName;
    //Controls
    public KeyCode jumpKey;
    public KeyCode leftKey;
    public KeyCode rightKey;
    public KeyCode attackKey;
    //Stocks
    public int stocks;
    //Velocity
    public float velocity;
    //Ground Jump Force
    public float groundJumpForce;
    //Air Jump Force
    public float airJumpForce;
    //Mass
    public float mass;
    //Linear Drag
    public float drag;
    //Width Scaling
    public float widthScalar;
    //Height Scaling
    public float heightScalar;
    //Gravity Scaling
    public float gravityScalar;
    //Respawn Location
    public float respawnX;
    public float respawnY;
    //Hit Stun Scalar
    public float hitstunDamageScalar;

    public SerializedPlayer(String _name, KeyCode _jump, KeyCode _left, KeyCode _right, KeyCode _attack, Random rand)
    {
        playerName = _name;
        jumpKey = _jump;
        leftKey = _left;
        rightKey = _right;
        attackKey = _attack;
        stocks = 3;
        velocity = 2f + 8 * (float) rand.NextDouble();
        float totalJumpForce = 4f + 12 * (float) rand.NextDouble();
        float jumpRatio = (float) rand.NextDouble();
        groundJumpForce = totalJumpForce * jumpRatio;
        airJumpForce = totalJumpForce * (1 - jumpRatio);
        mass = 0.5f + 2 * (float) rand.NextDouble();
        drag = 1f + 5 * (float) rand.NextDouble();
        widthScalar = 0.7f + 0.8f * (float) rand.NextDouble();
        heightScalar = 0.5f + (float) rand.NextDouble();
        gravityScalar = 0.3f + (float) rand.NextDouble();
        hitstunDamageScalar = 0.1f + 0.2f * (float) rand.NextDouble();
        respawnX = 0f;
        respawnY = 0f;
    }
}

[Serializable]
public class SerializedMove
{
    //Move Center (Relative to Player Center)
    public float moveLocX;
    public float moveLocY;
    //Width Scaling
    public float widthScalar;
    //Height Scaling
    public float heightScalar;
    //Warm-Up State Duration
    public float warmUpDuration;
    //Active State Duration
    public float executionDuration;
    //Cool Down State Duration
    public float coolDownDuration;
    //Damage given from hitting
    public float damageGiven;
    //Knockback scalar applied from move hitting
    public float knockbackScalar;
    //Additional Knockback Vector Direction
    public float knockbackModX;
    public float knockbackModY;
    //Base Hitstun duration
    public float hitstunDuration;

    public SerializedMove(Random rand)
    {
        moveLocX = -1f;
        moveLocY = 0f;
        widthScalar = 0.5f + (float) rand.NextDouble();
        heightScalar = 0.5f + (float) rand.NextDouble();
        warmUpDuration = 0.1f + 0.5f *(float) rand.NextDouble();
        executionDuration = 0.1f + 0.3f * (float) rand.NextDouble();
        coolDownDuration = 0.1f + 0.5f * (float) rand.NextDouble();
        damageGiven = 5 + (warmUpDuration + executionDuration + coolDownDuration) * (float) rand.NextDouble() * 10f;
        knockbackScalar = 1f + 15f * (float) rand.NextDouble();
        knockbackModX = 0f;
        knockbackModY = 1f;
        hitstunDuration = 1f;
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
        Stack<Platform> stack = new Stack<Platform>();
        // Create initial platform
        Platform initialPlatform = Initial();
        allPlatforms.Add(initialPlatform);
        stack.Push(initialPlatform);
        // For each platform, create 0-2 children
        // Stop when the length reaches nPlatforms
        while (allPlatforms.Count < nPlatforms)
        {
            if (stack.Count == 0)
            {
                break;
            }
            Platform top = stack.Pop();
            if (rand.Next(0, 2) == 1)
            {
                Platform leftPlatform = Left(top);
                stack.Push(leftPlatform);
                allPlatforms.Add(leftPlatform);
            }
            if (rand.Next(0, 2) == 1)
            {
                Platform abovePlatform = Above(top);
                stack.Push(abovePlatform);
                allPlatforms.Add(abovePlatform);
            }
        }
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

    public Platform Initial()
    {
        int y = initialY;
        int ySize = rand.Next(minWidth, maxWidth + 1);
        int x = rand.Next(-maxPlatformSize - 1, -2);
        int midGap = jumpLength / 2;
        int xSize = -x + rand.Next(-midGap, 0);
        Debug.Log("initial");
        Debug.Log(x);
        Debug.Log(y);
        Debug.Log(xSize);
        Debug.Log(ySize);
        return new Platform(x, y, xSize, ySize);
    }

    public Platform Above(Platform platform)
    {
        // Generate y values
        int yMin = 2;
        int yMax = jumpHeight;
        int platformTop = platform.y + platform.ySize;
        int y = platformTop + rand.Next(yMin, yMax + 1);
        Debug.Log(minWidth);
        Debug.Log(maxWidth + 1);
        int ySize = rand.Next(minWidth, Math.Min(maxWidth, y - platformTop));
        //Math.Min(rand.Next(minWidth, maxWidth + 1), y - platform.y);
        // Generate x values
        int xMin = platform.x + 1;
        int xMax = platform.x + platform.xSize;
        int x = rand.Next(xMin, xMax);
        int xSize = rand.Next(2, platform.xSize - x + 1);
        Debug.Log("above");
        Debug.Log(yMin);
        Debug.Log(yMax);
        Debug.Log(x);
        Debug.Log(y);
        Debug.Log(xSize);
        Debug.Log(ySize);
        return new Platform(x, y, xSize, ySize);
    }

    public Platform Left(Platform platform)
    {
        // Generate y values
        int yMin = platform.y - jumpHeight;
        int yMax = platform.y + jumpHeight;
        int y = rand.Next(yMin, yMax + 1);
        int ySize = rand.Next(minWidth, maxWidth + 1);
        // Generate x values
        int xSize = rand.Next(2, maxPlatformSize);
        int xRight = rand.Next(1, jumpLength);
        int x = platform.x - xRight - xSize;
        Debug.Log("left");
        Debug.Log(x);
        Debug.Log(y);
        Debug.Log(xSize);
        Debug.Log(ySize);
        return new Platform(x, y, xSize, ySize);
    }
}

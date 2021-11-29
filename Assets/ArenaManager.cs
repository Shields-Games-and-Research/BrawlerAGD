using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Random = System.Random;
using System.Linq;
using UnityEngine.UI;

public class ArenaManager : MonoBehaviour
{
    //TODO: File management approach
    private static string levelPath = "Assets\\Game\\level.json";
    private static string player1Path = "Assets\\Game\\player1.json";
    private static string player2Path = "Assets\\Game\\player2.json";
    private static string player1Move1Path = "Assets\\Game\\p1move1.json";
    private static string player2Move1Path = "Assets\\Game\\p2move1.json";

    //UI components for each player
    public GameObject p1HUD;
    public GameObject p2HUD;

    //UI components for notifications
    public GameObject notifications;

    //Prefab Declaration
    public Move move;
    public Player player;
    public Platforms platforms;

    // Start is called before the first frame update
    void Start()
    {
        this.InitializeGame(true, true);
    }

    public void InitializeGame(bool p1Playable, bool p2Playable)
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

        //Player 1 Heads Up Display
        player1.playerDetails = p1HUD.GetComponent<Text>();
        player1.notifications = notifications.GetComponent<Text>();

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


        //Player 2 Heads Up Display
        player2.playerDetails = p2HUD.GetComponent<Text>();
        player2.notifications = notifications.GetComponent<Text>();

        //Player Controller or Agent Assignment
        if (p1Playable)
        {
            player1.controller = new Controller(player1, null);
            player1.controller.leftKey = KeyCode.A;
            player1.controller.rightKey = KeyCode.D;
            player1.controller.jumpKey = KeyCode.W;
            player1.controller.move1Key = KeyCode.S;
        }
        else 
        {
            player1.controller = new AI(player1, player2);
        }
        if (p2Playable)
        {
            player2.controller = new Controller(player2, null);
            player2.controller.leftKey = KeyCode.J;
            player2.controller.rightKey = KeyCode.L;
            player2.controller.jumpKey = KeyCode.I;
            player2.controller.move1Key = KeyCode.K;
        }
        else
        {
            player2.controller = new AI(player2, player1);
        }

        //update gameobjects instantiated into the scene with values from JSON
        InitializePlayerFromSerializedObj(serializedPlayer1, player1);
        InitializeMoveFromSerializedObj(serializedMove1Player1, player1);
        InitializePlayerFromSerializedObj(serializedPlayer2, player2);
        InitializeMoveFromSerializedObj(serializedMove1Player2, player2);

        //TODO: move notification to arena manager
        StartCoroutine(player1.NotificationCoroutine("FIGHT!"));

        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = Time.fixedDeltaTime * Time.timeScale;
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

    /** Assignment of values from the Serialized Object. TODO: Static evaluators
     * See player object for detailed field information
 */
    public void InitializePlayerFromSerializedObj(SerializedPlayer serializedPlayer, Player player)
    {
        //assigns controls of player
        player.leftKey = serializedPlayer.leftKey;
        player.rightKey = serializedPlayer.rightKey;
        player.jumpKey = serializedPlayer.jumpKey;
        player.move1Key = serializedPlayer.attackKey;
        //Player parameter initialization
        player.playerName = serializedPlayer.playerName;
        player.stocks = serializedPlayer.stocks;
        player.groundAcceleration = serializedPlayer.groundAcceleration;
        player.airAcceleration = serializedPlayer.airAcceleration;
        player.maxGroundSpeed = serializedPlayer.maxGroundSpeed;
        player.maxAirSpeed = serializedPlayer.maxAirSpeed;
        player.groundJumpForce = serializedPlayer.groundJumpForce;
        player.airJumpForce = serializedPlayer.airJumpForce;
        player.hitstunDamageScalar = serializedPlayer.hitstunDamageScalar;
        player.respawnLoc = new Vector2(serializedPlayer.respawnX, serializedPlayer.respawnY);
        player.transform.localScale = new Vector2(serializedPlayer.widthScalar, serializedPlayer.heightScalar);
        player.rb.gravityScale = serializedPlayer.gravityScalar;
        player.rb.mass = serializedPlayer.mass;
        player.rb.drag = serializedPlayer.drag;
    }

    /** Assignment of values from the Serialized Object. Requires a player as a parameter as a dependency
     *  See move definition for field information
     */
    public void InitializeMoveFromSerializedObj(SerializedMove serializedMove, Player player)
    {
        //first find center for instantiation
        Vector2 center = player.transform.position + new Vector3(serializedMove.moveLocX, serializedMove.moveLocY);
        //instantiates a move to a player and sets location relative to the player
        player.move1 = Instantiate<Move>(move, center, Quaternion.identity, player.transform);
        player.move1.center = center;
        //Sets width and height of move
        player.move1.transform.localScale = new Vector2(serializedMove.widthScalar, serializedMove.heightScalar);
        //Move Parameter initialization
        player.move1.warmUpDuration = serializedMove.warmUpDuration;
        player.move1.executionDuration = serializedMove.executionDuration;
        player.move1.coolDownDuration = serializedMove.coolDownDuration;
        player.move1.damageGiven = serializedMove.damageGiven;
        player.move1.knockbackScalar = serializedMove.knockbackScalar;
        player.move1.knockbackDirection = new Vector2(serializedMove.knockbackModX, serializedMove.knockbackModY).normalized;
        player.move1.hitstunDuration = serializedMove.hitstunDuration;
    }



    public void BeginGame() 
    { 
    
    }

    public void EndGame() 
    {
        //send results to Evolution Manager
        //GameResult result = new GameResult();
    
    }

   
}

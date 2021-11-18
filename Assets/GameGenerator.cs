using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Random = System.Random;
using System.Linq;
using UnityEngine.UI;

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

    //UI components for notifications
    public GameObject notifications;

    //Game Data Singleton for Agent Usage
    public GameData gameData;

    //Prefab Declaration
    public Move move;

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
        //self-applied accelleration on ground
        player.groundAcceleration = serializedPlayer.groundAcceleration;
        //self-applied accelleration in air
        player.airAcceleration = serializedPlayer.airAcceleration;
        //Maximum self-applied speed on ground
        player.maxGroundSpeed = serializedPlayer.maxGroundSpeed;
        //Maximum self-applied speed in air
        player.maxAirSpeed = serializedPlayer.maxAirSpeed;
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
    public void InitializeMoveFromSerializedObj(SerializedMove serializedMove, Player player) 
    {
        //first find center for instantiation
        Vector2 center = player.transform.position + new Vector3(serializedMove.moveLocX, serializedMove.moveLocY);

        //instantiates a move to a player
        player.move1 = Instantiate<Move>(move, center, Quaternion.identity, player.transform);

        //sets location relative to assigned player
        player.move1.center = center;
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
        //Player 1 Controller/Agent Controller Assignment
        //player1.controller = new Controller(player1, null);
        //player1.controller.leftKey = KeyCode.A;
        //player1.controller.rightKey = KeyCode.D;
        //player1.controller.jumpKey = KeyCode.W;
        //player1.controller.move1Key = KeyCode.S;
        //Player 1 Heads Up Display
        player1.playerDetails = p1HUD.GetComponent<Text>();
        player1.notifications = notifications.GetComponent<Text>();

        InitializePlayerFromSerializedObj(serializedPlayer1, player1);
        InitializeMoveFromSerializedObj(serializedMove1Player1, player1);
        //Add Player 1 reference to GameData
        gameData.players.Add(player1);

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
        //Player 2 Controller/Agent Controller Assignment
        player1.controller = new AI(player1, player2);
        player2.controller = new AI(player2, player1);
        //player2.controller = new Controller(player2);
        //player2.controller.leftKey = KeyCode.J;
        //player2.controller.rightKey = KeyCode.L;
        //player2.controller.jumpKey = KeyCode.I;
        //player2.controller.move1Key = KeyCode.K;
        
        
        //Player 2 Heads Up Display
        player2.playerDetails = p2HUD.GetComponent<Text>();
        player2.notifications = notifications.GetComponent<Text>();

        InitializePlayerFromSerializedObj(serializedPlayer2, player2);
        InitializeMoveFromSerializedObj(serializedMove1Player2, player2);

        //Add Player 2 reference to GAmeData
        gameData.players.Add(player2);

        StartCoroutine(player1.NotificationCoroutine("FIGHT!"));
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

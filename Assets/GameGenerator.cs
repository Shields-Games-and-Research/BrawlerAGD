using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;


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

    public Player player;
    public Platforms platforms;
    private static string level_path = "Assets\\Game\\level.json";

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

        //Serialized Player 1 Setup
        SerializedPlayer serializedPlayer1 = new SerializedPlayer();
        serializedPlayer1.playerName = "Player 1";
        serializedPlayer1.jumpKey = KeyCode.W;
        serializedPlayer1.leftKey = KeyCode.A;
        serializedPlayer1.rightKey = KeyCode.D;
        serializedPlayer1.attackKey = KeyCode.S;
        serializedPlayer1.stocks = 3f;
        serializedPlayer1.velocity = 7f;
        serializedPlayer1.groundJumpForce = 5f;
        serializedPlayer1.airJumpForce = 8f;
        serializedPlayer1.mass = 1f;
        serializedPlayer1.drag = 1f;
        serializedPlayer1.widthScalar = 1f;
        serializedPlayer1.heightScalar = 1f;
        serializedPlayer1.gravityScalar = 1;
        serializedPlayer1.respawnX = -3f;
        serializedPlayer1.respawnY = 3f;
        serializedPlayer1.hitstunDamageScalar = 0.2f;

        //Serialized Player 1, Move 1 Setup
        SerializedMove serializedMove1Player1 = new SerializedMove();
        serializedMove1Player1.moveLocX = -1f;
        serializedMove1Player1.moveLocY = 0f;
        serializedMove1Player1.widthScalar = 1f;
        serializedMove1Player1.heightScalar = 1f;
        serializedMove1Player1.warmUpDuration = 0.1f;
        serializedMove1Player1.executionDuration = 0.2f;
        serializedMove1Player1.coolDownDuration = 0.1f;
        serializedMove1Player1.damageGiven = 5;
        serializedMove1Player1.knockbackScalar = 0.5f;
        serializedMove1Player1.knockbackModX = 0f;
        serializedMove1Player1.knockbackModY = 1f;
        serializedMove1Player1.hitstunDuration = 1f;

        //Player 1 Instantiation and Initialization
        Vector2 spawnLocationP1 = new Vector3(-2, 0);
        Player player1 = Instantiate(player, spawnLocationP1, Quaternion.identity);
        player1.playerDetails = p1HUD.GetComponent<Text>();
        InitializePlayerFromSerializedObj(serializedPlayer1, player1);
        InitializeMoveFromSerializedObj(serializedMove1Player1, player1.move1, player1);

        //Serialized Player 2 Setup
        SerializedPlayer serializedPlayer2 = new SerializedPlayer();
        serializedPlayer2.playerName = "Player 2";
        serializedPlayer2.jumpKey = KeyCode.I;
        serializedPlayer2.leftKey = KeyCode.J;
        serializedPlayer2.rightKey = KeyCode.L;
        serializedPlayer2.attackKey = KeyCode.K;
        serializedPlayer2.stocks = 5f;
        serializedPlayer2.velocity = 5f;
        serializedPlayer2.groundJumpForce = 8f;
        serializedPlayer2.airJumpForce = 4f;
        serializedPlayer2.mass = 2f;
        serializedPlayer2.drag = 1f;
        serializedPlayer2.widthScalar = 0.5f;
        serializedPlayer2.heightScalar = 1.5f;
        serializedPlayer2.gravityScalar = 0.8f;
        serializedPlayer2.respawnX = 3f;
        serializedPlayer2.respawnY = 3f;
        serializedPlayer2.hitstunDamageScalar = 0.1f;

        //Serialized Player 2 Move 1 Setup
        SerializedMove serializedMove1Player2 = new SerializedMove();
        serializedMove1Player2.moveLocX = 2f;
        serializedMove1Player2.moveLocY = 2f;
        serializedMove1Player2.widthScalar = 2f;
        serializedMove1Player2.heightScalar = 0.3f;
        serializedMove1Player2.warmUpDuration = 0.5f;
        serializedMove1Player2.executionDuration = 0.2f;
        serializedMove1Player2.coolDownDuration = 0.3f;
        serializedMove1Player2.damageGiven = 12;
        serializedMove1Player2.knockbackScalar = 0.75f;
        serializedMove1Player2.knockbackModX = 1f;
        serializedMove1Player2.knockbackModY = 0f;
        serializedMove1Player2.hitstunDuration = 0.8f;

        //Player 2 Instantiation
        Vector3 spawnLocationP2 = new Vector3(2, 0, 0);
        Player player2 = Instantiate(player, spawnLocationP2, Quaternion.identity);
        player2.playerDetails = p2HUD.GetComponent<Text>();
        InitializePlayerFromSerializedObj(serializedPlayer2, player2);
        InitializeMoveFromSerializedObj(serializedMove1Player2, player2.move1, player2);

}

    // Update is called once per frame
    void Update()
    {
        
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
    public float stocks;
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

    public SerializedPlayer()
    {
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

    public SerializedMove()
    {
    }
}

public class MapGenerator
{
    //
}

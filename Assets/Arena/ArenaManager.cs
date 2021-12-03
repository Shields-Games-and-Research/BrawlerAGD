using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Random = System.Random;
using System.Linq;
using UnityEngine.UI;
using static GameResult;
using UnityEngine.SceneManagement;
using static EvolutionManager;
using static UnityEngine.Random;



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
    public Text p1HUDText;
    public GameObject p2HUD;
    public Text p2HUDText;

    //UI components for notifications
    public GameObject notifications;
    public Text notificationsText;

    //Prefab Declaration
    //TODO: Match Prefab naming style "movePrefab"
    public Move move;
    public Player player;
    public Platforms platforms;

    //Player references for this game
    public Player player1;
    public SerializedPlayer serializedPlayer1;
    public SerializedMove serializedMove1Player1;
    
    public Player player2;
    public SerializedPlayer serializedPlayer2;
    public SerializedMove serializedMove1Player2;

    //Settings parameters for game
    public bool p1Playable;
    public bool p2Playable;
    public bool UIEnabled;

    public GameResult result;
    
    // Start is called before the first frame update
    void Start()
    {
        this.InitializeGame(false, false, true);
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void EndGame() 
    {
        //record game scores
        this.result.totalDamageP1 = this.player1.totalDamage;
        this.result.totalRecoveryStateTransitionP1 = this.player1.totalRecoveryStateTransition;
        this.result.totalHitsReceivedP1 = this.player1.totalHitsReceived;
        this.result.totalDamageP2 = this.player2.totalDamage;
        this.result.totalRecoveryStateTransitionP2 = this.player2.totalRecoveryStateTransition;
        this.result.totalHitsReceivedP2 = this.player2.totalHitsReceived;

        //send to evolution manager
        EvolutionManager.instance.AddResultFromGame(this.result);

        //destroy objects to preserve score - all other objects unloaded by unloading scene
        this.player1.destroy();
        this.player2.destroy();

        this.SaveGameJSON(1);
        

        //if (SceneManager.GetActiveScene().name == "EvolutionaryArenaManager")
        //{
            //Do specific Evolutionary Alg stuff here if necessary
        //}


    }

    public void InitializeGame(bool p1Playable, bool p2Playable, bool UIEnabled)
    {
        this.p1Playable = p1Playable;
        this.p2Playable = p2Playable;
        this.UIEnabled = UIEnabled;

        this.result = new GameResult();

        // Initialize RNG
        Random rand = new Random();

        // Generate / Load Map
        MapGenerator mapGen = new MapGenerator(2, 2, 3, 6, rand);
        platforms = mapGen.generate();

        // Load from or write to file
        platforms = ReadJson<Platforms>(levelPath, platforms);

        // Serialized Player 1 Setup
        this.serializedPlayer1 = new SerializedPlayer("Player 1", KeyCode.W, KeyCode.A, KeyCode.D, KeyCode.S, rand);
        this.serializedPlayer1 = ReadJson<SerializedPlayer>(player1Path, serializedPlayer1);
        this.serializedPlayer1.respawnX = platforms.player1x;
        this.serializedPlayer1.respawnY = platforms.player1y;

        // Serialized Player 1, Move 1 Setup
        this.serializedMove1Player1 = new SerializedMove(rand);
        this.serializedMove1Player1 = ReadJson<SerializedMove>(player1Move1Path, serializedMove1Player1);

        // Player 1 Instantiation
        Vector3 spawnLocationP1 = new Vector3(platforms.player1x, platforms.player1y, 0);
        this.player1 = Instantiate(player, spawnLocationP1, Quaternion.identity);
        player1.arenaManager = this;

        // Serialized Player 2 Setup
        this.serializedPlayer2 = new SerializedPlayer("Player 2", KeyCode.I, KeyCode.J, KeyCode.L, KeyCode.K, rand);
        this.serializedPlayer2 = ReadJson<SerializedPlayer>(player2Path, serializedPlayer2);
        this.serializedPlayer2.respawnX = platforms.player2x;
        this.serializedPlayer2.respawnY = platforms.player2y;

        // Serialized Player 2 Move 1 Setup
        this.serializedMove1Player2 = new SerializedMove(rand);
        this.serializedMove1Player2 = ReadJson<SerializedMove>(player2Move1Path, serializedMove1Player2);

        // Player 2 Instantiation
        Vector3 spawnLocationP2 = new Vector3(platforms.player2x, platforms.player2y, 0);
        this.player2 = Instantiate(player, spawnLocationP2, Quaternion.identity);
        player2.arenaManager = this;

        if (UIEnabled)
        {
            //Player 1 Heads Up Display
            p1HUDText = p1HUD.GetComponent<Text>();
            player1.playerDetails = p1HUDText;
            //Player 2 Heads Up Display
            p2HUDText = p2HUD.GetComponent<Text>();
            player2.playerDetails = p2HUDText;
            //notifications
            notificationsText = notifications.GetComponent<Text>();
        }
        else 
        {
            Destroy(p1HUD);
            Destroy(p2HUD);
            Destroy(notifications);
        }

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
        
        player1.InitializePlayerFromSerializedObj(serializedPlayer1);
        player1.InitializeMoveFromSerializedObj(serializedMove1Player1);
        player2.InitializePlayerFromSerializedObj(serializedPlayer2);
        player2.InitializeMoveFromSerializedObj(serializedMove1Player2);

        StartCoroutine(NotificationCoroutine("FIGHT!"));

        //TODO: TimeScale Adjustment
        //Time.timeScale = 1.0f;
        //Time.fixedDeltaTime = Time.fixedDeltaTime * Time.timeScale;
        
    }

    /** checks to see if a file exists, if it doesn't, creates it and generates randomly
     */
    T ReadJson<T>(string filename, T ifFileMissing)
    {
        // Write to file
        if (!File.Exists(filename))
        {
            WriteJson<T>(filename, ifFileMissing);
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


    public void WriteJson<T>(string filename, T serializedObj) 
    {
        string serializedJSON = JsonUtility.ToJson(serializedObj);
        File.Create(filename).Dispose();
        File.WriteAllText(filename, serializedJSON);
        Debug.Log("File Saved: " + filename);
    }

    /**Saves the current arena's settings to JSON files according to the gameID in gameData
     */
    public bool SaveGameJSON() 
    {
        this.SaveGameJSON(this.result.gameID);
        return false;
    }

    /**Saves the current game based on a parameter
     */
    public bool SaveGameJSON(int gameID)
    {
        string tempDirectoryPath = "Assets\\Game\\game" + gameID + "\\";
        if (!File.Exists(tempDirectoryPath)) 
        {
            Directory.CreateDirectory(tempDirectoryPath);
        }

        //write level JSON to  
        string tempLevelPath = tempDirectoryPath + "level.json";        


        string tempPlayer1Path = tempDirectoryPath + "player1.json";
        if (!File.Exists(tempPlayer1Path))
        {
            this.WriteJson<SerializedPlayer>(tempPlayer1Path, this.serializedPlayer1);
        }

        string tempPlayer2Path = tempDirectoryPath + "player2.json";
        if (!File.Exists(tempPlayer2Path)) 
        {
            this.WriteJson<SerializedPlayer>(tempPlayer2Path, this.serializedPlayer2);
        }

        string tempPlayer1Move1Path = tempDirectoryPath + "p1move1.json";
        if (!File.Exists(tempPlayer1Move1Path)) 
        {
            this.WriteJson<SerializedMove>(tempPlayer1Move1Path, this.serializedMove1Player1);
        }

        string tempPlayer2Move1Path = tempDirectoryPath + "p2move1.json";
        if (!File.Exists(tempPlayer2Move1Path)) 
        {
            this.WriteJson<SerializedMove>(tempPlayer2Move1Path, this.serializedMove1Player2);
        }
        
        string tempGameResultPath = tempDirectoryPath + "gameresult.json";
        if (!File.Exists(tempGameResultPath)) 
        {
            this.WriteJson<GameResult>(tempGameResultPath, this.result);
        }



        return false;
        
    }

    //UI Control for this game
    public void UpdateNotifications(string message) 
    {
        if (UIEnabled)
        {
            notificationsText.text = message;
        }
    }

    public void ClearNotifications() 
    {
        if (UIEnabled)
        {
            notificationsText.text = "";
        }
    }

    public IEnumerator NotificationCoroutine(string message) 
    {
        UpdateNotifications(message);
        yield return new WaitForSeconds(5f);
        ClearNotifications();
    }
   
}

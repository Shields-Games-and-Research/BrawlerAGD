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
    private static string levelPath = "\\level.json";
    private static string player1Path = "\\player1.json";
    private static string player2Path = "\\player2.json";
    private static string player1Move1Path = "\\p1move1.json";
    private static string player2Move1Path = "\\p2move1.json";
    private static string gameResultPath = "\\gameresult.json";

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

    //Result of game stored here
    public GameResult result;
    
    // Start is called before the first frame update
    void Start()
    {
        this.InitializeGameByGameID(1, false, false, true);
        
    }

    // Update is called once per frame
    void Update()
    {

    }


    /** If there is a folder/file there, read it, if not, generate a new random instance for the game
     */
    public void InitializeGameByGameID(int gameID, bool p1Playable, bool p2Playable, bool UIEnabled) 
    {
        string tempDirectoryPath = "Assets\\Game\\game" + gameID;
        if (!Directory.Exists(tempDirectoryPath))
        {
            print("GENERATING NEW GAME");
            //Create new game if one does not exist
            Directory.CreateDirectory(tempDirectoryPath);
            this.GenerateNewGame(gameID, p1Playable, p2Playable, UIEnabled);
        }
        //Read from file
        else
        {
            print("READING FROM FILE");
            //Create new results no matter what
            this.result = new GameResult();
            this.result.gameID = gameID;

            // Initialize RNG
            Random rand = new Random();

            // Read platforms from file
            this.platforms = this.ReadJson<Platforms>(tempDirectoryPath + levelPath, this.platforms);
            // Read player from file
            this.serializedPlayer1 = this.ReadJson<SerializedPlayer>(tempDirectoryPath + player1Path, serializedPlayer1);
            this.serializedPlayer1.respawnX = platforms.player1x;
            this.serializedPlayer1.respawnY = platforms.player1y;

            // Serialized Player 1, Move 1 Setup
            this.serializedMove1Player1 = this.ReadJson<SerializedMove>(tempDirectoryPath + player1Move1Path, serializedMove1Player1);

            // Player 1 Instantiation
            Vector3 spawnLocationP1 = new Vector3(platforms.player1x, platforms.player1y, 0);
            this.player1 = Instantiate(player, spawnLocationP1, Quaternion.identity);
            this.player1.arenaManager = this;

            // Serialized Player 2 Setup
            this.serializedPlayer2 = this.ReadJson<SerializedPlayer>(tempDirectoryPath + player2Path, serializedPlayer2);
            this.serializedPlayer2.respawnX = platforms.player2x;
            this.serializedPlayer2.respawnY = platforms.player2y;

            // Serialized Player 2 Move 1 Setup
            this.serializedMove1Player2 = this.ReadJson<SerializedMove>(tempDirectoryPath + player2Move1Path, serializedMove1Player2);

            // Player 2 Instantiation
            Vector3 spawnLocationP2 = new Vector3(platforms.player2x, platforms.player2y, 0);
            this.player2 = Instantiate(player, spawnLocationP2, Quaternion.identity);
            this.player2.arenaManager = this;

            //update gameobjects instantiated into the scene with values from JSON
            this.player1.InitializePlayerFromSerializedObj(serializedPlayer1);
            this.player1.InitializeMoveFromSerializedObj(serializedMove1Player1);
            this.player2.InitializePlayerFromSerializedObj(serializedPlayer2);
            this.player2.InitializeMoveFromSerializedObj(serializedMove1Player2);

            StartCoroutine(NotificationCoroutine("FIGHT!"));

            //TODO: Move this to constructors
            this.serializedPlayer1.spriteIndex = this.player1.spriteIndex;
            this.serializedPlayer2.spriteIndex = this.player2.spriteIndex;
            this.serializedMove1Player1.spriteIndex = this.player1.move1.spriteIndex;
            this.serializedMove1Player2.spriteIndex = this.player2.move1.spriteIndex;

            //Set overall game options
            this.SetGameOptions(UIEnabled, p1Playable, p2Playable);
        }
    }

    public void GenerateNewGame(int gameID, bool p1Playable, bool p2Playable, bool UIEnabled) 
    {
        // Generate all objects from scratch
        this.result = new GameResult();
        this.result.gameID = gameID;

        // Initialize RNG
        Random rand = new Random();

        // Generate / Load Map
        MapGenerator mapGen = new MapGenerator(2, 2, 3, 6, rand);
        platforms = mapGen.generate();

        // Serialized Player 1 Setup
        this.serializedPlayer1 = new SerializedPlayer("Player 1", KeyCode.W, KeyCode.A, KeyCode.D, KeyCode.S, rand);
        this.serializedPlayer1.respawnX = platforms.player1x;
        this.serializedPlayer1.respawnY = platforms.player1y;

        // Serialized Player 1, Move 1 Setup
        this.serializedMove1Player1 = new SerializedMove(rand);

        // Player 1 Instantiation
        Vector3 spawnLocationP1 = new Vector3(platforms.player1x, platforms.player1y, 0);
        this.player1 = Instantiate(player, spawnLocationP1, Quaternion.identity);
        player1.arenaManager = this;

        // Serialized Player 2 Setup
        this.serializedPlayer2 = new SerializedPlayer("Player 2", KeyCode.I, KeyCode.J, KeyCode.L, KeyCode.K, rand);
        this.serializedPlayer2.respawnX = platforms.player2x;
        this.serializedPlayer2.respawnY = platforms.player2y;

        // Serialized Player 2 Move 1 Setup
        this.serializedMove1Player2 = new SerializedMove(rand);

        // Player 2 Instantiation
        Vector3 spawnLocationP2 = new Vector3(platforms.player2x, platforms.player2y, 0);
        this.player2 = Instantiate(player, spawnLocationP2, Quaternion.identity);
        player2.arenaManager = this;

        //update gameobjects instantiated into the scene with values from JSON
        player1.InitializePlayerFromSerializedObj(serializedPlayer1);
        player1.InitializeMoveFromSerializedObj(serializedMove1Player1);
        player2.InitializePlayerFromSerializedObj(serializedPlayer2);
        player2.InitializeMoveFromSerializedObj(serializedMove1Player2);

        //Set overall game options
        this.SetGameOptions(UIEnabled, p1Playable, p2Playable);

        //TODO: Move this to constructors
        this.serializedPlayer1.spriteIndex = this.player1.spriteIndex;
        this.serializedPlayer2.spriteIndex = this.player2.spriteIndex;
        this.serializedMove1Player1.spriteIndex = this.player1.move1.spriteIndex;
        this.serializedMove1Player2.spriteIndex = this.player2.move1.spriteIndex;

        //Save game to folder for next generation
        this.SaveGameJSON(result.gameID);
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

        this.SaveGameJSON(result.gameID);

        //destroy objects to preserve score - all other objects unloaded by unloading scene
        this.player1.destroy();
        this.player2.destroy();
    }

    /** Sets various arena-level settings, mostly for testing/debug
     */
    public void SetGameOptions(bool UIEnabled, bool p1Playable, bool p2Playable) 
    {
        if (UIEnabled)
        {
            this.InitUI();
        }
        else
        {
            this.DestroyUI();
        }

        //Player Controller or Agent Assignment
        if (p1Playable)
        {
            this.SetPlayerToWASD(player1);
        }
        else
        {
            this.SetPlayerToCPU(player1, player2);
        }
        if (p2Playable)
        {
            this.SetPlayerToIJKL(player2);
        }
        else
        {
            this.SetPlayerToCPU(player2, player1);
        }
    }

    public void InitUI() 
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

    public void DestroyUI() 
    {
        Destroy(p1HUD);
        Destroy(p2HUD);
        Destroy(notifications);
    }

    public void SetPlayerToCPU(Player player, Player opponent) 
    {
        player.controller = new AI(player, opponent);
    }
    
    public void SetPlayerToWASD(Player player) 
    {
        player.controller = new Controller(player1, null);
        player.controller.leftKey = KeyCode.A;
        player.controller.rightKey = KeyCode.D;
        player.controller.jumpKey = KeyCode.W;
        player.controller.move1Key = KeyCode.S;
    }
    
    public void SetPlayerToIJKL(Player player) 
    {
        player.controller = new Controller(player2, null);
        player.controller.leftKey = KeyCode.J;
        player.controller.rightKey = KeyCode.L;
        player.controller.jumpKey = KeyCode.I;
        player.controller.move1Key = KeyCode.K;
    }


    //TODO: Refactor this into a utility object
    /** checks to see if a file exists, if it doesn't, creates it and generates randomly
     */
    T ReadJson<T>(string filename, T ifFileMissing)
    {
        print("filename reading: " + filename);
        // Write to file
        if (!File.Exists(filename))
        {
            throw new FileNotFoundException("Attempting to read JSON failed");
        }
        // If the file exists, read from it
        else
        {
            string rawSerializedObj = File.ReadAllText(filename);
            T serializedObj = JsonUtility.FromJson<T>(rawSerializedObj);
            print(serializedObj);
            return serializedObj;
        }
    }


    public void WriteJson<T>(string filename, T serializedObj) 
    {
        string serializedJSON = JsonUtility.ToJson(serializedObj);
        File.WriteAllText(filename, serializedJSON);
        Debug.Log("File Saved: " + filename);
    }

    /**Saves the current arena's settings to JSON files according to the gameID in gameData
     */
    public void SaveGameJSON() 
    {
        this.SaveGameJSON(this.result.gameID);
    }

    /**Saves the current game based on a parameter
     */
    public void SaveGameJSON(int gameID)
    {

        //Create a directory if non exist
        string tempDirectoryPath = "Assets\\Game\\game" + gameID + "\\";
        if (!File.Exists(tempDirectoryPath)) 
        {
            Directory.CreateDirectory(tempDirectoryPath);
        }

        string tempLevelPath = tempDirectoryPath + "level.json";
        this.WriteJson<Platforms>(tempLevelPath, this.platforms);
        string tempPlayer1Path = tempDirectoryPath + "player1.json";
        this.WriteJson<SerializedPlayer>(tempPlayer1Path, this.serializedPlayer1);
        string tempPlayer2Path = tempDirectoryPath + "player2.json";
        this.WriteJson<SerializedPlayer>(tempPlayer2Path, this.serializedPlayer2);
        string tempPlayer1Move1Path = tempDirectoryPath + "p1move1.json";
        this.WriteJson<SerializedMove>(tempPlayer1Move1Path, this.serializedMove1Player1);
        string tempPlayer2Move1Path = tempDirectoryPath + "p2move1.json";
        this.WriteJson<SerializedMove>(tempPlayer2Move1Path, this.serializedMove1Player2);
        string tempGameResultPath = tempDirectoryPath + "gameresult.json";
        this.WriteJson<GameResult>(tempGameResultPath, this.result);
        
       
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

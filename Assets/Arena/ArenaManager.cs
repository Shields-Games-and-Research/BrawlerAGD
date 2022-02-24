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


public static class Consts
{
    public static string GAME_PATH = "Assets\\Game\\game";
    public static string EVO_RESULTS_PATH = "Assets\\Game\\evoresults";
    public static string HIGH_FITNESS_GAMES = "Assets\\Game\\randomfitness\\";
    public static string RESEARCH_RESULTS = "Assets\\Game\\research\\results\\";
    public static string RESEARCH_GAME = "Assets\\Game\\research\\game\\";
    public static string TUTORIAL_RESULTS = "Assets\\Game\\tutorial\\results";
    public static string TUTORIAL_GAME = "Assets\\Game\\tutorial\\game";
    //TODO: File management approach
    public static string LEVEL_PATH = "\\level.json";
    public static string PLAYER1_PATH = "\\player1.json";
    public static string PLAYER2_PATH = "\\player2.json";
    public static string PLAYER1MOVE1_PATH = "\\p1move1.json";
    public static string PLAYER2MOVE1_PATH = "\\p2move1.json";
    public static string GAME_RESULT_PATH = "\\gameresult.json";
    public static string RESULTS_FILE_PATH = "\\results.json";
}


public class ArenaManager : MonoBehaviour
{

    //Settings parameters for game
    public bool p1Playable;
    public bool p2Playable;
    public bool UIEnabled;

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

    //Result of game stored here
    public GameResult result;

    //Game Time Management
    public float startTime;

    //Game length in seconds
    public float gameLength;
    
    // Start is called before the first frame update
    void Start()
    {
        //Load existing game settings
        //TODO: Use Game Settings for initializing all games
        if (GameSettings.instance == null)
        {
            Debug.Log("Arena initializing with GameID: " + EvolutionManager.instance.currentGameID);
            this.InitializeGameByGameID(EvolutionManager.instance.currentGameID, false, false, true);
            this.startTime = Time.time;
        }
        else if (GameSettings.instance.loadWithTutorialController) 
        {
            Debug.Log("Arena initializing with Path: " + GameSettings.instance.loadGamePath);
            this.InitializeTutorialByPath(GameSettings.instance.loadGamePath, true, true, true);
            this.startTime = Time.time;
        }
        else
        {
            Debug.Log("Arena initializing with Path: " + GameSettings.instance.loadGamePath);
            this.InitializeGameByPath(GameSettings.instance.loadGamePath, true, true, true);
            this.startTime = Time.time;
        }
    }

    // Update is called once per frame
    void Update()
    {
        this.gameLength = Time.time - this.startTime;


        //End if this arena has lasted longer than 60 seconds
        if (GameSettings.instance == null)
        {
            if (gameLength >= EvolutionManager.instance.maxGameLength)
            {
                Debug.Log("This game has exceeded the evolution manager's max game length");
                this.EndGame("draw");
            }
        }

    }


    /** If there is a folder/file there, read it, if not, generate a new random instance for the game */
    public void InitializeGameByPath(string path, bool p1Playable, bool p2Playable, bool UIEnabled)
    {
        this.p1Playable = p1Playable;
        this.p2Playable = p2Playable;
        this.UIEnabled = UIEnabled;

        if (!Directory.Exists(path))
        {
            Debug.Log("ERROR: ATTEMPTING TO READ FILE THAT DOES NOT EXIST");
        }
        //Read from file
        else
        {
            Debug.Log("LOADING GAME FROM FILE: " + path);
            this.ReadGame(path);
        }

        // Compute spawn locations
        // Player 1 spawns on the initial platform
        Platform initialPlatform = this.platforms.platformList[0];
        int player1Spawnx = (int)initialPlatform.x + (initialPlatform.xSize + 1) / 2;
        int player1Spawny = initialPlatform.y + initialPlatform.ySize + 2;
        //ensure p1 spawn is safe before committing, move up if not
        while (!this.SpawnIsSafe(player1Spawnx, player1Spawny, this.platforms.platformList))
        {
            player1Spawny += 1;
        }
        Vector2 player1Spawn = new Vector2(player1Spawnx, player1Spawny);
        // Mirror Player 2's spawn relative to Player 1's
        int player2Spawnx = -player1Spawnx;
        int player2Spawny = player1Spawny;
        //ensure p2 spawn is safe before committing, move up if not
        while (!this.SpawnIsSafe(player2Spawnx, player2Spawny, this.platforms.platformList))
        {
            player2Spawny += 1;
        }
        Vector2 player2Spawn = new Vector2(player2Spawnx, player2Spawny);

        // Player 1 Instantiation
        Vector3 spawnLocationP1 = new Vector3(player1Spawnx, player1Spawny, 0);
        this.player1 = Instantiate(player, spawnLocationP1, Quaternion.identity);
        player1.arenaManager = this;

        // Player 2 Instantiation
        Vector3 spawnLocationP2 = new Vector3(player2Spawnx, player2Spawny, 0);
        this.player2 = Instantiate(player, spawnLocationP2, Quaternion.identity);
        player2.arenaManager = this;

        //update gameobjects instantiated into the scene with values from JSON
        player1.InitializePlayerFromSerializedObj(this.serializedPlayer1, player1Spawn);
        player1.InitializeMoveFromSerializedObj(this.serializedMove1Player1);
        player2.InitializePlayerFromSerializedObj(this.serializedPlayer2, player2Spawn);
        player2.InitializeMoveFromSerializedObj(this.serializedMove1Player2);

        //Set overall game options
        this.SetGameOptions();

        //Save game to folder for next generation
        this.SaveGameJSON(result.gameID);
    }

    /** If there is a folder/file there, read it, if not, generate a new random instance for the game */
    public void InitializeTutorialByPath(string path, bool p1Playable, bool p2Playable, bool UIEnabled)
    {
        this.p1Playable = p1Playable;
        this.p2Playable = p2Playable;
        this.UIEnabled = UIEnabled;

        if (!Directory.Exists(path))
        {
            Debug.Log("ERROR: ATTEMPTING TO READ FILE THAT DOES NOT EXIST");
        }
        //Read from file
        else
        {
            Debug.Log("LOADING GAME FROM FILE: " + path);
            this.ReadGame(path);
        }

        // Compute spawn locations
        // Player 1 spawns in left side of screen
        Platform initialPlatform = this.platforms.platformList[0];
        float player1Spawnx = -6f;
        float player1Spawny = 0f;
        Vector2 player1Spawn = new Vector2(player1Spawnx, player1Spawny);
        // Mirror Player 2's spawn relative to Player 1's
        float player2Spawnx = 6f;
        float player2Spawny = 0f;
        Vector2 player2Spawn = new Vector2(player2Spawnx, player2Spawny);

        // Player 1 Instantiation
        Vector3 spawnLocationP1 = new Vector3(player1Spawnx, player1Spawny, 0);
        this.player1 = Instantiate(player, spawnLocationP1, Quaternion.identity);
        player1.arenaManager = this;

        // Player 2 Instantiation
        Vector3 spawnLocationP2 = new Vector3(player2Spawnx, player2Spawny, 0);
        this.player2 = Instantiate(player, spawnLocationP2, Quaternion.identity);
        player2.arenaManager = this;

        //update gameobjects instantiated into the scene with values from JSON
        player1.InitializePlayerFromSerializedObj(this.serializedPlayer1, player1Spawn);
        player1.InitializeMoveFromSerializedObj(this.serializedMove1Player1);
        player2.InitializePlayerFromSerializedObj(this.serializedPlayer2, player2Spawn);
        player2.InitializeMoveFromSerializedObj(this.serializedMove1Player2);

        //Create two NPCs with jump AI, update their names, set to jumping controller
        Vector3 spawnLocationDummy1 = new Vector3(-3f, 0f, 0f);
        Vector3 spawnLocationDummy2 = new Vector3(3f, 0f, 0f);
        Player player1Dummy = Instantiate(player, spawnLocationDummy1, Quaternion.identity);
        Player player2Dummy = Instantiate(player, spawnLocationDummy2, Quaternion.identity);
        player1Dummy.arenaManager = this;
        player2Dummy.arenaManager = this;
        player1Dummy.isDummy = true;
        player2Dummy.isDummy = true;
        this.SetPlayerToJumpCPU(player1Dummy, this.player1);
        this.SetPlayerToJumpCPU(player2Dummy, this.player2);
        player1Dummy.InitializePlayerFromSerializedObj(this.serializedPlayer2, new Vector2(-3f, 0f));
        player1Dummy.InitializeMoveFromSerializedObj(this.serializedMove1Player2);
        player2Dummy.InitializePlayerFromSerializedObj(this.serializedPlayer1, new Vector2(3f, 0f));
        player2Dummy.InitializeMoveFromSerializedObj(this.serializedMove1Player1);
        player1Dummy.playerName = "Training Dummy 1";
        player2Dummy.playerName = "Training Dummy 2";


        //Set overall game options
        this.SetGameOptions();

    }

    /** If there is a folder/file there, read it, if not, generate a new random instance for the game
     */
    public void InitializeGameByGameID(int gameID, bool p1Playable, bool p2Playable, bool UIEnabled) 
    {
        this.p1Playable = p1Playable;
        this.p2Playable = p2Playable;
        this.UIEnabled = UIEnabled;

        string tempDirectoryPath = Consts.GAME_PATH + gameID;
        if (!Directory.Exists(tempDirectoryPath))
        {
            print("GENERATING NEW GAME");
            //Create new game if one does not exist
            Directory.CreateDirectory(tempDirectoryPath);
            this.GenerateGame(gameID);
            this.SaveGameJSON(gameID);  
        }
        //Read from file
        else
        {
            print("READING FROM FILE");
            this.ReadGame(tempDirectoryPath);
        } 

        // Compute spawn locations
        // Player 1 spawns on the initial platform
        Platform initialPlatform = this.platforms.platformList[0];
        int player1Spawnx = (int)initialPlatform.x + (initialPlatform.xSize + 1) / 2;
        int player1Spawny = initialPlatform.y + initialPlatform.ySize + 2;
        //ensure p1 spawn is safe before committing, move up if not
        while (!this.SpawnIsSafe(player1Spawnx, player1Spawny, this.platforms.platformList))
        {
            player1Spawny += 1;
        }
        Vector2 player1Spawn = new Vector2(player1Spawnx, player1Spawny);
        // Mirror Player 2's spawn relative to Player 1's
        int player2Spawnx = -player1Spawnx;
        int player2Spawny = player1Spawny;
        //ensure p2 spawn is safe before committing, move up if not
        while (!this.SpawnIsSafe(player2Spawnx, player2Spawny, this.platforms.platformList))
        {
            player2Spawny += 1;
        }
        Vector2 player2Spawn = new Vector2(player2Spawnx, player2Spawny);

        // Player 1 Instantiation
        Vector3 spawnLocationP1 = new Vector3(player1Spawnx, player1Spawny, 0);
        this.player1 = Instantiate(player, spawnLocationP1, Quaternion.identity);
        player1.arenaManager = this;

        // Player 2 Instantiation
        Vector3 spawnLocationP2 = new Vector3(player2Spawnx, player2Spawny, 0);
        this.player2 = Instantiate(player, spawnLocationP2, Quaternion.identity);
        player2.arenaManager = this;

        //update gameobjects instantiated into the scene with values from JSON
        player1.InitializePlayerFromSerializedObj(this.serializedPlayer1, player1Spawn);
        player1.InitializeMoveFromSerializedObj(this.serializedMove1Player1);
        player2.InitializePlayerFromSerializedObj(this.serializedPlayer2, player2Spawn);
        player2.InitializeMoveFromSerializedObj(this.serializedMove1Player2);

        //Save game to folder for next generation
        this.SaveGameJSON(result.gameID);

        //Set overall game options
        this.SetGameOptions();

    }

    public bool SpawnIsSafe(int spawnX, int spawnY, List<Platform> platforms) 
    {
        foreach(Platform platform in platforms) 
        {
            if (this.SpawnInPlatform(spawnX, spawnY, platform.x, platform.xSize, platform.y, platform.ySize)) 
            {
                return false;
            }
        }
        return true;
    }

    public bool SpawnInPlatform(int spawnX, int spawnY, int platX, int platSizeX, int platY, int platSizeY)
    {
        bool intersectsX = (spawnX >= platX) && (spawnX <= (platX + platSizeX));
        bool intersectsY = (spawnY >= platY) && (spawnY <= (platY + platSizeY));
        return (intersectsX && intersectsY);
    }

    public void GenerateGame(int gameID)
    {
        // Initialize RNG
        Random rand = EvolutionManager.instance.rand;

        // Create GameResults Object
        this.result = new GameResult();
        this.result.gameID = gameID;
        this.result.generationNum = EvolutionManager.instance.currGeneration;

        // Generate / Load Map
        MapGenerator mapGen = new MapGenerator(2, 2, 3, 6, rand);
        this.platforms = mapGen.generate();

        // Serialized Player 1 Setup
        this.serializedPlayer1 = new SerializedPlayer("Player 1", rand);

        // Serialized Player 1, Move 1 Setup
        this.serializedMove1Player1 = new SerializedMove(rand);

        // Serialized Player 2 Setup
        this.serializedPlayer2 = new SerializedPlayer("Player 2", rand);

        // Serialized Player 2 Move 1 Setup
        this.serializedMove1Player2 = new SerializedMove(rand);
    }

    public void ReadGame(string tempDirectoryPath)
    {
        // Read platforms from file
        this.platforms = this.ReadJson<Platforms>(tempDirectoryPath + Consts.LEVEL_PATH);
        // Read player from file
        this.serializedPlayer1 = this.ReadJson<SerializedPlayer>(tempDirectoryPath + Consts.PLAYER1_PATH);
        // Serialized Player 1, Move 1 Setup
        this.serializedMove1Player1 = this.ReadJson<SerializedMove>(tempDirectoryPath + Consts.PLAYER1MOVE1_PATH);
        // Serialized Player 2 Setup
        this.serializedPlayer2 = this.ReadJson<SerializedPlayer>(tempDirectoryPath + Consts.PLAYER2_PATH);
        // Serialized Player 2 Move 1 Setup
        this.serializedMove1Player2 = this.ReadJson<SerializedMove>(tempDirectoryPath + Consts.PLAYER2MOVE1_PATH);
        // Game Result Setup
        // Generate all objects from scratch
        GameResult oldResult = this.ReadJson<GameResult>(tempDirectoryPath + Consts.GAME_RESULT_PATH);
        this.result = new GameResult();
        this.result.gameID = oldResult.gameID;
        this.result.generationNum = oldResult.generationNum;
    }

    public void EndGame(string loser)
    {
        //record game scores
        this.result.totalDamageP1 = this.player1.totalDamage;
        this.result.totalRecoveryStateTransitionP1 = this.player1.totalRecoveryStateTransition;
        this.result.totalHitsReceivedP1 = this.player1.totalHitsReceived;
        this.result.remainingStocksP1 = this.player1.stocks;
        this.result.totalDamageP2 = this.player2.totalDamage;
        this.result.totalRecoveryStateTransitionP2 = this.player2.totalRecoveryStateTransition;
        this.result.totalHitsReceivedP2 = this.player2.totalHitsReceived;
        this.result.remainingStocksP2 = this.player2.stocks;
        this.result.totalGameLength = this.gameLength;
        this.result.loser = loser;
        //send to evolution manager
        if (GameSettings.instance == null)
        {
            //update evolution manager
            EvolutionManager.instance.AddResultFromGame(this.result);
            //Save to file
            this.SaveGameJSON(result.gameID);
        }
        else if (!GameSettings.instance.loadWithTutorialController)
        {
            //Evaluate Results
            this.result.evaluateHumanGame();
            //Save to research results
            SaveGameJSONtoResultsFolder(GameSettings.instance.resultsPath);
            //return to main menu
            StartCoroutine(this.ReturnToMenuCoroutine());
        }
        else
        { 
        }

        //destroy objects to preserve score - all other objects unloaded by unloading scene
        this.player1.destroy();
        this.player2.destroy();
    }

    /** Sets various arena-level settings, mostly for testing/debug
     */
    public void SetGameOptions() 
    {
        if (this.UIEnabled)
        {
            this.InitUI();
        }
        else
        {
            this.DestroyUI();
        }

        //Player Controller or Agent Assignment
        if (this.p1Playable)
        {
            this.SetPlayerToWASD(player1);
        }
        else
        {
            this.SetPlayerToCPU(player1, player2);
        }
        if (this.p2Playable)
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
        this.p1HUDText = this.p1HUD.GetComponent<Text>();
        this.player1.playerDetails = this.p1HUDText;
        //Player 2 Heads Up Display
        this.p2HUDText = this.p2HUD.GetComponent<Text>();
        this.player2.playerDetails = this.p2HUDText;
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

    public void SetPlayerToJumpCPU(Player player, Player opponent) 
    {
        player.controller = new HoldJump(player, opponent);
    }
    
    public void SetPlayerToWASD(Player player) 
    {
        player.controller = new Controller(player1, null);
        player.controller.SetPlayer1Buttons();
        //player.controller.leftKey = KeyCode.A;
        //player.controller.rightKey = KeyCode.D;
        //player.controller.jumpKey = KeyCode.W;
        //player.controller.move1Key = KeyCode.S;
    }
    
    public void SetPlayerToIJKL(Player player) 
    {
        player.controller = new Controller(player2, null);
        player.controller.SetPlayer2Buttons();
        //player.controller.leftKey = KeyCode.J;
        //player.controller.rightKey = KeyCode.L;
        //player.controller.jumpKey = KeyCode.I;
        //player.controller.move1Key = KeyCode.K;
    }


    //TODO: Refactor this into a utility object
    /** checks to see if a file exists, if it doesn't, creates it and generates randomly
     */
    T ReadJson<T>(string filename)
    {
        //print("filename reading: " + filename);
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
            //print(serializedObj);
            return serializedObj;
        }
    }

    public void WriteJson<T>(string filename, T serializedObj) 
    {
        string serializedJSON = JsonUtility.ToJson(serializedObj);
        File.WriteAllText(filename, serializedJSON);
    }

    /**Saves the current game based on a parameter
     */
    public void SaveGameJSON(int gameID)
    {
        Debug.Log("Saving game with ID to disk: " + gameID);
        //Create a directory if non exist
        string tempDirectoryPath = Consts.GAME_PATH + gameID;
        if (!File.Exists(tempDirectoryPath)) 
        {
            Directory.CreateDirectory(tempDirectoryPath);
        }
        string tempLevelPath = tempDirectoryPath + Consts.LEVEL_PATH;
        this.WriteJson<Platforms>(tempLevelPath, this.platforms);
        string tempPlayer1Path = tempDirectoryPath + Consts.PLAYER1_PATH;
        this.WriteJson<SerializedPlayer>(tempPlayer1Path, this.serializedPlayer1);
        string tempPlayer2Path = tempDirectoryPath + Consts.PLAYER2_PATH;
        this.WriteJson<SerializedPlayer>(tempPlayer2Path, this.serializedPlayer2);
        string tempPlayer1Move1Path = tempDirectoryPath + Consts.PLAYER1MOVE1_PATH;
        this.WriteJson<SerializedMove>(tempPlayer1Move1Path, this.serializedMove1Player1);
        string tempPlayer2Move1Path = tempDirectoryPath + Consts.PLAYER2MOVE1_PATH;
        this.WriteJson<SerializedMove>(tempPlayer2Move1Path, this.serializedMove1Player2);
        string tempGameResultPath = tempDirectoryPath + Consts.GAME_RESULT_PATH;
        this.WriteJson<GameResult>(tempGameResultPath, this.result);
    }

    public void SaveGameJSONtoResultsFolder(string gamePath)
    {
        Debug.Log("Saving research results to path: " + gamePath);
        if (!File.Exists(gamePath))
        {
            Directory.CreateDirectory(gamePath);
        }
        string tempLevelPath = gamePath + Consts.LEVEL_PATH;
        this.WriteJson<Platforms>(tempLevelPath, this.platforms);
        string tempPlayer1Path = gamePath + Consts.PLAYER1_PATH;
        this.WriteJson<SerializedPlayer>(tempPlayer1Path, this.serializedPlayer1);
        string tempPlayer2Path = gamePath + Consts.PLAYER2_PATH;
        this.WriteJson<SerializedPlayer>(tempPlayer2Path, this.serializedPlayer2);
        string tempPlayer1Move1Path = gamePath + Consts.PLAYER1MOVE1_PATH;
        this.WriteJson<SerializedMove>(tempPlayer1Move1Path, this.serializedMove1Player1);
        string tempPlayer2Move1Path = gamePath + Consts.PLAYER2MOVE1_PATH;
        this.WriteJson<SerializedMove>(tempPlayer2Move1Path, this.serializedMove1Player2);
        string tempGameResultPath = gamePath + Consts.GAME_RESULT_PATH;
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

    public IEnumerator ReturnToMenuCoroutine() 
    {
        yield return new WaitForSeconds(5f);
        UpdateNotifications("Returning to study menu.");
        yield return new WaitForSeconds(5f);
        ClearNotifications();
        SceneManager.LoadSceneAsync("LoadGame", LoadSceneMode.Single);
    }
   
}

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
using TMPro;
using static GameSettings;

public class ArenaManager : MonoBehaviour
{

    //Settings parameters for game
    public bool p1Playable;
    public bool p2Playable;
    public bool UIEnabled;

    //UI components for each player
    public GameObject p1HUD;
    public TextMeshProUGUI p1HUDText;
    public GameObject p2HUD;
    public TextMeshProUGUI p2HUDText;
    public GameObject p1H1;
    public GameObject p1H2;
    public GameObject p1H3;
    public GameObject p2H1;
    public GameObject p2H2;
    public GameObject p2H3;
    
    //UI components for pause screen
    public GameObject pauseMenuUI;
    public GameObject pauseButton;

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
    
    public static bool evo;

    public float timeScale;

    //Indicates if game is paused
    public bool gameIsPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        bool p1IsHuman = GameSettings.instance.p1IsHuman;
        bool p2IsHuman = GameSettings.instance.p2IsHuman;
        bool UIEnabled = GameSettings.instance.UIEnabled;

        string loadPath = GameSettings.instance.loadGamePath;


        this.startTime = Time.time;
        print("Attempting to load file. " + loadPath);
        this.pauseMenuUI.SetActive(false);

        //On loading the arena, check the GameSettings file to ensure appropriate files are loaded.
        switch (GameSettings.instance.mode) 
        {
            //Loading Arena in an Evolution Context
            case GameSettings.GameMode.EVO:
                this.InitializeGameByGameID(EvolutionManager.instance.currentGameID, p1IsHuman, p2IsHuman, UIEnabled);
                break;
            //Loading Arena in from a study file
            case GameSettings.GameMode.LOAD:
                Debug.Log(Constants.INITIALIZING_ARENA + GameSettings.instance.loadGamePath);
                this.InitializeGameByPath(loadPath, p1IsHuman, p2IsHuman, UIEnabled);
                break;
            //Loading Arena in from a user-specified file
            case GameSettings.GameMode.LOADDISK:
                Debug.Log(Constants.INITIALIZING_ARENA + GameSettings.instance.loadGamePath);
                this.InitializeGameByPath(loadPath, p1IsHuman, p2IsHuman, UIEnabled);
                break;
            //Loading Tutorial Arena (includes dummies for players)
            case GameSettings.GameMode.TUTORIAL:
                Debug.Log(Constants.LOADING_TUTORIAL + GameSettings.instance.loadGamePath);
                this.InitializeTutorialByPath(loadPath, p1IsHuman, p2IsHuman, UIEnabled);
                break;
            //Something has gone wrong
            default:
                break;
            
        }

    }

    // Update is called once per frame
    void Update()
    {
        this.gameLength = Time.time - this.startTime;

        if(evo) {
            this.pauseButton.SetActive(false);
            this.pauseMenuUI.SetActive(false);
        } else {
            this.pauseButton.SetActive(true);
        }
        if(Input.GetKeyDown(KeyCode.Escape)) {
            if(gameIsPaused) {
                Resume();
            } else {
                Pause();
                
            }
        }
        //End if this arena has lasted longer than 60 seconds
        if (EvolutionManager.instance != null)
        {
            if (gameLength >= EvolutionManager.instance.maxGameLength)
            {
                Debug.Log(Constants.GAME_TOO_LONG);
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
            Debug.Log(Constants.FILE_NOT_FOUND);
        }
        //Read from file
        else
        {
            Debug.Log(Constants.INITIALIZING_ARENA + path);
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

        //Save game to folder for next generation, pulling gameID if we're in evo
        if (EvolutionManager.instance != null)
        {
            this.SaveGameJSON(EvolutionManager.instance.currentGameID);
        }
        else 
        {
            this.SaveGameJSON(result.gameID);
        }

    }

    /// <summary>
    /// Initalizes a tutorial level with two additional training dummies for players to practice.
    /// 
    /// TODO: Refactor all Initialize methods.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="p1Playable"></param>
    /// <param name="p2Playable"></param>
    /// <param name="UIEnabled"></param>
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

        string tempDirectoryPath = Constants.EVO_POPULATION_PATH + gameID;
        if (!Directory.Exists(tempDirectoryPath))
        {
            Debug.Log(Constants.GENERATING_GAME);
            //Create new game if one does not exist
            Directory.CreateDirectory(tempDirectoryPath);
            this.GenerateGame(gameID);
            this.SaveGameJSON(gameID);  
        }
        //Read from file
        else
        {
            print("READING FROM FILE: " + tempDirectoryPath);
            this.ReadGame(tempDirectoryPath + Constants.PC_SLASH);
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
        this.SaveGameJSON(EvolutionManager.instance.currentGameID);

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
        print(tempDirectoryPath);
        // Read platforms from file
        this.platforms = this.ReadJson<Platforms>(tempDirectoryPath + Constants.LEVEL_JSON);
        // Read player from file
        this.serializedPlayer1 = this.ReadJson<SerializedPlayer>(tempDirectoryPath + Constants.PLAYER1_JSON);
        // Serialized Player 1, Move 1 Setup
        this.serializedMove1Player1 = this.ReadJson<SerializedMove>(tempDirectoryPath + Constants.PLAYER1MOVE1_JSON);
        // Serialized Player 2 Setup
        this.serializedPlayer2 = this.ReadJson<SerializedPlayer>(tempDirectoryPath + Constants.PLAYER2_JSON);
        // Serialized Player 2 Move 1 Setup
        this.serializedMove1Player2 = this.ReadJson<SerializedMove>(tempDirectoryPath + Constants.PLAYER2MOVE1_JSON);
        // Game Result Setup - pull generation and id number if there's a file, otherwise generate new
        this.result = new GameResult();
        if (File.Exists(tempDirectoryPath + Constants.GAME_RESULT_JSON))
        {
            GameResult oldResult = this.ReadJson<GameResult>(tempDirectoryPath + Constants.GAME_RESULT_JSON);
            this.result.gameID = oldResult.gameID;
            this.result.generationNum = oldResult.generationNum;
        }
        
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
        if (GameSettings.instance.mode == GameSettings.GameMode.EVO)
        {
            //Ensure that all IDs and rounds are up to date before sending. TODO: figure out why I needed this
            this.result.gameID = EvolutionManager.instance.currentGameID;
            this.result.round = EvolutionManager.instance.currRound;
            this.result.generationNum = EvolutionManager.instance.currGeneration;
            this.result.evaluate();

            //update evolution manager
            EvolutionManager.instance.AddResultFromRound(this.result);
            //Save to file
            this.SaveGameJSON(this.result.gameID);
        }
        else if (GameSettings.instance.mode == GameSettings.GameMode.LOADDISK) 
        {
            //Evaluate Results
            this.result.evaluateHumanGame();
            //Save to research results
            SaveGameJSONtoResultsFolder(GameSettings.instance.resultsPath);
            //return to main menu
            StartCoroutine(this.ReturnToLoadMenuCoroutine());
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
        this.p1HUDText = this.p1HUD.GetComponent<TextMeshProUGUI>();
        this.player1.playerDetails = this.p1HUDText;
        this.player1.heart1 = this.p1H1;
        this.player1.heart2 = this.p1H2;
        this.player1.heart3 = this.p1H3;
        //Player 2 Heads Up Display
        this.p2HUDText = this.p2HUD.GetComponent<TextMeshProUGUI>();
        this.player2.playerDetails = this.p2HUDText;
        this.player2.heart1 = this.p2H1;
        this.player2.heart2 = this.p2H2;
        this.player2.heart3 = this.p2H3;
        //notifications
        notificationsText = notifications.GetComponent<Text>();
    }

    public void DestroyUI() 
    {
        Destroy(p1HUD);
        Destroy(p2HUD);
        Destroy(notifications);
    }


    //TODO: Refactor this into a utility object
    /** checks to see if a file exists, if it doesn't, generates it
     */
    T ReadJson<T>(string filename)
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
        //Create a directory if non exist
        string tempDirectoryPath = Constants.EVO_POPULATION_PATH + gameID + Constants.PC_SLASH;
        //MAke a folder if it doesn't exist
        if (!File.Exists(tempDirectoryPath)) 
        {
            Directory.CreateDirectory(tempDirectoryPath);
        }
        //Save game description files to new folder
        string tempLevelPath = tempDirectoryPath + Constants.LEVEL_JSON;
        this.WriteJson<Platforms>(tempLevelPath, this.platforms);
        string tempPlayer1Path = tempDirectoryPath + Constants.PLAYER1_JSON;
        this.WriteJson<SerializedPlayer>(tempPlayer1Path, this.serializedPlayer1);
        string tempPlayer2Path = tempDirectoryPath + Constants.PLAYER2_JSON;
        this.WriteJson<SerializedPlayer>(tempPlayer2Path, this.serializedPlayer2);
        string tempPlayer1Move1Path = tempDirectoryPath + Constants.PLAYER1MOVE1_JSON;
        this.WriteJson<SerializedMove>(tempPlayer1Move1Path, this.serializedMove1Player1);
        string tempPlayer2Move1Path = tempDirectoryPath + Constants.PLAYER2MOVE1_JSON;
        this.WriteJson<SerializedMove>(tempPlayer2Move1Path, this.serializedMove1Player2);
        
        //Build string for results
        string resultsName = "";
        if (EvolutionManager.instance != null)
        {
            resultsName = "round" + EvolutionManager.instance.currRound;
        }
        //Check for folder, create it if it doesn't exist
        string tempGameResultPath = tempDirectoryPath + Constants.ROUND_RESULTS_FOLDER;
        if (!File.Exists(tempGameResultPath))
        {
            Directory.CreateDirectory(tempGameResultPath);
        }
        tempGameResultPath += Constants.PC_SLASH + resultsName + Constants.JSON;
        this.WriteJson<GameResult>(tempGameResultPath, this.result);
    }

    public void SaveGameJSONtoResultsFolder(string gamePath)
    {
        Debug.Log("Saving research results to path: " + gamePath);
        if (!File.Exists(gamePath))
        {
            Directory.CreateDirectory(gamePath);
        }
        string tempLevelPath = gamePath + Constants.LEVEL_JSON;
        this.WriteJson<Platforms>(tempLevelPath, this.platforms);
        string tempPlayer1Path = gamePath + Constants.PLAYER1_JSON;
        this.WriteJson<SerializedPlayer>(tempPlayer1Path, this.serializedPlayer1);
        string tempPlayer2Path = gamePath + Constants.PLAYER2_JSON;
        this.WriteJson<SerializedPlayer>(tempPlayer2Path, this.serializedPlayer2);
        string tempPlayer1Move1Path = gamePath + Constants.PLAYER1MOVE1_JSON;
        this.WriteJson<SerializedMove>(tempPlayer1Move1Path, this.serializedMove1Player1);
        string tempPlayer2Move1Path = gamePath + Constants.PLAYER2MOVE1_JSON;
        this.WriteJson<SerializedMove>(tempPlayer2Move1Path, this.serializedMove1Player2);
        string tempGameResultPath = gamePath + Constants.GAME_RESULT_JSON;
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

    public IEnumerator ReturnToLoadMenuCoroutine()
    {
        yield return new WaitForSeconds(5f);
        UpdateNotifications("Returning to Load Menu");
        yield return new WaitForSeconds(5f);
        ClearNotifications();
        SceneManager.LoadSceneAsync(Constants.LOAD_GAME_DISK_SCENE, LoadSceneMode.Single);
    }

    public void Pause(){
        pauseMenuUI.SetActive(true);
        timeScale = Time.timeScale;
        Time.timeScale = 0f;
        gameIsPaused = true;
    }

    public void Resume(){
        pauseMenuUI.SetActive(false);
        Time.timeScale = this.timeScale;
        gameIsPaused = false;
    }

    public void Menu(){
        Destroy(GameSettings.instance);
        Time.timeScale = this.timeScale;
        SceneManager.LoadScene("LoadGame");
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
    }

    public void SetPlayerToIJKL(Player player)
    {
        player.controller = new Controller(player2, null);
        player.controller.SetPlayer2Buttons();
    }

}
//Old/outdated constants that I am not brave enough to TODO yet
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
    public static string ROUND_RESULTS_FOLDER_PATH = "\\round_results";
}
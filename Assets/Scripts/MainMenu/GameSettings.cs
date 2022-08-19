using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using static Constants;
using static DataLogger;

/// <summary>
/// Catch-all central class for storing, managing, and reading game settings and 
/// session information at runtime.
/// 
/// Responsible for saving preferences on:
/// 
/// 1. Controller Preferences - are games played by players or agents
/// 2. File Overwrites - are you continuing a run or starting a new one
/// 3. Loading "Special" Games - Tutorials, Research Games, etc.
/// 4. Understanding Game Mode
/// 5. Cleaning memory
/// 
/// 
/// </summary>
public class GameSettings : MonoBehaviour
{
    //Lets other objects understand game state
    public enum GameMode { MENU, EVO, LOAD, PILOT, TUTORIAL, LOADDISK };

    public GameMode mode;

    //Singleton Object
    public static GameSettings instance = null;

    //Evolution Settings - null unless we are running evo
    public EvolutionSettings evolutionSettings = null;

    //Player Control Settings - assume that agent plays
    public bool p1IsHuman = false;
    public bool p2IsHuman = false;

    //Determines if UI is enabled - default to yes
    public bool UIEnabled = true;

    //Erase Settings - if true, erase all data and generated games before proceeding.
    public bool clearFilesBeforeRun = false;

    //Used for managing arena loads and results
    public string loadGamePath = "";
    public string resultsPath = "";

    //Used for Human Fitness Calculation, refactor 
    public float damageFitnessScalar = 10f;

    public bool loadWithTutorialController = false;

    void Awake()
    {
        ArenaManager.evo = false;

        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this) 
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        ArenaManager.evo = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetGameSettings() 
    {
        //Player Control Settings - assume that agent plays
        this.p1IsHuman = false;
        this.p2IsHuman = false;

        //Determines if UI is enabled - default to yes
        this.UIEnabled = true;

        //Erase Settings - if true, erase all data and generated games before proceeding.
        this.clearFilesBeforeRun = false;

        //Used for managing arena loads and results
        this.loadGamePath = "";
        this.resultsPath = "";

        //Used for Human Fitness Calculation, refactor 
        this.damageFitnessScalar = 10f;

        this.loadWithTutorialController = false;
    }

    /// <summary>
    /// Cleans up game objects depending on current game mode.
    /// </summary>
    public void CleanUp() { }


}

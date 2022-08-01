using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using static GameSettings;

public class StudyMenu : MonoBehaviour
{
    GameSettings gameSettings;
    // Start is called before the first frame update
    void Start()
    {
        //Update Game Mode
        this.gameSettings = GameSettings.instance;
        this.gameSettings.mode = GameSettings.GameMode.PILOT;

        this.gameSettings.p1IsHuman = true;
        this.gameSettings.p2IsHuman = true;

        //TODO: Initialize GameSettings if we haven't.
        //if (!GameSettings.instance) { }
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Returns you to the main menu.
    /// </summary>
    public void ReturnToMainMenu(){
        GameSettings.instance.ResetGameSettings();
        SceneManager.LoadScene(Constants.MAIN_MENU_SCENE);
    }

    /// <summary>
    /// Loads a game from file depending on which button was pressed.
    /// </summary>
    public void LoadGameFromButtonPress() 
    {
        //Button name is written in unity editor, hardcoded here. 
        string buttonName = EventSystem.current.currentSelectedGameObject.name;
        switch (buttonName) 
        {
            case ("Load A"):
                this.LoadResearchArenaFromFile(Constants.GAME_A);
                Debug.Log(Constants.LOADING_PILOT_GAME + Constants.GAME_A + Constants.JSON);
                break;
            case ("Load B"):
                this.LoadResearchArenaFromFile(Constants.GAME_B);
                Debug.Log(Constants.LOADING_PILOT_GAME + Constants.GAME_B + Constants.JSON);
                break;
            case ("Load C"):
                this.LoadResearchArenaFromFile(Constants.GAME_C);
                Debug.Log(Constants.LOADING_PILOT_GAME + Constants.GAME_C + Constants.JSON);
                break;
            case ("Load D"):
                this.LoadResearchArenaFromFile(Constants.GAME_D);
                Debug.Log(Constants.LOADING_PILOT_GAME + Constants.GAME_D + Constants.JSON);
                break;
            case ("Load E"):
                this.LoadResearchArenaFromFile(Constants.GAME_E);
                Debug.Log(Constants.LOADING_PILOT_GAME + Constants.GAME_E + Constants.JSON);
                break;
            case ("Load F"):
                this.LoadResearchArenaFromFile(Constants.GAME_F);
                Debug.Log(Constants.LOADING_PILOT_GAME + Constants.GAME_F + Constants.JSON);
                break;
            case ("Tutorial"):
                this.LoadTutorialArenaFromFile();
                Debug.Log(Constants.LOADING_TUTORIAL);
                break;
            default:
                Debug.Log(Constants.LOAD_STUDY_BUTTON_ERROR);
                break;
        }
    }

    public void LoadResearchArenaFromFile(string file)
    {
        GameSettings.instance.mode = GameSettings.GameMode.LOAD;
        GameSettings.instance.loadWithTutorialController = true;
        string loadPath = Constants.PILOT_GAME_LOAD_PATH + file + Constants.PC_SLASH;
        string resultsPath = Constants.PILOT_GAME_RESULTS_PATH + file + Constants.PC_SLASH;
        GameSettings.instance.loadGamePath = loadPath;
        GameSettings.instance.resultsPath = resultsPath;
        GameSettings.instance.loadWithTutorialController = false;
        SceneManager.LoadSceneAsync(Constants.ARENA_SCENE, LoadSceneMode.Single);
    }

    public void LoadTutorialArenaFromFile()
    {
        GameSettings.instance.mode = GameSettings.GameMode.TUTORIAL;
        GameSettings.instance.loadWithTutorialController = true;
        GameSettings.instance.loadGamePath = Constants.TUTORIAL_GAME_PATH + Constants.PC_SLASH;
        GameSettings.instance.resultsPath = Constants.TUTORIAL_RESULTS_PATH + Constants.PC_SLASH;
        SceneManager.LoadSceneAsync(Constants.ARENA_SCENE, LoadSceneMode.Single);
    }
}

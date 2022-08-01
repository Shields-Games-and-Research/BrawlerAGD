using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameSettings;

public class MainMenuManager : MonoBehaviour
{
    GameSettings gameSettings;

    //Juice to make the main menu animated
    GameObject titleText;
    float titleBounceHeight;
    float titleBounceLength;

    // Start is called before the first frame update
    void Start()
    {
        //Init Game Mode
        this.gameSettings = GameSettings.instance;
        this.gameSettings.mode = GameSettings.GameMode.MENU;
    }

    // Update is called once per frame
    void Update()
    {
        //If for any reason we are in another game mode, return to menu.
        if (this.gameSettings.mode != GameSettings.GameMode.MENU) 
        {
            this.gameSettings.mode = GameSettings.GameMode.MENU;
        }
    }

    public void LoadEvoManager(){
        SceneManager.LoadScene(Constants.EVOLUTION_MENU_SCENE);
    }

    public void LoadGame(){
        SceneManager.LoadScene(Constants.LOAD_GAME_DISK_SCENE);
    }

    public void LoadPilotStudy(){
        SceneManager.LoadSceneAsync(Constants.PILOT_STUDY_SCENE);
    }

    public void LoadCreditScene(){
        SceneManager.LoadScene(Constants.CREDITS_SCENE);
    }
}

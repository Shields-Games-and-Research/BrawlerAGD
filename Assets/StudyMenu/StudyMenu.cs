using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
public class StudyMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ReturnToMainMenu(){
        SceneManager.LoadScene("MainMenuScene");
    }
    // Load Game
    public void LoadGame() 
    {
        string buttonName = EventSystem.current.currentSelectedGameObject.name;
        switch (buttonName) 
        {
            case ("Load A"):
                Debug.Log("Loading A.");
                GameSettings.instance.LoadResearchArenaFromFile("GameA");
                break;
            case ("Load B"):
                GameSettings.instance.LoadResearchArenaFromFile("GameB");
                Debug.Log("Loading B.");
                break;
            case ("Load C"):
                GameSettings.instance.LoadResearchArenaFromFile("GameC");
                Debug.Log("Loading C.");
                break;
            case ("Load D"):
                GameSettings.instance.LoadResearchArenaFromFile("GameD");
                Debug.Log("Loading D.");
                break;
            case ("Load E"):
                GameSettings.instance.LoadResearchArenaFromFile("GameE");
                Debug.Log("Loading E.");
                break;
            case ("Load F"):
                GameSettings.instance.LoadResearchArenaFromFile("GameF");
                Debug.Log("Loading F.");
                break;
            case ("Tutorial"):
                GameSettings.instance.LoadTutorialArenaFromFile();
                Debug.Log("Loading Tutorial");
                break;
            default:
                Debug.Log("Bad button name passed");
                break;
        }
    }
}

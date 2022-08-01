using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;
using System.Linq;

public class LoadGameManager : MonoBehaviour
{
    public TMP_Text errorText;
    public GameObject pathText;


    // Start is called before the first frame update
    void Start()
    {
        errorText.gameObject.SetActive(false);

        if (GameSettings.instance != null)
        {
            GameSettings.instance.mode = GameSettings.GameMode.LOADDISK;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReturnToMainMenu() {
        SceneManager.LoadScene("MainMenuScene");
    }

    /// <summary>
    /// Evaluates 
    /// </summary>
    public void LoadGameFromTextInput() 
    { 
        //get string from field
        string userPath = pathText.GetComponent<TMP_InputField>().text;
        print(userPath);

        //attempt to load
        if (!Directory.Exists(userPath))
        {
            Debug.Log(Constants.FILE_NOT_FOUND);
            errorText.gameObject.SetActive(true);
        }
        else 
        {
            string loadPath = userPath + Constants.PC_SLASH;
            string resultsPath = userPath + Constants.PC_SLASH + "HumanResults" + Constants.PC_SLASH;
            GameSettings.instance.loadGamePath = loadPath;
            GameSettings.instance.resultsPath = resultsPath;
            GameSettings.instance.p1IsHuman = true;
            GameSettings.instance.p2IsHuman = true;
            GameSettings.instance.UIEnabled = true;
            GameSettings.instance.loadWithTutorialController = false;
            SceneManager.LoadSceneAsync(Constants.ARENA_SCENE, LoadSceneMode.Single);
        }


    }
}

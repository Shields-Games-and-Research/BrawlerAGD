using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/**Class used to load game settings and prepare ArenaManager
 * 
 */
public class GameSettings : MonoBehaviour
{
    public static GameSettings instance = null;

    public string loadGamePath = "";
    public string resultsPath = "";
    //Used for Human Fitness Calculation, refactor 
    public float damageFitnessScalar = 10f;

    public bool loadWithTutorialController = false;

    void Awake()
    {
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

    public void LoadResearchArenaFromFile(string file) 
    {
        string loadPath = Consts.RESEARCH_GAME + file;
        string resultsPath = Consts.RESEARCH_RESULTS + file;
        this.loadGamePath = loadPath;
        this.resultsPath = resultsPath;
        this.loadWithTutorialController = false;
        SceneManager.LoadSceneAsync("Arena", LoadSceneMode.Single);
    }

    public void LoadTutorialArenaFromFile() 
    {
        this.loadGamePath = Consts.TUTORIAL_GAME;
        this.resultsPath = Consts.TUTORIAL_RESULTS;
        this.loadWithTutorialController = true;
        SceneManager.LoadSceneAsync("Arena", LoadSceneMode.Single);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class EvolutionSettings : MonoBehaviour
{

    public static EvolutionSettings instance = null;
    public float timeScale;
    public int totalPopulation;
    public float targetGameLength;
    public int roundsToEvaluate;
    public float dropoutRate;
    public float mutationRate;
    public float maxGameLength;
    public int numGenerations;
    public float estimatedSimTime;
    public bool changeScene;

    //reference to game settings
    public GameSettings gameSettings;

    // Awake is called before Start
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
        //Set up connection between game and evo settings, update mode
        this.gameSettings = GameSettings.instance;
        this.gameSettings.mode = GameSettings.GameMode.EVO;
        this.gameSettings.evolutionSettings = this;

        timeScale = 1f;
        totalPopulation = 100;
        targetGameLength = 45f;
        roundsToEvaluate = 5;
        dropoutRate = 0.5f;
        mutationRate = 0.4f;
        maxGameLength = 60f;
        numGenerations = 100;
        changeScene = false;

        if(EvolutionManager.instance != null ) {
            Destroy(EvolutionManager.instance);
        }
    }

    // Update is called once per frame
    //TODO Move this to a discrete function and update from functions
    void Update()
    {
        if(!changeScene) {
            estimatedSimTime = CalculateEstimateSimTime(this.numGenerations, this.roundsToEvaluate, this.totalPopulation, this.targetGameLength, this.maxGameLength);
            if(estimatedSimTime == 0.0) {
                GameObject.Find("EstSimTimeText").GetComponent<TextMeshProUGUI>().text = "Estimated Simulation Length: " + "∞";
            } else {
                GameObject.Find("EstSimTimeText").GetComponent<TextMeshProUGUI>().text = "Estimated Simulation Length: " + estimatedSimTime + " ticks";
            }
        }
    }

    public void StartEvolutionScene() 
    {
        changeScene = true;
        SceneManager.LoadScene("EvolutionaryArenaManager");
    }

    public void ReturnToMainMenu(){
        changeScene = true;
        Destroy(this.gameObject);
        SceneManager.LoadScene("MainMenuScene");

    }

    public void AdjustTimeScale(float value) 
    {
        this.timeScale = value;
        GameObject.Find("TimeValue").GetComponent<TextMeshProUGUI>().text = value.ToString("0.00");
    }

    public void AdjustTotalPopulation(float value) 
    {
        this.totalPopulation = (int)value;
        GameObject.Find("PopValue").GetComponent<TextMeshProUGUI>().text = this.totalPopulation.ToString("0");

    }

    public void AdjustTargetGameLength(float value) 
    {
        this.targetGameLength = value;
        GameObject.Find("TarLenValue").GetComponent<TextMeshProUGUI>().text = value.ToString("0");
    }

    public void AdjustRoundsToEvaluate(float value) 
    {
        this.roundsToEvaluate = (int)value;
        GameObject.Find("RoundEvalValue").GetComponent<TextMeshProUGUI>().text = this.roundsToEvaluate.ToString("0");
    }

    public void AdjustDropoutRate(float value) 
    {
        this.dropoutRate = value;
        GameObject.Find("DropValue").GetComponent<TextMeshProUGUI>().text = value.ToString("0.00");
    }

    public void AdjustMutationRate(float value) 
    {
        this.mutationRate = value;
        GameObject.Find("MutValue").GetComponent<TextMeshProUGUI>().text = value.ToString("0.00");
    }

    public void AdjustMaxGameLength(float value) 
    {
        this.maxGameLength = value;
        GameObject.Find("MaxLenValue").GetComponent<TextMeshProUGUI>().text = value.ToString("0");
    }

    public void AdjustNumGenerations(float value) 
    {
        this.numGenerations = (int)value;
        if(value == 0f) {
            GameObject.Find("GenValue").GetComponent<TextMeshProUGUI>().text = "∞";
            return;

        } 
        GameObject.Find("GenValue").GetComponent<TextMeshProUGUI>().text = this.numGenerations.ToString("0");
    }
    public void UserToggle(bool tog) {
        Slider slide = GameObject.Find("GenSlider").GetComponent<Slider>();
        if(tog) {
            AdjustNumGenerations(0f);
            slide.enabled = false;
        } else {
            AdjustNumGenerations(slide.value);
            slide.enabled = true;
        }
    }


    public float CalculateEstimateSimTime( int generations, int roundsToEvaluate, int totalPopulation, float targetGameLength, float maxGameLength) {
        //just a placeholder.
        if(generations < 10) {
            return generations *roundsToEvaluate * totalPopulation * (targetGameLength - ((maxGameLength - targetGameLength) * 1 / 2));
        } else {
            return generations *roundsToEvaluate * totalPopulation * (targetGameLength + ((maxGameLength - targetGameLength) * 1 / 8));
        }
    }
 
}

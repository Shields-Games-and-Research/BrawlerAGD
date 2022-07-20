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
        timeScale = 1f;
        totalPopulation = 100;
        targetGameLength = 45f;
        roundsToEvaluate = 5;
        dropoutRate = 0.5f;
        mutationRate = 0.4f;
        maxGameLength = 60f;
        numGenerations = 100;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartEvolutionScene() 
    {
        SceneManager.LoadScene("EvolutionaryArenaManager");
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
    public void userToggle(bool tog) {
        Slider slide = GameObject.Find("GenSlider").GetComponent<Slider>();
        if(tog) {
            AdjustNumGenerations(0f);
            slide.enabled = false;
        } else {
            AdjustNumGenerations(slide.value);
            slide.enabled = true;
        }
    }

}

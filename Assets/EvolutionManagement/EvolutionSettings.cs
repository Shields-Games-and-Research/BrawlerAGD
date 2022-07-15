using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolutionSettings : MonoBehaviour
{

    public static EvolutionSettings instance = null;
    public float timeScale = 0;
    public float totalPopulation = 0;
    public float targetGameLength = 0;
    public float roundsToEvaluate = 0;
    public float dropoutRate = 0;
    public float mutationRate = 0;
    public float maxGameLength = 0;
    public float numGenerations = 0;

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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AdjustTimeScale(float value) 
    {
        this.timeScale = value;
        Debug.Log(value);
    }
    public void AdjustTotalPopulation(float value) 
    {
        this.totalPopulation = value;
        Debug.Log(value);
    }
    public void AdjustTargetGameLength(float value) 
    {
        this.targetGameLength = value;
        Debug.Log(value);
    }
    public void AdjustRoundsToEvaluate(float value) 
    {
        this.roundsToEvaluate = value;
        Debug.Log(value);
    }
    public void AdjustDropoutRate(float value) 
    {
        this.dropoutRate = value;
        Debug.Log(value);
    }
    public void AdjustMutationRate(float value) 
    {
        this.mutationRate = value;
        Debug.Log(value);
    }
    public void AdjustMaxGameLength(float value) 
    {
        this.maxGameLength = value;
        Debug.Log(value);
    }
    public void AdjustNumGenerations(float value) 
    {
        this.numGenerations = value;
        Debug.Log(value);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EvolutionManager : MonoBehaviour
{
    public static EvolutionManager instance = null;

    //Game Length
    //Player has: number of hits, total damage, number of recovery
    public List<GameResult> results = new List<GameResult>();

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

        //StartCoroutine(testCoroutine());
        SceneManager.LoadSceneAsync("Arena", LoadSceneMode.Additive);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddResultFromGame(GameResult result) 
    {
        this.results.Add(result);
    }

    public void Evolve(int numGenerations, int popSize) 
    {
        int currGeneration = 0;
        //loop through population numGenerations times
        while (currGeneration < numGenerations) 
        { 
            //Read Json, initialize game by ID
        }
    }

    IEnumerator testCoroutine() 
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync("Arena", LoadSceneMode.Additive);
        while (!ao.isDone) 
        {
            yield return null;
        }
        SceneManager.UnloadSceneAsync("Arena");
    }



}



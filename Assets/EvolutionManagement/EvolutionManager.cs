using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using Random = System.Random;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Class That Manages the Evolutionary Process
/// Singleton once generated
/// </summary>
public class EvolutionManager : MonoBehaviour
{
    //Create instance for singleton reference
    public static EvolutionManager instance = null;

    //Random object for mutations and generation
    public Random rand = new Random();

    //Time factor for games (<1 slows, >1 speeds)
    public float timeScale = 1f;
    //Determines if simulation is paused
    public bool gameIsPaused = false;
    public bool pauseMenuActive;
    public GameObject pauseMenuUI;
    public float timeInRound;
    public float roundStartTime;
    // Population Size For Each Generation
    private int popSize = 100;
    private bool[] gamesFinished = new bool[100];
    
    // Generation Details. If numGenerations is 0, run indefinitely
    public int numGenerations = 0;
    public int currGeneration = 0;

    // Number of Evaluation Rounds
    private int numEvalRounds = 1;
    public bool[] roundsFinished = new bool[1];
    public int currRound = 0;
    
    // How many individuals are removed from the population each generation.
    public float dropoutRate = 0.5f;
    
    // The rate at which attributes are mutated on an individual's genome 
    public double mutationRate = 0.4;
    
    // The index of the current Game's results
    public int currentGameID = 0;
    
    // Maximimum length of game
    public float maxGameLength = 60f;
    public float targetGameLength = 45f;
    
    //Fitness Scalars - Make scoring occur on the same order of magnitude
    public float damageFitnessScalar = 10f;
    
    // Average fitness of all individuals
    public List<float> averageFitness = new List<float>();
    // Average fitness of non-dropped individuals
    public List<float> averageTopFitness = new List<float>();

    //List of game results for processing; id is index, list is all results for that game during rounds
    public Dictionary<int, List<GameResult>> results = new Dictionary<int, List<GameResult>>();
    //Raw scores of each gameID entry
    public Dictionary<int, float> evals = new Dictionary<int, float>();

    //puts all results into a serialized object for printing to file.
    public EvolutionResults evolutionResults;

    //TODO: SETTING TO SET GAME TIME

    void Awake()
    {
        //Instantiate class as singleton.
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        //Game Settings Check - if there is a settings object in the scene, load key instance settings from that.
        GameObject evolutionSettingsObj = GameObject.Find("EvolutionSettings");
        if (evolutionSettingsObj != null) 
        {
            EvolutionSettings evoSettings = evolutionSettingsObj.GetComponent<EvolutionSettings>();
            this.SetTimeScale(evoSettings.timeScale);
            this.popSize = evoSettings.totalPopulation;
            this.gamesFinished = new bool[this.popSize];
            this.dropoutRate = evoSettings.dropoutRate;
            this.mutationRate = evoSettings.mutationRate;
            this.maxGameLength = evoSettings.maxGameLength;
            this.targetGameLength = evoSettings.targetGameLength;
            this.numGenerations = evoSettings.numGenerations;
            this.numEvalRounds = evoSettings.roundsToEvaluate;
            this.roundsFinished = new bool[this.numEvalRounds];
        }
        
    }

    void Start()
    {
        //Set timescale based on optimization needs
        var fixedDeltaTime = Time.fixedDeltaTime;
        this.SetTimeScale(this.timeScale);

        //Begin Evolution
        this.evolutionResults = new EvolutionResults();
        StartCoroutine(Evolve());
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            if(gameIsPaused) {
                Resume();
            } else {
                Pause();
            }
        }
        if(pauseMenuActive){
            GameObject.Find("TimeElapsed").GetComponent<TextMeshProUGUI>().text = "Simulation Time Elapsed: " + Time.time.ToString("0.00") + " ticks";
            EvolutionSettings evs = new EvolutionSettings();
            float estimatedTime = CalculateEstimateSimTime(numGenerations, numEvalRounds, popSize, targetGameLength, maxGameLength);
            TextMeshProUGUI estimateText = GameObject.Find("EstSimTime").GetComponent<TextMeshProUGUI>();
            if(estimatedTime != 0.0f) {
                estimateText.text = "Estimated Simulation Time: " + estimatedTime + " ticks";
            } else {
                estimateText.text = "Estimated Simulation Time: " + "∞";

            }
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

    // Settings

    /// <summary>
    /// Sets the time scale of the running simulation and adjusted Time.fixedDeltaTime.
    /// </summary>
    /// <param name="timeScalar">timeScalar</param>
    public void SetTimeScale(float timeScalar)
    {
        this.timeScale = timeScalar;
        Time.timeScale = timeScalar;
        Time.fixedDeltaTime = Time.fixedDeltaTime * Time.timeScale;
    }

    // Evolution
    /// <summary>
    /// Generates or reads an existing population of individuals from file, then performs a genetic algorithm on them for numGenerations or until stopped. 
    /// Scoring is recorded up to the current generation.
    /// 
    /// </summary>
    /// <returns></returns>
    public IEnumerator Evolve()
    {
        //Create a list of games 
        int[] gameIDs = new int[this.popSize];
        List<int> gidList = new List<int>();
        // Setup
        for (int i = 0; i < gameIDs.Length; i++)
        {
            gameIDs[i] = i;
            gidList.Add(i);
        }
        //loop through population numGenerations times
        while (this.currGeneration < numGenerations || numGenerations == 0)
        {
            this.currGeneration++;
            // For each gameID, run a game, and collect the result
            foreach (int id in gameIDs)
            {
                this.results[id] = new List<GameResult>();
                //RUN N TIMES, WHERE N IS ROUNDS
                this.currentGameID = id;
                gamesFinished[this.currentGameID] = false;

                //Track the current rounds done before proceeding
                for(int i = 0; i < this.numEvalRounds; i++) 
                {
                    this.roundsFinished[i] = false;
                }
                //play a given number of rounds, recording their results to results
                this.currRound = 0;
                while (this.currRound < this.numEvalRounds) 
                {
                    Debug.Log("Running Game " + id + " Round: " + this.currRound);
                    // Load the Arena scene
                    SceneManager.LoadSceneAsync("Arena", LoadSceneMode.Additive);
                    // Check if the current round is nearly finished
                    // TODO: should likely check that the ID stays good
                    while (!roundsFinished[this.currRound] )
                    {
                        yield return null;
                    }
                    //Unload Scene
                    SceneManager.UnloadSceneAsync("Arena");
                    this.currRound++;
                }
                //All rounds have been run for this game

                //Evaluate rounds
                this.EvaluateRoundsForIndividual(this.currentGameID, 0);

            }
            //All rounds have been run for all games and all have been evaluated

            //Save the current generation to our evolution results for data analysis
            EvolutionResult generationResult = new EvolutionResult();
            generationResult.generationNumber = this.currGeneration;

            
            // Sort the population by the fitness and keep the top x%
            gidList.Sort(compareGameIDs);
            int indexToCut = (int)(popSize * dropoutRate);
            int validParents = popSize - indexToCut;
            // Generate new individuals to fill out the population
            // Write the generated games to the appropriate folders
            for (int i = 0; i < indexToCut; i++)
            {
                int gid = gidList[i];
                Debug.Log("DISCARDING GAME SAVED IN FOLDER game" + gid + " WITH FITNESS: " + evals[gid]);
                int parentid1 = rand.Next(validParents) + indexToCut;
                int parentid2 = rand.Next(validParents) + indexToCut;
                crossoverGames(parentid1, parentid2, gid);
            }
            // Average fitness
            float totalFitness = 0f;
            float totalTopFitness = 0f;
            float topFitness = float.MinValue;
            for (int i = 0; i < popSize; i++)
            {
                float currFitness = evals[gidList[i]];
                if (currFitness >= topFitness)
                {
                    topFitness = currFitness;
                }
                totalFitness += currFitness;
                if (i >= indexToCut)
                {
                    totalTopFitness += currFitness;
                }
            }

            float averageFitness = (totalFitness / (float)popSize);
            float averageTopFitness = (totalTopFitness / (float)(popSize - indexToCut));

            //TODO: Rename these lists or the above variables
            this.averageFitness.Add(averageFitness);
            this.averageTopFitness.Add(averageTopFitness);

            generationResult.topFitness = topFitness;
            generationResult.averageFitness = averageFitness;
            generationResult.averageTopFitness = averageTopFitness;

            //add to our temporary results object, then save to file for analsysis
            this.evolutionResults.evolutionResults.Add(generationResult);
            this.SaveToResults();
            this.Pause();
        }
    }

    public void AddResultFromRound(GameResult result)
    {
        //add results to dictionary of rounds to results
        List<GameResult> roundResults = this.results[result.gameID];
        if (roundResults.Count == 0) 
        {
            this.results[result.gameID] = new List<GameResult>();
            for (int i = 0; i < this.numEvalRounds; i++) 
            {
                this.results[result.gameID].Add(new GameResult());
            }
        }
        this.results[result.gameID][result.round] = result;
        //indicate that this round is done to continue while loop in evolve
        this.roundsFinished[result.round] = true;

    }

    /// <summary>
    /// Evaluates all rounds for a GameID and returns a fitness based on a strategy
    /// 
    /// Future ideas for evaluation: random, mode within a range
    /// </summary>
    /// <param name="gameID">id in the population to be evaluated </param>
    /// <param name="evalStrategy">0 = median, 1 = average, default grabs 0th fitness</param>
    /// <returns></returns>
    public float EvaluateRoundsForIndividual(int gameID, int evalStrategy)
    {
        //get all three results from results object
        List<GameResult> tempResults = this.results[gameID].OrderBy(gameResult => gameResult.fitness).ToList();

        float averageFitness = 0;
        foreach(GameResult result in tempResults) 
        {
            averageFitness += result.fitness;
        }
        averageFitness = averageFitness / tempResults.Count;
        float medianFitness = tempResults[tempResults.Count/2].fitness;
        if (medianFitness <= -7f && medianFitness >= -13f) 
        {
            this.SaveToResults();
            
        }
        switch (evalStrategy) 
        {
            case 0:
                this.evals[gameID] = medianFitness;
                return medianFitness;
            case 1:
                this.evals[gameID] = averageFitness;
                return averageFitness;
            default:
                this.evals[gameID] = tempResults[0].fitness;
                return tempResults[0].fitness;
        }
        
    }

    public void crossoverGames(int gid1, int gid2, int newGameid)
    {
        string g1Path = Consts.GAME_PATH + gid1;
        string g2Path = Consts.GAME_PATH + gid2;
        string gnewPath = Consts.GAME_PATH + newGameid;
        // Parent1 stuff
        SerializedPlayer parent1player1 = ReadJson<SerializedPlayer>(g1Path + Consts.PLAYER1_PATH);
        SerializedMove parent1player1move1 = ReadJson<SerializedMove>(g1Path + Consts.PLAYER1MOVE1_PATH);
        SerializedPlayer parent1player2 = ReadJson<SerializedPlayer>(g1Path + Consts.PLAYER2_PATH);
        SerializedMove parent1player2move1 = ReadJson<SerializedMove>(g1Path + Consts.PLAYER2MOVE1_PATH);
        Platforms parent1platforms = ReadJson<Platforms>(g1Path + Consts.LEVEL_PATH);

        // Parent2 stuff
        SerializedPlayer parent2player1 = ReadJson<SerializedPlayer>(g2Path + Consts.PLAYER1_PATH);
        SerializedMove parent2player1move1 = ReadJson<SerializedMove>(g2Path + Consts.PLAYER1MOVE1_PATH);
        SerializedPlayer parent2player2 = ReadJson<SerializedPlayer>(g2Path + Consts.PLAYER2_PATH);
        SerializedMove parent2player2move1 = ReadJson<SerializedMove>(g2Path + Consts.PLAYER2MOVE1_PATH);
        Platforms parent2platforms = ReadJson<Platforms>(g2Path + Consts.LEVEL_PATH);

        // New stuff
        SerializedPlayer newplayer1 = SerializedPlayer.singlePointCrossover(parent1player1, parent2player1, rand);
        SerializedMove newplayer1move1 = SerializedMove.singlePointCrossover(parent1player1move1, parent2player1move1, rand);
        SerializedPlayer newplayer2 = SerializedPlayer.singlePointCrossover(parent1player2, parent2player2, rand);
        SerializedMove newplayer2move1 = SerializedMove.singlePointCrossover(parent1player2move1, parent2player2move1, rand);
        Platforms newPlatforms = Platforms.singlePointCrossover(parent1platforms, parent2platforms, rand);
        if (rand.NextDouble() < mutationRate)
        {
            newplayer1.mutate(rand);
            newplayer1move1.mutate(rand);
            newplayer2.mutate(rand);
            newplayer2move1.mutate(rand);
            newPlatforms.mutate(rand);
        }

        // Write out
        WriteJson<SerializedPlayer>(gnewPath + Consts.PLAYER1_PATH, newplayer1);
        WriteJson<SerializedMove>(gnewPath + Consts.PLAYER1MOVE1_PATH, newplayer1move1);
        WriteJson<SerializedPlayer>(gnewPath + Consts.PLAYER2_PATH, newplayer2);
        WriteJson<SerializedMove>(gnewPath + Consts.PLAYER2MOVE1_PATH, newplayer2move1);
        WriteJson<Platforms>(gnewPath + Consts.LEVEL_PATH, newPlatforms);
    }

    public int compareGameIDs(int gid1, int gid2)
    {
        float val1 = evals[gid1];
        float val2 = evals[gid2];
        if (val1 < val2)
        {
            return -1;
        }
        else if (val1 > val2)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    public void SaveToResults()
    {
        if (!File.Exists(Consts.EVO_RESULTS_PATH))
        {
            Directory.CreateDirectory(Consts.EVO_RESULTS_PATH);
        }
        this.WriteJson<EvolutionResults>(Consts.EVO_RESULTS_PATH + Consts.RESULTS_FILE_PATH, this.evolutionResults);
    }

    public void Pause(){
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;
        pauseMenuActive = true;
    }

    public void Resume(){
        pauseMenuUI.SetActive(false);
        Time.timeScale = this.timeScale;
        gameIsPaused = false;
        pauseMenuActive = false;

    }
    public void ResumeInMenu(){
        //pauseMenuUI.SetActive(false);

        Time.timeScale = this.timeScale;
        gameIsPaused = false;
        pauseMenuActive = true;

    }
    /*public void Menu(){
        SceneManager.LoadScene("EvolutionaryManagerStartScene");
    }*/
    // TODO : duplicate of the code in ArenaManager
    T ReadJson<T>(string filename)
    {
        print("filename reading: " + filename);
        // Write to file
        if (!File.Exists(filename))
        {
            throw new FileNotFoundException("Attempting to read JSON failed");
        }
        // If the file exists, read from it
        else
        {
            string rawSerializedObj = File.ReadAllText(filename);
            T serializedObj = JsonUtility.FromJson<T>(rawSerializedObj);
            //print(serializedObj);
            return serializedObj;
        }
    }

    public void WriteJson<T>(string filename, T serializedObj)
    {
        string serializedJSON = JsonUtility.ToJson(serializedObj);
        File.WriteAllText(filename, serializedJSON);
    }
}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Random = System.Random;
using UnityEngine.SceneManagement;

public class EvolutionManager : MonoBehaviour
{
    // Make sure gamesFinished is the right length
    private int popSize = 10;
    private int numGenerations = 0;
    private bool[] gamesFinished = new bool[10];
    private float dropoutRate = 0.5f;
    private double mutationRate = 0.4;
    // Number of games that can be running at a time
    public int currGeneration = 0;


    public int currentGameID = 0;
    // Maximimum length of game
    public float maxGameLength = 60f;
    public float targetGameLength = 45f;
    //Fitness Scalars
    public float damageFitnessScalar = 10f;
    

    public static EvolutionManager instance = null;
    public Random rand = new Random();
    // Average fitness of all individuals
    public List<float> averageFitness = new List<float>();
    // Average fitness of non-dropped individuals
    public List<float> averageTopFitness = new List<float>();

    //Game Length
    //Player has: number of hits, total damage, number of recovery
    public Dictionary<int, GameResult> results = new Dictionary<int, GameResult>();
    public Dictionary<int, float> evals = new Dictionary<int, float>();

    //puts all results into a serialized object for printing to file.
    public EvolutionResults evolutionResults;

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
        this.evolutionResults = new EvolutionResults();
        StartCoroutine(Evolve());
        //Set timescale based on optimization needs
        this.SetTimeScale(2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddResultFromGame(GameResult result) 
    {
        this.results[result.gameID] = result;
        this.evals[result.gameID] = result.evaluate();
        this.gamesFinished[result.gameID] = true;
    }

    public void SetTimeScale(float timeScalar) 
    {
        Time.timeScale = timeScalar;
        Time.fixedDeltaTime = Time.fixedDeltaTime * Time.timeScale;
    }

    public IEnumerator Evolve() 
    {
        int[] gameIDs = new int[this.popSize];
        List<int> gidList = new List<int>();
        // Setup
        for (int i = 0; i < gameIDs.Length; i ++)
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
                Debug.Log("Running Game " + id);
                gamesFinished[id] = false;
                this.currentGameID = id;
                // Load the Arena scene
                SceneManager.LoadSceneAsync("Arena", LoadSceneMode.Additive);   
                // Wait for the game to finish
                while (!gamesFinished[id])
                {
                    yield return null;
                }
                SceneManager.UnloadSceneAsync("Arena");
                Debug.Log("Done running! Fitness was " + evals[id]);

            }
            // Save the current generation to our evolution results for data analysis
            EvolutionResult generationResult = new EvolutionResult();
            generationResult.generationNumber = this.currGeneration;

            // Save results for generation
            foreach (GameResult tempGameResult in this.results.Values) 
            {
                tempGameResult.generationNum = this.currGeneration;
                generationResult.gameResults.Add(tempGameResult);
            }
            // Sort the population by the fitness and keep the top x%
            gidList.Sort(compareGameIDs);
            int indexToCut = (int) (popSize * dropoutRate);
            int validParents = popSize - indexToCut;
            // Generate new individuals to fill out the population
            // Write the generated games to the appropriate folders
            for (int i = 0; i < indexToCut; i ++)
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
            for (int i = 0; i < popSize; i ++)
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
            float averageTopFitness = (totalTopFitness / (float) (popSize - indexToCut));
            
            //TODO: Rename these lists or the above variables
            this.averageFitness.Add(averageFitness);
            this.averageTopFitness.Add(averageTopFitness);

            generationResult.topFitness = topFitness;
            generationResult.averageFitness = averageFitness;
            generationResult.averageTopFitness = averageTopFitness;

            //add to our temporary results object, then save to file for analsysis
            this.evolutionResults.evolutionResults.Add(generationResult);
            this.SaveToResults();
            
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



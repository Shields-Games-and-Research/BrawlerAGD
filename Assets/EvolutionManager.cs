using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Random = System.Random;
using UnityEngine.SceneManagement;

public class EvolutionManager : MonoBehaviour
{
    // Make sure gamesFinished is the right length
    public int popSize = 2;
    public int numGenerations = 5;
    public bool[] gamesFinished = new bool[2];
    public float dropoutRate = 0.5f;
    public double mutationRate = 0.1;
    // Number of games that can be running at a time
    public float nInstances = 1;
    public int currentGameID = 0;
    public static EvolutionManager instance = null;
    Random rand = new Random();
    // Average fitness of all individuals
    public List<float> averageFitness = new List<float>();
    // Average fitness of non-dropped individuals
    public List<float> averageTopFitness = new List<float>();

    //Game Length
    //Player has: number of hits, total damage, number of recovery
    public Dictionary<int, GameResult> results = new Dictionary<int, GameResult>();
    public Dictionary<int, float> evals = new Dictionary<int, float>();

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
        StartCoroutine(Evolve());
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

    public IEnumerator Evolve() 
    {
        Debug.Log("Starting Evolution");
        int currGeneration = 0;
        int[] gameIDs = new int[popSize];
        List<int> gidList = new List<int>();
        // Setup
        for (int i = 0; i < gameIDs.Length; i ++)
        {
            gameIDs[i] = i;
            gidList.Add(i);
        }
        //loop through population numGenerations times
        while (currGeneration < numGenerations) 
        {
            Debug.Log("Generation: " + currGeneration);
            Debug.Log("Running " + popSize + " games");
            currGeneration++;
            // For each gameID, run a game, and collect the result
            foreach (int id in gameIDs)
            {
                Debug.Log("Running Game " + id);
                gamesFinished[id] = false;
                currentGameID = id;
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
            Debug.Log("Done running games");
            // Sort the population by the fitness and keep the top x%
            gidList.Sort(compareGameIDs);
            int indexToCut = (int) (popSize * dropoutRate);
            int validParents = popSize - indexToCut;
            // Generate new individuals to fill out the population
            // Write the generated games to the appropriate folders
            for (int i = 0; i < indexToCut; i ++)
            {
                int gid = gidList[i];
                int parentid1 = rand.Next(validParents) + indexToCut;
                int parentid2 = rand.Next(validParents) + indexToCut;
                crossoverGames(parentid1, parentid2, i);
            }
            // Average fitness
            float totalFitness = 0f;
            float totalTopFitness = 0f;
            for (int i = 0; i < popSize; i ++)
            {
                float f = evals[gidList[i]];
                totalFitness += f;
                if (i >= indexToCut)
                {
                    totalTopFitness += f;
                }
            }
            averageFitness.Add(totalFitness / (float) popSize);
            averageTopFitness.Add(totalTopFitness / (float) (popSize - indexToCut));
            Debug.Log("Average Fitness");
            foreach (float entry in averageFitness)
            {
                Debug.Log(entry);
            }
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

    IEnumerator testCoroutine() 
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync("Arena", LoadSceneMode.Additive);
        while (!ao.isDone) 
        {
            yield return null;
        }
        SceneManager.UnloadSceneAsync("Arena");
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



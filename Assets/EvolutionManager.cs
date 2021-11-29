using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolutionManager : MonoBehaviour
{
    public static EvolutionManager instance = null;

    //Game Length
    //Player has: number of hits, total damage, number of recovery
    List<GameResult> results = new List<GameResult>();

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

    public void AddResultFromGame(GameResult result) 
    {
        this.results.Add(result);
    }

}

public class GameResult
{
    //format for file-names?
    string gameID = "Foo";
    int generationNum = 0;

    float totalDamageP1;
    float totalRecoveryStateTransitionP1;
    float totalHitsReceivedP1;

    float totalDamageP2;
    float totalRecoveryStateTransitionP2;
    float totalHitsReceivedP2;

    public GameResult(
        float totalDamageP1, 
        float totalRecoveryStateTransitionP1, 
        float totalHitsReceivedP1, 
        float totalDamageP2, 
        float totalRecoveryStateTransitionP2, 
        float totalHitsReceivedP2) 
    {
        this.totalDamageP1 = totalDamageP1;
        this.totalRecoveryStateTransitionP1 = totalRecoveryStateTransitionP1;
        this.totalHitsReceivedP1 = totalHitsReceivedP1;

        this.totalDamageP2 = totalDamageP2;
        this.totalRecoveryStateTransitionP2 = totalRecoveryStateTransitionP2;
        this.totalHitsReceivedP2 = totalHitsReceivedP2;
    }
}

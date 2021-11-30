using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResult
{
    //format for file-names?
    public string gameID = "Foo";
    public int generationNum = 0;

    public float totalDamageP1;
    public float totalRecoveryStateTransitionP1;
    public float totalHitsReceivedP1;

    public float totalDamageP2;
    public float totalRecoveryStateTransitionP2;
    public float totalHitsReceivedP2;

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

    public GameResult()
    {
        this.totalDamageP1 = 0;
        this.totalRecoveryStateTransitionP1 = 0;
        this.totalHitsReceivedP1 = 0;

        this.totalDamageP2 = 0;
        this.totalRecoveryStateTransitionP2 = 0;
        this.totalHitsReceivedP2 = 0;
    }
}


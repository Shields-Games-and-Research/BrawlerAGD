using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameResult
{
    //file naming is game + numbered ID
    public int gameID;
    public int generationNum;

    public float totalDamageP1;
    public float totalRecoveryStateTransitionP1;
    public float totalHitsReceivedP1;

    public float totalDamageP2;
    public float totalRecoveryStateTransitionP2;
    public float totalHitsReceivedP2;

    public float totalGameLength;

    public GameResult(
        int _gameID,
        float totalDamageP1,
        float totalRecoveryStateTransitionP1,
        float totalHitsReceivedP1,
        float totalDamageP2,
        float totalRecoveryStateTransitionP2,
        float totalHitsReceivedP2,
        float totalGameLength
        )
    {
        this.gameID = _gameID;
        
        this.totalDamageP1 = totalDamageP1;
        this.totalRecoveryStateTransitionP1 = totalRecoveryStateTransitionP1;
        this.totalHitsReceivedP1 = totalHitsReceivedP1;

        this.totalDamageP2 = totalDamageP2;
        this.totalRecoveryStateTransitionP2 = totalRecoveryStateTransitionP2;
        this.totalHitsReceivedP2 = totalHitsReceivedP2;

        this.totalGameLength = totalGameLength;
    }

    public GameResult()
    {
        this.totalDamageP1 = 0;
        this.totalRecoveryStateTransitionP1 = 0;
        this.totalHitsReceivedP1 = 0;

        this.totalDamageP2 = 0;
        this.totalRecoveryStateTransitionP2 = 0;
        this.totalHitsReceivedP2 = 0;

        this.totalGameLength = 0;
    }

    public float evaluate()
    {
        // TODO
        return 0;
    }
}


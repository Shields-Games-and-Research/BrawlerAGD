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
    public float remainingStocksP1;

    public float totalDamageP2;
    public float totalRecoveryStateTransitionP2;
    public float totalHitsReceivedP2;
    public float remainingStocksP2;

    public float totalGameLength;

    public float fitness;

    public GameResult(
        int _gameID,
        float totalDamageP1,
        float totalRecoveryStateTransitionP1,
        float totalHitsReceivedP1,
        float remainingStocksP1,
        float totalDamageP2,
        float totalRecoveryStateTransitionP2,
        float totalHitsReceivedP2,
        float remainingStocksP2,
        float totalGameLength
        )
    {
        this.gameID = _gameID;
        
        this.totalDamageP1 = totalDamageP1;
        this.totalRecoveryStateTransitionP1 = totalRecoveryStateTransitionP1;
        this.totalHitsReceivedP1 = totalHitsReceivedP1;
        this.remainingStocksP1 = remainingStocksP1;

        this.totalDamageP2 = totalDamageP2;
        this.totalRecoveryStateTransitionP2 = totalRecoveryStateTransitionP2;
        this.totalHitsReceivedP2 = totalHitsReceivedP2;
        this.remainingStocksP2 = remainingStocksP2;

        this.totalGameLength = totalGameLength;
    }

    public GameResult()
    {
        this.totalDamageP1 = 0;
        this.totalRecoveryStateTransitionP1 = 0;
        this.totalHitsReceivedP1 = 0;
        this.remainingStocksP1 = 0;

        this.totalDamageP2 = 0;
        this.totalRecoveryStateTransitionP2 = 0;
        this.totalHitsReceivedP2 = 0;
        this.remainingStocksP2 = 0;

        this.totalGameLength = 0;
        this.fitness = float.NegativeInfinity;
    }

    public float evaluate()
    {
        //Game went over time
        float overTimePenalty = (this.totalGameLength >= EvolutionManager.instance.maxGameLength)?-35f:0f;
        float timeFitness = -Math.Abs(EvolutionManager.instance.targetGameLength - this.totalGameLength) + overTimePenalty;

        //Damage Dealt (higher better)
        float damageFitness = (this.totalDamageP1 + this.totalDamageP2) / EvolutionManager.instance.damageFitnessScalar;

        //Total Collisions (higher better)
        float collisionFitness = (this.totalHitsReceivedP1 + this.totalHitsReceivedP2);
        
        //Penalize games that are dramatically mismatched in damage
        float damageFairnessFitness = -Math.Abs(this.totalDamageP1 - this.totalDamageP2) / EvolutionManager.instance.damageFitnessScalar;

        //Reward games that are lopsided in stocks. No penalty if there is a 1 stock difference. Scale to match other variables
        float stockFairnessFitness = (3f - Math.Abs(this.remainingStocksP1 - this.remainingStocksP1));

        //Save fitness to folder
        this.fitness = timeFitness + damageFitness + collisionFitness + damageFairnessFitness + stockFairnessFitness;
        return this.fitness;
    }
}


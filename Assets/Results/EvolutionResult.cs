using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EvolutionResult
{
    public int generationNumber;
    public float topFitness;
    public float averageFitness;
    public float averageTopFitness;

    public EvolutionResult() 
    {
        this.generationNumber = 0;
        this.topFitness = 0;
        this.averageFitness = 0;
        this.averageTopFitness = 0;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EvolutionResult
{
    public List<GameResult> evolutionResults;

    public EvolutionResult(List<GameResult> evolutionResults) 
    {
        this.evolutionResults = evolutionResults;
    }

    public EvolutionResult() 
    {
        this.evolutionResults = new List<GameResult>();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EvolutionResults 
{
    public List<EvolutionResult> evolutionResults;

    public EvolutionResults(List<EvolutionResult> evolutionResults) 
    {
        this.evolutionResults = evolutionResults;
    }

    public EvolutionResults() 
    {
        this.evolutionResults = new List<EvolutionResult>();
    }
}

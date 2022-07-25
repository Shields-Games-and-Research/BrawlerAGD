using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class EvolutionMenu : MonoBehaviour
{
    //Saved evolution setting values
    //Saves timescale to evolution settings
    public void SaveTimeScale(TextMeshProUGUI txt) 
    {
        EvolutionSettings.instance.AdjustTimeScale(float.Parse(txt.text));
    }

    //Saves total population to evolution settings
    public void SaveTotalPopulation(TextMeshProUGUI txt) 
    {
        EvolutionSettings.instance.AdjustTotalPopulation(float.Parse(txt.text));

    }

    //Saves target game length to evolution settings
    public void SaveTargetGameLength(TextMeshProUGUI txt) 
    {
        EvolutionSettings.instance.AdjustTargetGameLength(float.Parse(txt.text));

    }

    //Saves rounds to evaluate to evolution settings
    public void SaveRoundsToEvaluate(TextMeshProUGUI txt) 
    {
        EvolutionSettings.instance.AdjustRoundsToEvaluate(float.Parse(txt.text));

    }

    //Saves dropout rate to evolution settings
    public void SaveDropoutRate(TextMeshProUGUI txt) 
    {
        EvolutionSettings.instance.AdjustDropoutRate(float.Parse(txt.text));

    }

    //Saves mutation rate to evolution settings
    public void SaveMutationRate(TextMeshProUGUI txt) 
    {
        EvolutionSettings.instance.AdjustMutationRate(float.Parse(txt.text));

    }

    //Saves max game length to evolution settings
    public void SaveMaxGameLength(TextMeshProUGUI txt) 
    {
        EvolutionSettings.instance.AdjustMaxGameLength(float.Parse(txt.text));

    }

    //Saves number of generations to evolution settings
    public void SaveNumGenerations(TextMeshProUGUI txt) 
    {
        if(txt.text.Equals("∞")) {
            EvolutionSettings.instance.AdjustNumGenerations(0);
            return;
        }
        EvolutionSettings.instance.AdjustNumGenerations(float.Parse(txt.text));

    }
}

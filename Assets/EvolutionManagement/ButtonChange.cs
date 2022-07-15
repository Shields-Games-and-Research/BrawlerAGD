using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonChange : MonoBehaviour
{

    // Sets Text of Rounds to Evaluate

    public void SetText(string text) {
        TextMeshProUGUI txt = GameObject.Find("RoundEvalValue").GetComponent<TextMeshProUGUI>();
        txt.text = text;
        
            
        
    }



}

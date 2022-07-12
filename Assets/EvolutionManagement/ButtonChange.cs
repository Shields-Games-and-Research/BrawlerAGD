using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonChange : MonoBehaviour
{
    public TextMeshProUGUI buttonText;
    public TextMeshProUGUI changeText;
    // Start is called before the first frame update

    void Start()
    {
        changeText.text = buttonText.text;
    }



}

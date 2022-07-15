using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ToggleSlider : MonoBehaviour
{
    public Toggle toggleS;
    public Slider slider;
    public TextMeshProUGUI sliderText;

    // Start is called before the first frame update
    
    void Start()
    {
        
    }

    // Toggles Slider Enabled
    void Update()
    {
        if(toggleS.isOn){
            slider.enabled = false;
            sliderText.text = "∞";
            
        } else {
            slider.enabled = true;
        }
    }
}

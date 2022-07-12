using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolutionSettings : MonoBehaviour
{

    public static EvolutionSettings instance = null;
    public float timeScale = 0;

    // Awake is called before Start
    void Awake() 
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this) 
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AdjustTimeScale(float value) 
    {
        this.timeScale = value;
        Debug.Log(value);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ArenaCamera : MonoBehaviour
{

    void Awake()
    {
        if (SceneManager.GetActiveScene().name == "EvolutionaryArenaManager")
        {
            Destroy(GameObject.FindWithTag("ArenaCamera"));
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

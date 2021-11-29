using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ArenaFactory : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SceneManager.LoadSceneAsync("Arena", LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync("Arena", LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync("Arena", LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync("Arena", LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync("Arena", LoadSceneMode.Additive);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

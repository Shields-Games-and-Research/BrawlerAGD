using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData instance = null;

    //Game Length
    //Player has: number of hits, total damage, number of recovery

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

    //add num hits

    //addnumrecovery

    //add damage

    // Update is called once per frame
    void Update()
    {
        
    }

    //
    public void resetGameData() 
    {
        
    }

    public void EndGame() 
    {
        Time.timeScale = 0f;
        Time.fixedDeltaTime = Time.fixedDeltaTime * Time.timeScale;

        //TODO: output game data
    }
}

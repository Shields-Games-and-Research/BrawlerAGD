using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGenerator : MonoBehaviour
{

    public Player player;

    // Start is called before the first frame update
    void Start()
    {
        // Generate / Load Map

        // from map, spawnLocation1, spawnLocation2
        //var spawnLocation1 = new Vector3(0, 0, 0);
        //var spawnLocation2 = new Vector3(0, 0, 0);

        //Player player1 = Instantiate(player, spawnLocation1); // TODO: location
        Player player1 = Instantiate(player); // TODO: location
        //Player player2 = Instantiate(player, spawnLocation2); // location

        // Set keybindings for each player

        // Generate / Load constants for each player

        // Generate / Load constants for the player's moves 

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class MapGenerator
{
    //
}

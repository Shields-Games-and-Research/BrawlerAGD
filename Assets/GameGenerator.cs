using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGenerator : MonoBehaviour
{
    /** Player Constants:
     *  SPAWN_LOCATION_P1 = ; // where the first spawn of the first player is
     *  LEFT_KEY_P1 = left; //left key movement
     *  RIGHT_KEY_P1 = right; //right key movement
     *  JUMP_KEY_P1 = jump; //jump key
     *  FALL_KEY_P1 = fall; //fall key (TODO)
     *  MOVE_1_KEY_P1 = move1Key; //attack key
     *  
     *  SPAWN_LOCATION_P2 = ; // where the first spawn of the second player is
     *  LEFT_KEY_P2 = ; //left key movement
     *  RIGHT_KEY_P2 = ; //right key movement
     *  JUMP_KEY_P2 = ; //jump key
     *  FALL_KEY_P2 = ; //fall key (TODO)
     *  MOVE_1_KEY_P2 = ; //attack key
     */

    public Player player;

    // Start is called before the first frame update
    void Start()
    {
        // Generate / Load Map

        //TODO: Fetch from document - replace with constants set above during setup
        
    

        //Player 1 Instantiation
        Vector3 spawnLocationP1 = new Vector3(1, 0, 0);
        Player player1 = Instantiate(player, spawnLocationP1, Quaternion.identity);
        //Player 1 Controls
        player1.leftKey = KeyCode.A;
        player1.rightKey= KeyCode.D;
        player1.jumpKey = KeyCode.W;
        player1.fallKey = KeyCode.S;
        player1.move1Key = KeyCode.Space;

        //Player 2 Instantiation
        Vector3 spawnLocationP2 = new Vector3(-1, 0, 0);
        Player player2 = Instantiate(player, spawnLocationP2, Quaternion.identity);
        //Player 2 Controls
        player2.leftKey = KeyCode.J;
        player2.rightKey = KeyCode.L;
        player2.jumpKey = KeyCode.I;
        player2.fallKey = KeyCode.K;
        player2.move1Key = KeyCode.Return;



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

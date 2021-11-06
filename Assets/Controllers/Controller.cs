using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller
{
    public KeyCode leftKey;
    public KeyCode rightKey;
    public KeyCode jumpKey;
    public KeyCode pauseKey;
    public KeyCode move1Key;

    public Player player;

    public ControllerBehavior controllerBehavior;

    public Controller(Player player)
    {
        //Assign linked player
        this.player = player;

        //Defaults
        this.leftKey = KeyCode.A;
        this.rightKey = KeyCode.D;
        this.jumpKey = KeyCode.W;
        this.move1Key = KeyCode.S;
    }
}

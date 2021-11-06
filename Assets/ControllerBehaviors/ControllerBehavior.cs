using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ControllerBehavior
{
    public Controller controller;
    public Player player;

    public ControllerBehavior(Controller controller)
    {
        //Assign linked player and controller
        this.controller = controller;
        this.player = controller.player;
    }

    public void Update()
    {
        player.moveLeft();
    }

}
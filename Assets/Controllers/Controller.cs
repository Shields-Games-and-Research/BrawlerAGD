using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Controller
{
    public KeyCode leftKey;
    public KeyCode rightKey;
    public KeyCode jumpKey;
    public KeyCode pauseKey;
    public KeyCode move1Key;

    public Player player;

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

    public virtual void Update()
    {

    }

    public virtual bool GetKey(KeyCode code)
    {
        return Input.GetKey(code);
    }

    public virtual bool GetKeyDown(KeyCode code)
    {
        return Input.GetKeyDown(code);
    }
}


public class HoldLeft : Controller
{

    public HoldLeft(Player player) : base (player)
    {

    }

    public override bool GetKey(KeyCode code)
    {
        if (code == this.leftKey)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override bool GetKeyDown(KeyCode code)
    {
        if (code == this.leftKey)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

public class AI : Controller
{
    public AI(Player player) : base(player)
    {

    }

    public override void Update()
    {
        
    }

    public override bool GetKey(KeyCode code)
    {
        if (code == this.leftKey)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override bool GetKeyDown(KeyCode code)
    {
        if (code == this.leftKey)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
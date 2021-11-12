using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    Transform playerTransform;

    public AI(Player player) : base(player)
    {
        /**
         * Get references to:
         * Players, Moves, Platform locations
         * 
         * Above Pit?
         * Below Nearest Platform?
         */
        playerTransform = player.gameObject.transform;

    }

    public override void Update()
    {
        //TODO: Finish writing "Over Platform" check
        RaycastHit hit;
        LayerMask mask = LayerMask.GetMask("Floor");
        RaycastHit2D platformHit = Physics2D.Raycast(playerTransform.position, -Vector2.up, Mathf.Infinity, mask, -Mathf.Infinity, Mathf.Infinity);
       
        if (platformHit.collider != null)
        {

            Debug.Log("Over Platform" + platformHit.collider.gameObject.name);
        }
        else
        {
            Debug.Log("Over Pit ");
        }

        List<Collider2D> platforms = new List<Collider2D>();
        ContactFilter2D platformFilter = new ContactFilter2D();
        platformFilter.SetLayerMask(mask);
        player.bc.OverlapCollider(platformFilter, platforms);
        Debug.Log("Num Platforms: " + platforms.Count);
    }

    public override bool GetKey(KeyCode code)
    {
        return false;
    }

    public override bool GetKeyDown(KeyCode code)
    {
        return false;
    }


}
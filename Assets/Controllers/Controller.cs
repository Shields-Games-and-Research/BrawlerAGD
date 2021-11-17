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

    //ColliderDistance2D closestPoint;

    bool overPit;
    LayerMask mask;
    //bool aboveNearestPlatform;
    //bool inOpponentAttackRange;
    //bool opponentInAttackRange;

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
        this.mask = LayerMask.GetMask("Floor");
        //Get collider distance object for closest platform point
        
        List<Collider2D> platforms = new List<Collider2D>();
        ContactFilter2D platformFilter = new ContactFilter2D();
        platformFilter.SetLayerMask(mask);
        player.bc.OverlapCollider(platformFilter, platforms);
        if (platforms.Count > 0) 
        {
            ColliderDistance2D closestDistance = player.bc.Distance(platforms[0]); 
        }
        /** ColliderDistance2D properties
            distance    Gets the distance between two colliders.
            isOverlapped Gets whether the distance represents an overlap or not.
            isValid Gets whether the distance is valid or not.
            normal A normalized vector that points from pointB to pointA.
            pointA A point on a Collider2D that is a specific distance away from pointB.
            pointB A point on a Collider2D that is a specific distance away from pointA.
        */
        //this.closestPoint = player.bc.Distance(closestPlatformTest);

    }

    public override void Update()
    {
      
        RaycastHit2D platformHit = Physics2D.Raycast(playerTransform.position, -Vector2.up, Mathf.Infinity, this.mask, -Mathf.Infinity, Mathf.Infinity);

        this.overPit = (platformHit.collider == null);
        Debug.Log(this.overPit);

        //Debug.Log("closest point: " + this.closestPoint.distance);
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
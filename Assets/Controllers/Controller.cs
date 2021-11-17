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
    public Player opponent;

    public Transform playerTransform;
    public LayerMask mask;

    //TODO: Brainstorm implementation that scales beyond 2 players
    public Controller(Player player, Player opponent)
    {
        //Assign linked player
        this.player = player;

        //Assign opponent player
        this.opponent = opponent;

        //Defaults
        this.leftKey = KeyCode.A;
        this.rightKey = KeyCode.D;
        this.jumpKey = KeyCode.W;
        this.move1Key = KeyCode.S;

        this.playerTransform = player.gameObject.transform;
        this.mask = LayerMask.GetMask("Floor");
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

    public bool OverPit() 
    {
        RaycastHit2D platformHit = Physics2D.Raycast(this.playerTransform.position, -Vector2.up, Mathf.Infinity, this.mask, -Mathf.Infinity, Mathf.Infinity);
        return (platformHit.collider == null);
    }

    //TODO: allow for checking of different moves the player has
    public bool PlayerInRangeOfMove()
    {
        return opponent.sr.bounds.Intersects(player.move1.sr.bounds);
    }

    public bool OpponentAbove() 
    {
        return opponent.gameObject.transform.position.y > player.gameObject.transform.position.y;
    }

    public bool OpponentRight() 
    {
        return opponent.gameObject.transform.position.x > player.gameObject.transform.position.x;
    }

    public bool ApproachingEdge()
    { 
        
    }

    
}


public class HoldLeft : Controller
{

    public HoldLeft(Player player, Player opponent) : base (player, opponent)
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

    ColliderDistance2D closestDistance;

    //AI States
    public enum AIState
    {
        pursue,
        recover
    }

    public AIState state = AIState.pursue;

    public AI(Player player, Player opponent) : base(player, opponent)
    {
        
        //Get collider distance object for closest platform point
        
        List<Collider2D> platforms = new List<Collider2D>();
        ContactFilter2D platformFilter = new ContactFilter2D();
        platformFilter.SetLayerMask(mask);
        player.bc.OverlapCollider(platformFilter, platforms);
        if (platforms.Count > 0) 
        {
            this.closestDistance = player.bc.Distance(platforms[0]); 
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
        //Debug.Log("Over Pit: " + this.OverPit());
        //Debug.Log("In Range: " + this.PlayerInRangeOfMove());
        //Debug.Log("Opponent Above: " + this.OpponentAbove());
        //Debug.Log("Opponent Right: " + this.OpponentRight());
        /**TODO:
         * - Where the current player's move will land at tick x
         * - Player Above
         * - Player to Right
         * - Platform Edge Detection
         * - Are you in range of opponents 
         * - Overpit
         */

        switch (state) 
        {
            case AIState.pursue:
                break;
            case AIState.recover:
                break;
            default:
                break;
        }

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
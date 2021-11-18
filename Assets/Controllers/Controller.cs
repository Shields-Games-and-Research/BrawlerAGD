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

        

        /** ColliderDistance2D properties
            distance    Gets the distance between two colliders.
            isOverlapped Gets whether the distance represents an overlap or not.
            isValid Gets whether the distance is valid or not.
            normal A normalized vector that points from pointB to pointA.
            pointA A point on a Collider2D that is a specific distance away from pointB.
            pointB A point on a Collider2D that is a specific distance away from pointA.
        */
        
    }

    public virtual void Update()
    {
        Debug.Log(this.GetClosestPlatformDirection());
    }

    public Vector2 GetClosestPlatformDirection() 
    {
        //Initialize empty list for checking for platforms
        List<Collider2D> platforms = new List<Collider2D>();
        //Create a filter to only check for "floor"
        ContactFilter2D platformFilter = new ContactFilter2D();
        platformFilter.SetLayerMask(this.mask);
        //Check for all platforms within a reasonable range of player
        this.player.bc.OverlapCollider(platformFilter, platforms);
        //Return value that points to the nearest platform.
        Vector2 nearestPlatformPoint = new Vector2(0, 0);
        //Has at least one platform been detected
        if (platforms.Count > 0)
        {
            float minPlatformDist = float.PositiveInfinity;
            //Check for the closest platform, find the closest point on that platform
            foreach (Collider2D platform in platforms)
            {
                Vector2 tempVector = platform.ClosestPoint(playerTransform.position);
                float tempDist = Vector2.Distance(playerTransform.position, tempVector);

                if (tempDist < minPlatformDist)
                {
                    minPlatformDist = tempDist;
                    nearestPlatformPoint = tempVector;
                }
            }
        }
        //Returns the direction vector from the closest point on a platform to the player
        return nearestPlatformPoint - (Vector2)playerTransform.position;
    }

    public virtual bool GetKey(KeyCode code)
    {
        return Input.GetKey(code);
    }

    public virtual bool GetKeyDown(KeyCode code)
    {
        return Input.GetKeyDown(code);
    }

    public bool OverPit(Vector2 offset) 
    {
        Vector3 vector3Offset = new Vector3(offset.x * this.playerTransform.localScale.x, offset.y, 0);
        RaycastHit2D platformHit = Physics2D.Raycast(this.playerTransform.position + vector3Offset, -Vector2.up, Mathf.Infinity, this.mask, -Mathf.Infinity, Mathf.Infinity);
        return (platformHit.collider == null);
    }

    public bool OverPit()
    {
        return OverPit(new Vector2(0,0));
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
        //Assumes that grid base unit is 1 and that positive transform is direction player is facing
        return OverPit(new Vector2(1, 0)) && !OverPit(); 
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



    //AI States
    public enum AIState
    {
        pursue,
        recover
    }

    public AIState state = AIState.pursue;

    public AI(Player player, Player opponent) : base(player, opponent)
    {


    }

    public override void Update()
    {
        //Debug.Log("Over Pit: " + this.OverPit());
        //Debug.Log("In Range: " + this.PlayerInRangeOfMove());
        //Debug.Log("Opponent Above: " + this.OpponentAbove());
        //Debug.Log("Opponent Right: " + this.OpponentRight());
        //Debug.Log("Approaching Edge: " + this.ApproachingEdge());
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
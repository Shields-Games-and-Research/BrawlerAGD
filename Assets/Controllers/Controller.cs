using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Controller
{
    public string horizontalAxis;
    public string jumpKey;
    public string pauseKey;
    public string move1Key;

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
        this.horizontalAxis = "";
        this.jumpKey = "";
        this.move1Key = "";

        this.playerTransform = player.gameObject.transform;
        this.mask = LayerMask.GetMask("Floor");

        if (this.player.playerName == "player 1")
        {
            this.SetPlayer1Buttons();
        }
        else
        {
            this.SetPlayer2Buttons();
        }



        /** ColliderDistance2D properties
            distance    Gets the distance between two colliders.
            isOverlapped Gets whether the distance represents an overlap or not.
            isValid Gets whether the distance is valid or not.
            normal A normalized vector that points from pointB to pointA.
            pointA A point on a Collider2D that is a specific distance away from pointB.
            pointB A point on a Collider2D that is a specific distance away from pointA.
        */

    }

    public void SetPlayer1Buttons() 
    {
        this.horizontalAxis = "HorizontalP1";
        this.jumpKey = "JumpP1";
        this.move1Key = "AttackP1";
    }

    public void SetPlayer2Buttons() 
    {
        this.horizontalAxis = "HorizontalP2";
        this.jumpKey = "JumpP2";
        this.move1Key = "AttackP2";
    }

    public virtual void Update()
    {
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

    public virtual float GetAxis(string code)
    {
        return Input.GetAxis(code);
    }

    public virtual bool GetKeyDown(string code)
    {
        return Input.GetButtonDown(code);
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
        return OverPit(new Vector2(0.2f, 0)) && !OverPit(); 
    }

    public bool JumpsExhausted()
    {
        return this.player.jumpsExhausted;
    }

    
}


public class HoldJump : Controller
{

    public HoldJump(Player player, Player opponent) : base (player, opponent)
    {

    }

    public override float GetAxis(string code)
    {
        return 0f;
    }

    public override bool GetKeyDown(string code)
    {
        if (code == this.jumpKey) 
        {
            return false;
        }
        return false;
    }

}

public class HoldLeft : Controller
{

    public HoldLeft(Player player, Player opponent) : base(player, opponent)
    {

    }

    public override float GetAxis(string code)
    {
        if (code == this.horizontalAxis)
        {
            return 1f;
        }
        else
        {
            return -1f;
        }
    }

    public override bool GetKeyDown(string code)
    {
        return false;
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
    public bool pressLeft;
    public bool pressRight;
    public bool pressJump;
    public bool pressMove1;
    public int recoveryTime;
    public int recoveryTimeLimit;
    public Vector2 targetMod;
    public int targetTimeLimit;
    public int targetTime;

    public AI(Player player, Player opponent) : base(player, opponent)
    {
        recoveryTimeLimit = 300;
        recoveryTime = 0;
        targetTimeLimit = 150;
        targetMod = new Vector2(0, 0);
    }

    public override void Update()
    {
        //Debug.Log("Over Pit: " + this.OverPit());
        //Debug.Log("In Range: " + this.PlayerInRangeOfMove());
        //Debug.Log("Opponent Above: " + this.OpponentAbove());
        //Debug.Log("Opponent Right: " + this.OpponentRight());
        //Debug.Log("Approaching Edge: " + this.ApproachingEdge());
        // Update target timer and change target modification
        targetTime++;
        if (targetTime > targetTimeLimit)
        {
            targetTime = 0;
            targetMod.x = Random.Range(-1f, 1f);
            targetMod.y = Random.Range(-0.2f, 0.2f);
        }
        // Update recovery timer
        if (OverPit())
        {
            recoveryTime++;
        }
        else
        {
            recoveryTime = 0;
        }

        // Change state if the player is over a pit long enough
        if (recoveryTime > recoveryTimeLimit)
        {
            this.player.totalRecoveryStateTransition++;
            state = AIState.recover;
        }
        else
        {
            state = AIState.pursue;
        }

        switch (state)
        {
            case AIState.pursue:
                UpdatePursue();
                break;
            case AIState.recover:
                UpdateRecover();
                break;
            default:
                break;
        }

    }

    public void UpdatePursue()
    {
        Vector2 movePosition = player.move1.gameObject.transform.position;
        Vector2 relMovePosition = Vector2.Scale((movePosition - (Vector2) player.gameObject.transform.position), new Vector2(1.2f, 1.2f));
        Vector2 targetPosition = (Vector2) opponent.gameObject.transform.position - relMovePosition;
        targetPosition = targetPosition + targetMod;

        if (this.PlayerInRangeOfMove())
        {
            pressMove1 = true;
        }
        else
        {
            pressMove1 = false;

            //jump towards opponent if they are above you, or if you are approaching an edge
            if (((targetPosition.y > 0) && player.isGrounded) || ApproachingEdge())
            {
                pressJump = true;
            }
            else
            {
                pressJump = false;
            }

            //if the player is below and you are separated by platform, move until you are off platform
            if ((targetPosition.y < 0) && player.isGrounded)
            {
                pressRight = true;
                pressLeft = false;
            }
            //otherwise simply move towards opponent
            else if (targetPosition.x > player.gameObject.transform.position.x)
            {
                pressRight = true;
                pressLeft = false;
            }
            else
            {
                pressLeft = true;
                pressRight = false;
            }
        }
    }

    public void UpdateRecover()
    {
        Vector2 platformDirection = GetClosestPlatformDirection();
        if (OverPit() && platformDirection.y <= 0.1)
        {
            pressJump = true;
        }
        else
        {
            pressJump = false;
        }
        if (platformDirection.x > 0)
        {
            pressRight = true;
            pressLeft = false;
        }
        else
        {
            pressRight = false;
            pressLeft = true;
        }
        pressMove1 = false;
    }

    public override float GetAxis(string code)
    {
        if (code == this.horizontalAxis) 
        {
            if (this.pressLeft) 
            {
                return -1;
            }
            if (this.pressRight)
            {
                return 1;
            }
        }
        return 0f;
    }

    public override bool GetKeyDown(string code)
    {
        if (code == this.jumpKey) 
        {
            return this.pressJump;
        }
        if (code == this.move1Key)
        {
            return this.pressMove1;
        }
        return false;
    }

    


}
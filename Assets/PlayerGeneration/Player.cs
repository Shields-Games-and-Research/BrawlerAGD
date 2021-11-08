using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Player : MonoBehaviour
{
    /**COMPONENT DECLARATION*/
    public Rigidbody2D rb;
    public SpriteRenderer sr;

    /**PREFAB DECLARATION */
    public Move move;

    /**PLAYER MECHANICS: These track the characteristics of a specific character instance at any given point. Changing these parameters will alter the movement abilities of players. */

    //Determines how quickly a player reaches max speed on the ground.
    public float groundAcceleration = 2;
    //Determines how quickly a player reaches max speed in the air.
    public float airAcceleration = 1;

    //Determines maximum speed a player can move by themselves.
    public float maxGroundSpeed = 4;
    public float maxAirSpeed = 2;

    //force applied to each jump 
    public float groundJumpForce = 3;

    //force applied to each air jump
    public float airJumpForce = 3;

    //respawn location
    public Vector2 respawnLoc = new Vector2(0, 0);

    //stocks a player starts with - must be an integer, must be positive
    public float stocks = 3f;

    //the scalar for the hitstun applied to a player based on their damage
    public float hitstunDamageScalar = 0.1f;

    /**PLAYER MOVESET: these instance variables will be used to manage the generated moves of a player. */
    public Move move1;

    /**PRIVATE PARAMETERS: Parameters used for internal logic or defined rules in our design space. */
    private bool isGrounded;
    private bool jumpsExhausted;
    private float damage = 0;
    private bool isInvincible = false;
    //Shows initial UI
    public Text playerDetails;
    //Player Name
    public string playerName = "Lorem";

    public Controller controller;

    /**STATE MANAGEMENT: TODO: Refactor to separate classes eventually */
    public enum PlayerState
    {
        idle,
        air,
        airJumpsExhausted,
        warmUp,
        attack,
        coolDown,
        landing,
        stun
    }
    //default player state to idle on spawn.
    public PlayerState state = PlayerState.idle;

    /** PLAYER CONTROLS: sets the keycodes used to control the player. */
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode jumpKey = KeyCode.W;
    public KeyCode fallKey = KeyCode.S;
    public KeyCode move1Key = KeyCode.Space;

    void Awake()
    {
        Sprite[] playerSprites = Resources.LoadAll<Sprite>("players");
        int playerSprite = Random.Range(0, playerSprites.Length);

        //set relevant game objects as instance variables for performant access
        rb = GetComponent<Rigidbody2D>();
        
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = playerSprites[playerSprite];

        rb.freezeRotation = true;

        //instantiate moves on player creation
        move1 = Instantiate<Move>(move, transform.position + move.center, Quaternion.identity, transform);
        
    }

    // Start is called before the first frame update
    void Start()
    {

    }


    // Decide on State and then apply corresponding policies to said state
    void Update()
    {
        updatePlayerHUD();

        //check if controller has a behavior or not
        if (this.controller.controllerBehavior is null)
        {
            switch (state)
            {
                case PlayerState.idle:
                    updateIdle();
                    break;
                case PlayerState.air:
                    updateAir();
                    break;
                case PlayerState.airJumpsExhausted:
                    updateAirJumpsExhausted();
                    break;
                case PlayerState.warmUp:
                    updateWarmUp();
                    break;
                case PlayerState.attack:
                    updateAttack();
                    break;
                case PlayerState.coolDown:
                    updateCoolDown();
                    break;
                case PlayerState.landing:
                    updateLanding();
                    break;
                case PlayerState.stun:
                    updateStun();
                    break;
                default:
                    state = PlayerState.idle;
                    break;
            }
        }
        else 
        {
            this.controller.controllerBehavior.Update();
        }

    }

    void updatePlayerHUD() 
    {
        playerDetails.text =
            playerName + "\n" +
            "Damage: " + damage + "%\n" +
            "Stocks: " + stocks + "\n" +
            "State: " + state;
    }

    /**A player, from idle, can:
     * move
     * jump
     * attack
     * be attacked
     */
    void updateIdle()
    {
        sr.color = Color.white;

        if (Input.GetKey(controller.rightKey)) { moveRight(); }
        else if (Input.GetKey(controller.leftKey)) { moveLeft(); }

        if (Input.GetKeyDown(controller.jumpKey)) { jump(); }

        if (Input.GetKeyDown(controller.move1Key)) { performMove(move1); }
    }

    /**A player, from air, can:
     * jump
     * move
     * attack
     * be attacked
     */
    void updateAir()
    {
        sr.color = Color.green;

        if (Input.GetKey(controller.rightKey)) { moveRight(); }
        else if (Input.GetKey(controller.leftKey)) { moveLeft(); }

        if (Input.GetKeyDown(controller.jumpKey)) { jump(); }

        if (Input.GetKeyDown(controller.move1Key)) { performMove(move1); }

    }

    /**A player, from air jumps exhausted, can:
     * move
     */
    void updateAirJumpsExhausted()
    {
        sr.color = Color.grey;
        if (Input.GetKey(controller.rightKey)) { moveRight(); }
        if (Input.GetKey(controller.leftKey)) { moveLeft(); }
    }

    /**A player, from warm up, can:
     * move
     */
    void updateWarmUp()
    {
        sr.color = Color.yellow;
        if (Input.GetKey(controller.rightKey)) { moveRight(); }
        if (Input.GetKey(controller.leftKey)) { moveLeft(); }
    }

    /**A player, from attack, can:
     * 
     */
    void updateAttack()
    {
        sr.color = Color.red;
        move1.SetActive();
    }

    /**A player, from cool down, can:
     *
     */
    void updateCoolDown()
    {
        sr.color = Color.blue;
        move1.SetInactive();
    }

    /**A player, from stun, can:
     *  
     */
    void updateStun()
    {
        sr.color = Color.magenta;
    }

    void updateLanding() { }
    



    //When a collision begins, this method is called
    void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = true;
            jumpsExhausted = false;
            if (state == PlayerState.airJumpsExhausted || state == PlayerState.air) 
            {
                state = PlayerState.idle;
            }
        }

    }

    void OnTriggerExit2D(Collider2D collision) 
    {
        //Player has left the arena
        if (collision.gameObject.CompareTag("Arena"))
        {
            print("Exit Arena Triggered");
            this.respawn();
        }
    }

    void OnTriggerStay2D(Collider2D collision) 
    {
        //Player has been hit by a move and is not currently invincible
        if (collision.gameObject.CompareTag("Attack") && !this.isInvincible) 
        {
            Move tempMove = collision.gameObject.GetComponent<Move>();
            this.damage += tempMove.damageGiven;
            Vector2 collKnockbackDir = (transform.position - collision.gameObject.transform.position);
            this.applyKnockback(collKnockbackDir, tempMove.knockbackScalar, tempMove.knockbackDirection, tempMove.hitstunDuration);
            StartCoroutine(InvincibilityCoroutine(0.1f));
        }
    }

    //When a collision ends, this method is called
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = false;
        }
    }

    /**PLAYER ACTIONS
 * 
 * 
 * 
 * 
 */

    public void jump()
    {
        if (isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, groundJumpForce);
            state = PlayerState.air;
        }
        else if (!jumpsExhausted)
        {
            rb.velocity = new Vector2(rb.velocity.x, airJumpForce);
            jumpsExhausted = true;
            state = PlayerState.airJumpsExhausted;
        }
    }

    public void moveRight()
    {
        //is the character on the ground
        if (isGrounded)
        {
            //will adding an accelleration exceed max self-applied speed
            if (Mathf.Abs(rb.velocity.x) + groundAcceleration >= maxGroundSpeed)
            {
                //go max speed
                rb.velocity = new Vector2(maxGroundSpeed, rb.velocity.y);
            }
            else 
            {
                //add accelleration vector if not
                rb.velocity += new Vector2(groundAcceleration, 0);
            }
        }
        else 
        {
            //will adding an accelleration exceed max self-applied air speed
            if (Mathf.Abs(rb.velocity.x) + airAcceleration >= maxAirSpeed)
            {
                //go max speed
                rb.velocity = new Vector2(maxAirSpeed, rb.velocity.y);
            }
            else 
            {
                //add accelleration air vector if not
                rb.velocity += new Vector2(airAcceleration, 0);
            }
        }
        //change direction if appropriate
        if (transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    public void moveLeft()
    {
        //is the character on the ground
        if (isGrounded)
        {
            //will adding an accelleration exceed max self-applied speed
            if (Mathf.Abs(rb.velocity.x) + groundAcceleration >= maxGroundSpeed)
            {
                //go max speed
                rb.velocity = new Vector2(-maxGroundSpeed, rb.velocity.y);
            }
            else 
            {
                //add accelleration vector if not
                rb.velocity -= new Vector2(groundAcceleration, 0);
            }
        }
        else
        {
            //will adding an accelleration exceed max self-applied speed
            if (Mathf.Abs(rb.velocity.x) + airAcceleration >= maxAirSpeed)
            {
                //go max speed
                rb.velocity = new Vector2(-maxAirSpeed, rb.velocity.y);
            }
            else
            {
                //add accelleration vector if not
                rb.velocity -= new Vector2(airAcceleration, 0);
            }
        }
        //change direction if appropriate
        if (transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }

    }

    public void performMove(Move move)
    {
        StartCoroutine(MoveCoroutine(move));
    }

    //TODO: prevent player-facing KB
    void applyKnockback(Vector2 collisionDirection, float moveScalar, Vector2 moveDirection, float hitstunDuration)
    {
        Vector2 transformedMoveDirection = moveDirection;

        //flip move knockback over x axis if the player is facing left
        if (transform.localScale.x < 0)
        {
            transformedMoveDirection = new Vector2(-moveDirection.x, moveDirection.y);
        }
        else 
        {
            transformedMoveDirection = new Vector2(moveDirection.x, moveDirection.y);
        }
        Vector2 appliedKnockback = (collisionDirection + transformedMoveDirection);
        appliedKnockback = (appliedKnockback) * (moveScalar) * (damage * .1f);
        rb.velocity += appliedKnockback;
        StartCoroutine(HitstunCoroutine(hitstunDuration));
    }

    public void respawn() 
    {
        if (stocks == 0)
        {
            //TODO: Gameover Animation
            print("Game Over! No more respawns.");
        }
        else 
        {
            this.stocks--;
            //TODO: Constant for damage reset
            this.damage = 0f;
            //TODO: Constant for velocity reset
            rb.velocity = new Vector2(0f, 0f);
            this.transform.position = this.respawnLoc;
            this.state = PlayerState.idle;
        }
        
    }

    IEnumerator MoveCoroutine(Move move)
    {

        state = PlayerState.warmUp;
        yield return new WaitForSeconds(move.warmUpDuration);

        state = PlayerState.attack;
        yield return new WaitForSeconds(move.executionDuration);

        state = PlayerState.coolDown;
        yield return new WaitForSeconds(move.coolDownDuration);

        state = PlayerState.idle;
    }

    /**Takes a hitstun duration from a move, scales it to the player's current damage, and then sets that player to that state for that amount of time
     */
    IEnumerator HitstunCoroutine(float hitstunDuration) 
    {
        this.state = PlayerState.stun;
        float scaledHitstunDuration = hitstunDuration * damage * hitstunDamageScalar;
        yield return new WaitForSeconds(scaledHitstunDuration);
        this.state = PlayerState.idle;

    }

    IEnumerator InvincibilityCoroutine(float invincibilityDuration)
    {
        this.isInvincible = true;
        sr.color -= new Color(0, 0, 0, 100f);
        yield return new WaitForSeconds(invincibilityDuration);
        this.isInvincible = false;
        sr.color += new Color(0, 0, 0, 100f);
    }

}

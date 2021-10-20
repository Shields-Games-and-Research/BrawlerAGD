using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer sr;

    /***GENERATOR PARAMETERS: Intended to be moderated by generator
     * TODO: Refactor constants into a separate object and initialize  them with a generator
     * 
     * 
     */
    public static float VELOCITY = 3;
    public static float JUMP_FORCE = 3;
    public static float AIR_JUMP_FORCE = 3;

    public static KeyCode LEFT = KeyCode.A;
    public static KeyCode RIGHT = KeyCode.D;
    public static KeyCode JUMP = KeyCode.W;
    public static KeyCode FALL = KeyCode.S;
    public static KeyCode MOVE_1 = KeyCode.Space;

    public static bool DEFAULT_SPRITE_DIRECTION = true;

    /**PLAYER MOVEMENT: These track the characteristics of a specific character instance at any given point.
     * Changing these parameters will alter the movement abilities of players.
     */

    //velocity applied while a key is depressed.
    public float velocity = VELOCITY;

    //force applied to each jump 
    public float groundJumpForce = JUMP_FORCE;

    //force applied to each air jump
    public float airJumpForce = AIR_JUMP_FORCE;

    //true is right, false is left. Used for animation and move direction. TODO change this for readability
    public bool defaultSpriteDirection = DEFAULT_SPRITE_DIRECTION;

    /**PRIVATE PARAMETERS: 
     * Parameters used for internal logic or defined rules in our design space.
     */
    private bool isGrounded;
    private bool jumpsExhausted;

    /**STATE MANAGEMENT:
     * Refactor to separate classes eventually
     */
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

    public PlayerState state = PlayerState.idle;

    /**CONTROLLER: 
     * enables us to assign different codes to the character
     */

    //Input Assignment
    private KeyCode left = LEFT;
    private KeyCode right = RIGHT;
    private KeyCode up = JUMP;
    private KeyCode fall = FALL;
    private KeyCode move1 = MOVE_1;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        rb.freezeRotation = true;
    }
 
    // Decide on State and then apply corresponding policies to said state
    void Update()
    {
        switch (state) {
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

    /**A player, from idle, can:
     * move
     * jump
     * attack
     * be attacked
     */
    void updateIdle() 
    {
        if (Input.GetKey(right)) { moveRight(); }
        else if (Input.GetKey(left)) { moveLeft(); }
        if (Input.GetKeyDown(up)) { jump(); }
    }

    /**A player, from air, can:
     * jump
     * move
     * attack
     * be attacked
     */
    void updateAir() 
    {
        if (Input.GetKey(right)) { moveRight(); }
        else if (Input.GetKey(left)) { moveLeft(); }
        if (Input.GetKeyDown(up)) { jump(); }
    }

    /**A player, from air jumps exhausted, can:
     * move
     */
    void updateAirJumpsExhausted() 
    {
        if (Input.GetKey(right)) { moveRight(); }
        if (Input.GetKey(left)) { moveLeft(); }
    }

    /**PLAYER ACTIONS
     * 
     * 
     * 
     * 
     */
 
    private void jump() 
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

    private void moveRight() 
    {
        rb.velocity = new Vector2(velocity, rb.velocity.y);
        sr.flipX = !defaultSpriteDirection;
    }

    private void moveLeft()
    {
        rb.velocity = new Vector2(-velocity, rb.velocity.y);
        sr.flipX = defaultSpriteDirection;
    }

    void updateWarmUp() { }
    void updateAttack() { }
    void updateCoolDown() { }
    void updateLanding() { }
    void updateStun() { }


    void activateMove(Move move) 
    { 
        
    }

    //When a collision begins, this method is called
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = true;
            jumpsExhausted = false;
            state = PlayerState.idle;
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


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    /**COMPONENT DECLARATION*/
    Rigidbody2D rb;
    SpriteRenderer sr;

    /**PREFAB DECLARATION */
    public Move move;

    /**PLAYER MECHANICS: These track the characteristics of a specific character instance at any given point. Changing these parameters will alter the movement abilities of players. */
    
    //velocity applied while a key is depressed.
    public float velocity = 3;

    //force applied to each jump 
    public float groundJumpForce = 3;

    //force applied to each air jump
    public float airJumpForce = 3;

    //true is right, false is left. Used for animation and move direction. TODO change this for readability
    public bool defaultSpriteDirection = true;

    /**PLAYER MOVESET: these instance variables will be used to manage the generated moves of a player. */
    public Move move1;

    /**PRIVATE PARAMETERS: Parameters used for internal logic or defined rules in our design space. */
    private bool isGrounded;
    private bool jumpsExhausted;

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


    // Start is called before the first frame update
    void Start()
    {
        //set relevant game objects as instance variables for performant access
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        rb.freezeRotation = true;

        //instantiate moves on player creation
        move1 = Instantiate<Move>(move, transform.position + new Vector3(1,0,0), Quaternion.identity, transform);
        
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
        sr.color = Color.white;

        if (Input.GetKey(rightKey)) { moveRight(); }
        else if (Input.GetKey(leftKey)) { moveLeft(); }

        if (Input.GetKeyDown(jumpKey)) { jump(); }

        if (Input.GetKeyDown(move1Key)) { performMove(); }
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

        if (Input.GetKey(rightKey)) { moveRight(); }
        else if (Input.GetKey(leftKey)) { moveLeft(); }

        if (Input.GetKeyDown(jumpKey)) { jump(); }

        if (Input.GetKeyDown(move1Key)) { performMove(); }

    }

    /**A player, from air jumps exhausted, can:
     * move
     */
    void updateAirJumpsExhausted() 
    {
        sr.color = Color.grey;
        if (Input.GetKey(rightKey)) { moveRight(); }
        if (Input.GetKey(leftKey)) { moveLeft(); }
    }

    /**A player, from warm up, can:
     * 
     */
    void updateWarmUp() 
    {
        sr.color = Color.yellow;
    
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

    private void performMove() 
    {
        StartCoroutine(MoveCoroutine());  
    }

    void updateLanding() { }
    void updateStun() { }


    IEnumerator MoveCoroutine() 
    {
        print("Move Coroutine activated.");
        state = PlayerState.warmUp;
        yield return new WaitForSeconds(1f);
        state = PlayerState.attack;
        yield return new WaitForSeconds(1f);
        state = PlayerState.coolDown;
        yield return new WaitForSeconds(1f);
        state = PlayerState.idle;
    }

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

    //When a collision ends, this method is called
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = false;
        }
    }


}

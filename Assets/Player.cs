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

    /**PLAYER MOVESET: these instance variables will be used to manage the generated moves of a player. */
    public Move move1;

    /**PRIVATE PARAMETERS: Parameters used for internal logic or defined rules in our design space. */
    private bool isGrounded;
    private bool jumpsExhausted;
    private float damage = 0;

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
        //set relevant game objects as instance variables for performant access
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        rb.freezeRotation = true;

        //instantiate moves on player creation
        move1 = Instantiate<Move>(move, transform.position + new Vector3(1, 0, 0), Quaternion.identity, transform);

    }

    // Start is called before the first frame update
    void Start()
    {

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

        if (Input.GetKeyDown(move1Key)) { performMove(move1); }
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

        if (Input.GetKeyDown(move1Key)) { performMove(move1); }

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
        if (transform.localScale.x < 0) 
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    private void moveLeft()
    {
        rb.velocity = new Vector2(-velocity, rb.velocity.y);
        if (transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }

    }

    private void performMove(Move move) 
    {
        StartCoroutine(MoveCoroutine(move));  
    }

    void updateLanding() { }
    void updateStun() { }


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

    void applyKnockback(Vector2 direction, float scalar) 
    {
        rb.velocity += (scalar) * (direction) * (damage * .1f);//TODO: make hardcoded float a constant for the game generator
    }

    void OnTriggerEnter2D(Collider2D collision) 
    {
        //print(collision.gameObject.ToString());
        if (collision.gameObject.CompareTag("Attack")) 
        {
            Move tempMove = collision.gameObject.GetComponent<Move>();
            Vector2 knockbackDir = (transform.position - collision.gameObject.transform.position).normalized;
            applyKnockback(knockbackDir, tempMove.knockbackScalar);
            damage += tempMove.damageGiven;
            print("Damage: " + damage);
            //TODO make direction flip

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

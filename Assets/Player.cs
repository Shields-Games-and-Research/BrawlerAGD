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
    public const float VELOCITY = 3;
    public const float JUMP_FORCE = 3;
    public const float AIR_JUMP_FORCE = 3;

    public const KeyCode LEFT = KeyCode.A;
    public const KeyCode RIGHT = KeyCode.D;
    public const KeyCode JUMP = KeyCode.W;
    public const KeyCode FALL = KeyCode.S;
    public const KeyCode MOVE_1 = KeyCode.Space;

    public const bool DEFAULT_SPRITE_DIRECTION = true;

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

    /**CONTROLLER: 
     * enables us to assign different codes to the character
     */

    //Input Assignment
    private KeyCode left = LEFT;
    private KeyCode right = RIGHT;
    private KeyCode jump = JUMP;
    private KeyCode fall = FALL;
    private KeyCode move1 = MOVE_1;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {

        //Left/Right Movement
        //change velocity, update sprite
        if (Input.GetKey(right))
        { 
            rb.velocity = new Vector2(velocity, rb.velocity.y);
            sr.flipX = !defaultSpriteDirection;
        }
        else if (Input.GetKey(left))
        {
            rb.velocity = new Vector2(-velocity, rb.velocity.y);
            sr.flipX = defaultSpriteDirection;
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

        //Single Jump
        if (Input.GetKeyDown(jump))
        {
            if (isGrounded == true)
            {
                rb.velocity = new Vector2(rb.velocity.x, groundJumpForce);
            }
            else if (jumpsExhausted == false)
            {
                rb.velocity = new Vector2(rb.velocity.x, airJumpForce);
                jumpsExhausted = true;
            }
        }
    }

    //When a collision begins, this method is called
    void OnCollisionEnter2D(Collision2D collision)
    {
        print("reached");
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = true;
            jumpsExhausted = false;
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

/**KNOWN BUGS:
 * 1. Players refresh jumps if they have a platform both above and below them
 * 
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Player : MonoBehaviour
{
    /**ARENA MANAGEMENT REFERENCE*/
    public ArenaManager arenaManager;

    /**COMPONENT DECLARATION*/
    public Rigidbody2D rb;
    public SpriteRenderer sr;
    public BoxCollider2D bc;
    public CapsuleCollider2D cc;

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

    /**ENGINE PARAMETERS: Parameters used for internal logic or defined rules in our design space. */
    public bool isGrounded;
    public bool jumpsExhausted;
    public float damage = 0;
    public bool isInvincible = false;
    public int spriteIndex = 0;
    
    //Shows initial UI
    public Text playerDetails;
    //Reference to notifications board
    public Text notifications;
    //Player Name
    public string playerName = "Lorem";

    /**TRACKING NUMBERS FOR EVALUATION*
     * Number of Hits Received
     * Total Damage Taken
     * Number of Recovery Transitions
     */
    public float totalDamage = 0;
    public float totalRecoveryStateTransition = 0;
    public float totalHitsReceived = 0;

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
        //Sprite[] playerSprites = Resources.LoadAll<Sprite>("players");
        //this.playerSpriteIndex = Random.Range(0, playerSprites.Length);

        //set relevant game objects as instance variables for performant access
        rb = GetComponent<Rigidbody2D>();
        
        sr = GetComponent<SpriteRenderer>();
        //sr.sprite = playerSprites[playerSpriteIndex];

        rb.freezeRotation = true;
        
    }

    // Start is called before the first frame update
    void Start()
    {


    }


    // Decide on State and then apply corresponding policies to said state
    void Update()
    {
        if (arenaManager.UIEnabled) 
        {
            updatePlayerHUD();
        }

        //check if controller has a behavior or not
        this.controller.Update();
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

    void updatePlayerHUD() 
    {
        if (arenaManager.UIEnabled) 
        {
            playerDetails.text =
                playerName + "\n" +
                "Damage: " + damage + "%\n" +
                "Stocks: " + stocks + "\n" +
                "State: " + state;
        }
        
    }

    void updateNotifications(string message) 
    {
        if (arenaManager.UIEnabled) 
        {
            arenaManager.UpdateNotifications(message);
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

        if (controller.GetKey(controller.rightKey)) { moveRight(); }
        else if (controller.GetKey(controller.leftKey)) { moveLeft(); }

        if (controller.GetKeyDown(controller.jumpKey)) { jump(); }

        if (controller.GetKeyDown(controller.move1Key)) { performMove(move1); }
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

        if (controller.GetKey(controller.rightKey)) { moveRight(); }
        else if (controller.GetKey(controller.leftKey)) { moveLeft(); }

        if (controller.GetKeyDown(controller.jumpKey)) { jump(); }

        if (controller.GetKeyDown(controller.move1Key)) { performMove(move1); }

    }

    /**A player, from air jumps exhausted, can:
     * move
     */
    void updateAirJumpsExhausted()
    {
        sr.color = Color.grey;
        if (controller.GetKey(controller.rightKey)) { moveRight(); }
        if (controller.GetKey(controller.leftKey)) { moveLeft(); }
    }

    /**A player, from warm up, can:
     * move
     */
    void updateWarmUp()
    {
        sr.color = Color.yellow;
        if (controller.GetKey(controller.rightKey)) { moveRight(); }
        if (controller.GetKey(controller.leftKey)) { moveLeft(); }
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
        LayerMask mask = LayerMask.GetMask("Floor");
        RaycastHit2D platformHit = Physics2D.Raycast(this.transform.position, -Vector2.up, Mathf.Infinity, mask, -Mathf.Infinity, Mathf.Infinity);
        RaycastHit2D ceilingHit = Physics2D.Raycast(this.transform.position, Vector2.up, Mathf.Infinity, mask, -Mathf.Infinity, Mathf.Infinity);

        //if the player is touching a tile, the raycast is reasonably within the player's height, and is not null... 
        //Magic number is to try and make sure that we correctly recover regardless of character size.
        if (
            collision.gameObject.CompareTag("Floor") && 
            (Mathf.Abs(platformHit.distance) <= this.sr.bounds.size.y*2f && platformHit.collider != null) &&
            (Mathf.Abs(platformHit.distance) <= Mathf.Abs(ceilingHit.distance) || ceilingHit.distance == 0)
            )
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
        //Player has been hit by a move and is not currently invincible
        if (collision.gameObject.CompareTag("Attack") && !this.isInvincible)
        {
            Move tempMove = collision.gameObject.GetComponent<Move>();
            this.damage += tempMove.damageGiven;
            Vector2 collKnockbackDir = (transform.position - collision.gameObject.transform.position);
            this.applyKnockback(collKnockbackDir, tempMove.knockbackScalar, tempMove.knockbackDirection, tempMove.hitstunDuration);
            StartCoroutine(InvincibilityCoroutine(0.1f));
        }
        //Player has left the arena
        if (collision.gameObject.CompareTag("Arena"))
        {
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

    void OnTriggerEnter2D(Collider2D collision) 
    {
        //Player has been hit by a move and is not currently invincible
        if (collision.gameObject.CompareTag("Attack") && !this.isInvincible)
        {
            Move tempMove = collision.gameObject.GetComponent<Move>();
            this.damage += tempMove.damageGiven;
            this.totalDamage += tempMove.damageGiven;
            this.totalHitsReceived++;
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
            if (this.jumpsExhausted)
            {
                this.state = PlayerState.airJumpsExhausted;
            }
            else
            {
                this.state = PlayerState.air;
            }
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
            this.rb.velocity = new Vector2(rb.velocity.x, groundJumpForce);
            this.state = PlayerState.air;
        }
        else if (!this.jumpsExhausted)
        {
            this.rb.velocity = new Vector2(rb.velocity.x, airJumpForce);
            this.jumpsExhausted = true;
            this.state = PlayerState.airJumpsExhausted;
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
            StartCoroutine(arenaManager.NotificationCoroutine(this.playerName + " HAS LOST THE GAME"));
            arenaManager.EndGame();
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
            StartCoroutine(arenaManager.NotificationCoroutine(this.playerName + " HAS LOST A STOCK"));
        }
        
    }

    public void destroy()
    {
        Destroy(this.gameObject);
    }

    /** Assignment of values from the Serialized Object. TODO: Static evaluators
 * See player object for detailed field information
 * TODO: Move to player
*/
    public void InitializePlayerFromSerializedObj(SerializedPlayer serializedPlayer, Vector2 spawnLoc)
    {
        //assigns controls of player
        this.leftKey = serializedPlayer.leftKey;
        this.rightKey = serializedPlayer.rightKey;
        this.jumpKey = serializedPlayer.jumpKey;
        this.move1Key = serializedPlayer.attackKey;

        //Player parameter initialization
        this.playerName = serializedPlayer.playerName;
        this.stocks = serializedPlayer.stocks;
        this.groundAcceleration = serializedPlayer.groundAcceleration;
        this.airAcceleration = serializedPlayer.airAcceleration;
        this.maxGroundSpeed = serializedPlayer.maxGroundSpeed;
        this.maxAirSpeed = serializedPlayer.maxAirSpeed;
        this.groundJumpForce = serializedPlayer.groundJumpForce;
        this.airJumpForce = serializedPlayer.airJumpForce;
        this.hitstunDamageScalar = serializedPlayer.hitstunDamageScalar;
        this.respawnLoc = spawnLoc;
        this.transform.localScale = new Vector2(serializedPlayer.widthScalar, serializedPlayer.heightScalar);
        this.rb.gravityScale = serializedPlayer.gravityScalar;
        this.rb.mass = serializedPlayer.mass;
        this.rb.drag = serializedPlayer.drag;
        // Sprite initialization
        Sprite[] playerSprites = Resources.LoadAll<Sprite>("players");
        this.spriteIndex = serializedPlayer.spriteIndex;
        this.sr.sprite = playerSprites[this.spriteIndex];
    }

    /**Creates a move object in the player according to the player's relative position, then calls move instantiate to complete
     */
    public void InitializeMoveFromSerializedObj(SerializedMove serializedMove) 
    {
        //TODO: move instantiation to make object safe should be done in player awake/start, not here
        Vector2 center = this.transform.position + new Vector3(serializedMove.moveLocX, serializedMove.moveLocY);
        //instantiates a move to a player and sets location relative to the player
        this.move1 = Instantiate<Move>(move, center, Quaternion.identity, this.transform);
        this.move1.center = center;
        this.move1.InitializeMoveFromSerializedObj(serializedMove);
    }

    IEnumerator MoveCoroutine(Move move)
    {

        state = PlayerState.warmUp;
        yield return new WaitForSeconds(move.warmUpDuration);

        state = PlayerState.attack;
        yield return new WaitForSeconds(move.executionDuration);

        state = PlayerState.coolDown;
        yield return new WaitForSeconds(move.coolDownDuration);
        //TODO branch depending on air/ground
        if (this.isGrounded)
        {
            state = PlayerState.idle;
        }
        else 
        {
            if (!this.jumpsExhausted)
            {
                state = PlayerState.air;
            }
            else 
            {
                state = PlayerState.airJumpsExhausted;
            }
        }
       
    }

    /**Takes a hitstun duration from a move, scales it to the player's current damage, and then sets that player to that state for that amount of time
     */
    IEnumerator HitstunCoroutine(float hitstunDuration) 
    {
        this.state = PlayerState.stun;
        float scaledHitstunDuration = hitstunDuration * damage * hitstunDamageScalar;
        yield return new WaitForSeconds(scaledHitstunDuration);
        if (this.isGrounded)
        {
            state = PlayerState.idle;
        }
        else
        {
            if (!this.jumpsExhausted)
            {
                state = PlayerState.air;
            }
            else
            {
                state = PlayerState.airJumpsExhausted;
            }
        }
    }

    //TODO
    IEnumerator InvincibilityCoroutine(float invincibilityDuration)
    {
        this.isInvincible = true;
        sr.color -= new Color(0, 0, 0, 200f);
        yield return new WaitForSeconds(invincibilityDuration);
        this.isInvincible = false;
        sr.color += new Color(0, 0, 0, 200f);
    }

}

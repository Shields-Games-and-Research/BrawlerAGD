using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public SpriteRenderer sr;
    public BoxCollider2D bc;
    /**GENERATOR PARAMETERS: Intended to be created by generator
     * 
     * 
     */

    public Vector3 center;
    public float xScale;
    public float yScale;
    
    public float zRotation = 0;

    //determines execution of the move
    public float warmUpDuration;
    public float executionDuration;
    public float coolDownDuration;

    //damage applied from hitting with the move
    public float damageGiven;

    //additional directional base knockback
    //applied in the direction of the player
    public Vector2 knockbackDirection;

    //Determines knockback intensity
    public float knockbackScalar;

    //Determines hitstun duration
    public float hitstunDuration;

    //Shield fields
    // Is this move a shield? 0 = false, 1 = true
    public int isShield;
    // Can this shield break? 0 = false, 1 = true
    public bool canBreak;
    // How much damage a shield can take before breaking
    public float damageDurability = 0.0f;
    // The scale when the shield has full size (hasn't taken damage (example from smash bros.))
    public float fullSizeScale = 1f;
    // The scale when the shield has the smallest size (before breaking (example from smash bros.))
    public float smallSizeScale = 0.2f;
    // which shield state can the parry. 0 = disabled, 1 = startup, 2 = mid, 3 = end
    public float parryState = 0;
    // starting tick that a player can parry in, inclusive
    public float parryWindowStart = 0;
    // ending tick that a player can parry in, inclusive
    public float parryWindowEnd = 0;
    // which shield state can the parry. 0 = disabled, 1 = startup, 2 = mid, 3 = end
    public float reflectionState = 0;
    // starting tick that a player can reflect in, inclusive
    public float reflectionWindowStart = 0;
    // ending tick that a player can reflect in, inclusive
    public float reflectionWindowEnd = 0;
    
    //index of fetched move sprite
    public int spriteIndex = 0;

    //parent player
    public Player player;


    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        bc = GetComponent<BoxCollider2D>();
        //disable before starting play
        SetInactive();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Keep move enabled, but disable collision and visuals
    public void SetInactive() 
    {
        bc.enabled = false;
        sr.enabled = false;
    }

    //enable collision and visuals
    public void SetActive() 
    {
        bc.enabled = true;
        sr.enabled = true;
    }

    void OnCollisionEnter2D(Collision2D collision) 
    {

    }

    void OnCollisionExit2D(Collision2D collision) 
    { 
    
    }

    /** Assignment of values from the Serialized Object. Requires a player as a parameter as a dependency
    *  See move definition for field information
    */
    public void InitializeMoveFromSerializedObj(SerializedMove serializedMove)
    {
        //Sets width and height of move
        this.transform.localScale = new Vector2(serializedMove.widthScalar, serializedMove.heightScalar);
        //Move Parameter initialization
        isShield = serializedMove.isShield;
        
        this.warmUpDuration = serializedMove.warmUpDuration;
        this.executionDuration = serializedMove.executionDuration;
        this.coolDownDuration = serializedMove.coolDownDuration;
        switch (isShield)
        {
            case 0:
                //Debug.Log("NO S");
                this.damageGiven = serializedMove.damageGiven;
                this.knockbackScalar = serializedMove.knockbackScalar;
                this.knockbackDirection = new Vector2(serializedMove.knockbackModX, serializedMove.knockbackModY).normalized;
                this.hitstunDuration = serializedMove.hitstunDuration;
                
                break;
            case 1:
                //Debug.Log("YES SHIELD");
                canBreak = serializedMove.canBreak;
                damageDurability = serializedMove.damageDurability;
                fullSizeScale = serializedMove.fullSizeScale;
                smallSizeScale = serializedMove.smallSizeScale;
                parryState = serializedMove.parryState;
                parryWindowStart = serializedMove.parryWindowStart;
                parryWindowEnd = serializedMove.parryWindowEnd;
                reflectionState = serializedMove.reflectionState;
                reflectionWindowStart = serializedMove.reflectionWindowStart;
                reflectionWindowEnd = serializedMove.reflectionWindowEnd ;

                break;
            default:
                Debug.Log("INVALID Shield");
                break;
        }
        
        // Sprite initialization
        Sprite[] moveSprites = Resources.LoadAll<Sprite>("moves");
        this.spriteIndex = serializedMove.spriteIndex;
        this.sr.sprite = moveSprites[this.spriteIndex];
    }


}

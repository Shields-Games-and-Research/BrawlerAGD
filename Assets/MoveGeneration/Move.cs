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
        this.warmUpDuration = serializedMove.warmUpDuration;
        this.executionDuration = serializedMove.executionDuration;
        this.coolDownDuration = serializedMove.coolDownDuration;
        this.damageGiven = serializedMove.damageGiven;
        this.knockbackScalar = serializedMove.knockbackScalar;
        this.knockbackDirection = new Vector2(serializedMove.knockbackModX, serializedMove.knockbackModY).normalized;
        this.hitstunDuration = serializedMove.hitstunDuration;
        if (serializedMove.spriteIndex >= 0)
        {
            this.spriteIndex = serializedMove.spriteIndex;
        }
        else
        {
            Sprite[] moveSprites = Resources.LoadAll<Sprite>("moves");
            int randomMoveSpriteIndex = UnityEngine.Random.Range(0, moveSprites.Length);
            this.spriteIndex = randomMoveSpriteIndex;
            this.sr.sprite = moveSprites[this.spriteIndex];
        }

    }


}

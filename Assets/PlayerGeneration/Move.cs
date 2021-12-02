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


    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        //Sprite[] moveSprites = Resources.LoadAll<Sprite>("moves");
        //this.moveSpriteIndex = Random.Range(0, moveSprites.Length);
        //sr.sprite = moveSprites[this.moveSpriteIndex];


        bc = GetComponent<BoxCollider2D>();
        SetInactive();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetInactive() 
    {
        bc.enabled = false;
        sr.enabled = false;
    }

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


}

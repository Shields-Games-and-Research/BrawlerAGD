using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    SpriteRenderer sr;
    CircleCollider2D cc;
    /**GENERATOR PARAMETERS: Intended to be created by generator
     * 
     * 
     */

    public Vector3 center = new Vector3(1, 0, 0);
    public float xScale = 1;
    public float yScale = 1;
    
    public float zRotation = 0;

    //determines execution of the move
    public float warmUpDuration = 0.1f;
    public float executionDuration = 0.3f;
    public float coolDownDuration = 0.1f;

    //damage applied from hitting with the move
    public float damageGiven = 10;

    //additional directional base knockback
    public Vector2 knockbackDirection = new Vector2(1f, 1f);

    //Determines knockback intensity
    public float knockbackScalar = 5;


    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CircleCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        SetInactive();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetInactive() 
    {
        transform.gameObject.SetActive(false);
    }

    public void SetActive() 
    {
        transform.gameObject.SetActive(true);
    }

    void OnCollisionEnter2D(Collision2D collision) 
    {

    }

    void OnCollisionExit2D(Collision2D collision) 
    { 
    
    }


}

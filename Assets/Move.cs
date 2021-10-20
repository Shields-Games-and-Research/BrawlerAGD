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

    public Vector2 CENTER = new Vector2(1, 0);
    public float X_SCALE = 1;
    public float Y_SCALE = 1;
    
    public float Z_ROTATION = 0;

    public float WARM_UP_DURATION = 1;
    public float EXECUTION_DURATION = 1;
    public float COOL_DOWN_DURATION = 1;



    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CircleCollider2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rb;

    //velocity applied
    public float speed = 10;

    public float jumpForce = 1;
    public bool isGrounded;
    public bool jumpsExhausted;
    public int maxJumps;

    //Input Assignment
    private KeyCode left = KeyCode.A;
    private KeyCode right = KeyCode.D;
    private KeyCode jump = KeyCode.W;
    private KeyCode fall = KeyCode.S;
    private KeyCode move1 = KeyCode.Space;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Left/Right Movement
        if (Input.GetKey(right))
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
        }
        else if (Input.GetKey(left))
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
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
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
            else if (jumpsExhausted == false)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
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

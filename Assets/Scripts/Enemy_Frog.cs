using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Frog : Enemy
{
    private Rigidbody2D rb;
    //private Animator ani;
    private Collider2D col2d;
    public LayerMask Ground;


    public Transform leftpoint, rightpoint;

    private bool Faceleft = true;

    public float speed, JumpForce;

    private float leftx, rightx;
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();// get father's Start
        rb = GetComponent<Rigidbody2D>();
        //transform.DetachChildren();
        //ani = GetComponent<Animator>();
        col2d = GetComponent<Collider2D>();

        leftx = leftpoint.position.x;
        rightx = rightpoint.position.x;
        Destroy(leftpoint.gameObject);
        Destroy(rightpoint.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        SwitchAnimation();
    }

    void Movement()
    {
        if(Faceleft) // move left
        {
            
            if (col2d.IsTouchingLayers(Ground)) // is on the ground
            {
                ani.SetBool("Jumping", true);
                rb.velocity = new Vector2(-speed, JumpForce); // - means move left
            }
            
            if(transform.position.x < leftx + speed) // turn right
            {
                rb.velocity = new Vector2(1, JumpForce); // smoothly turn right
                transform.localScale = new Vector3(-1, 1, 1); // face to right
                Faceleft = false;
            }
        }
        else // move right
        {
            if (col2d.IsTouchingLayers(Ground))
            {
                ani.SetBool("Jumping", true);
                rb.velocity = new Vector2(speed, JumpForce);
            }

            if (transform.position.x > rightx - speed) // turn left
            {
                rb.velocity = new Vector2(1, JumpForce);
                transform.localScale = new Vector3(1, 1, 1); // face to left
                Faceleft = true;
            }
        }
    }

    void SwitchAnimation()
    {
        if (ani.GetBool("Jumping"))
        {
            if (rb.velocity.y < 0.1)
            {
                ani.SetBool("Jumping", false);
                ani.SetBool("Falling", true);
            }
        }

        if(col2d.IsTouchingLayers(Ground) && ani.GetBool("Falling"))
        {
            ani.SetBool("Falling", false);
        }
    }

    
}

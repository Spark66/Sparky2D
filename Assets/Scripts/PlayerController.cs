using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator ani;

    public float speed = 10f;
    public float jumpForce;
    public LayerMask ground;
    public Collider2D col2d;

    public AudioSource jumpAudio;
    public AudioSource hurtAudio;
    public AudioSource cherryAudio;
    public Joystick joystick;

    public Text CherryNum;

    public int counter = 0; // for counting the number of collections

    private bool isHurt; // is false as default

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate() // Using FixedUpdate to make it move smoothly on different devices
    {
        if(!isHurt)
        {
            Movement();
        }
        SwitchAnimations();
    }

    void Movement()
    {
        // Getting the inputs from keyboard
        float horizontalMove = Input.GetAxis("Horizontal"); // -1 left, 0 no movement, 1 right

        float playerDirecton = Input.GetAxisRaw("Horizontal"); // Get the value only from -1, 0, 1

        // Judging the movement
        
            rb.velocity = new Vector2(horizontalMove * speed * Time.deltaTime, rb.velocity.y); // y is jumping so no change

            // Add animation to run
            ani.SetFloat("Running", Mathf.Abs(playerDirecton)); // Using Abs() for avoiding when the player turn left the value will be changed to negative value.
        

        if(playerDirecton != 0)
        {
            transform.localScale = new Vector3(playerDirecton, 1, 1); // Only change the x-axis direction
        }

        // Judging the jumping
        if(Input.GetButtonDown("Jump") && col2d.IsTouchingLayers(ground))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce * Time.deltaTime); // y-axis movement
            jumpAudio.Play();
            // add animation to jump
            ani.SetBool("Jumping", true);
        }
    }


    void SwitchAnimations()
    {
        ani.SetBool("Idling", false);

        // Drop down from an object
        if (rb.velocity.y < 0.1f && !col2d.IsTouchingLayers(ground))
        {
            ani.SetBool("Falling", true);
        }

        // Judging the jumping
        if (ani.GetBool("Jumping"))
        {
            // if there is no force on y-axis, switch from jump animation to fall animation
            if (rb.velocity.y < 0)
            {
                ani.SetBool("Jumping", false);
                ani.SetBool("Falling", true);
            }
        }
        else if(isHurt)
        {
            ani.SetBool("Hurt", true);
            ani.SetFloat("Running", 0); // Switch back to idle
            if (Mathf.Abs(rb.velocity.x) < 0.1f)
            {
                ani.SetBool("Hurt", false);
                ani.SetBool("Idling", true);
                isHurt = false;
            }
        }
        else if (col2d.IsTouchingLayers(ground))
        {
            ani.SetBool("Falling", false);
            ani.SetBool("Idling", true);
        }
    }

    // Trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // if the player hits the cherry, destroy it
        if(collision.tag == "Collection")
        {
            cherryAudio.Play();
            Destroy(collision.gameObject);
            counter += 1;
            CherryNum.text = counter.ToString();
        }

        if (collision.tag == "DeadBound")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    // Destroy the enemies
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If the player jump on the object's head, the destroy it
        if(collision.gameObject.tag == "Enemy")
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            //Enemy_Frog frog = collision.gameObject.GetComponent<Enemy_Frog>(); // frog can invoke all components in Enemy_Frog
            if (ani.GetBool("Falling"))
            {
                // Destroy(collision.gameObject);
                enemy.JumpOn();

                rb.velocity = new Vector2(rb.velocity.x, jumpForce * Time.deltaTime); // y-axis movement
                ani.SetBool("Jumping", true);
            }
            // if the player is at the left of the enemy
            else if (transform.position.x < collision.gameObject.transform.position.x)
            {
                rb.velocity = new Vector2(-10, rb.velocity.y);
                hurtAudio.Play();
                isHurt = true;
            }
            // if the player is at the right of the enemy
            else if (transform.position.x > collision.gameObject.transform.position.x)
            {
                rb.velocity = new Vector2(10, rb.velocity.y);
                hurtAudio.Play();
                isHurt = true;
            }
        }  
    }

}

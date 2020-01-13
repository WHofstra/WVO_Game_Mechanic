using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    const float SPEED = 10.0f;
    const float JUMPING_HEIGHT = 15.0f;

    [SerializeField]
    private Vector3 _beginPosition;

    private Rigidbody2D rb;

    private float hor;
    private float ver;
    private int jumpTimer;
    private int jumps;

    private bool[] jumping = new bool[2];
    private bool[] touchingWall = new bool[2];

    public float Hor { get { return hor; } set { hor = value; } }
    public float Ver { get { return rb.velocity.y; }}
    public int Jumps { get { return jumps; } set { jumps = value; } }
    public bool Jumping { get { return jumping[0]; } set { jumping[0] = value; } }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ResetPlayer();
    }

    private void ResetPlayer()
    {
        transform.position = _beginPosition;

        jumping[0] = false;
        jumping[1] = false;
        touchingWall[0] = false;
        touchingWall[1] = false;

        jumpTimer = 0;
        jumps = 2;
    }

    private void Update()
    {
        hor = Input.GetAxis("Horizontal");
        ver = Input.GetAxis("Vertical");

        KeyInputsVelocity();
        CheckJumpTime(jumping[1], 1.0f);
    }

    private void FixedUpdate()
    {
        WallVelocity(jumping[1], jumping[0]);
    }

    private void KeyInputsVelocity()
    {
        //Jump-key
        if (((Input.GetKeyDown(KeyCode.Space) && !Input.GetKey(KeyCode.W)) || 
             (Input.GetKeyDown(KeyCode.W) && !Input.GetKey(KeyCode.Space))) && (jumps > 0))
        {
            rb.velocity += new Vector2(0.0f, JUMPING_HEIGHT);
            jumps--;
            jumping[1] = true;
            jumping[0] = true;
        }
    }

    private void CheckJumpTime(bool jumpTime, float secs)
    {
        //Check a Certain Amount of Seconds before the Player Could Jump Again
        if (jumpTime)
        {
            jumpTimer++;

            if ((jumpTimer * Time.deltaTime) >= secs)
            {
                jumping[1] = false;
            }
        }
        else
        {
            jumpTimer = 0;
        }

        //Double Jumping is Fully Functional Once the Player Lands on the Ground
        if (!jumping[0])
        {
            jumps = 2;
        }
    }

    private void WallVelocity(bool jumpTime, bool jumping)
    {
        //Checking Collisions with Walls
        if (!jumping && !touchingWall[0] && !touchingWall[1])
        {
            rb.velocity = new Vector2(hor * SPEED, ver * SPEED + rb.velocity.y);

            //Amplifying Breaking Distance
            if (hor > -0.1f && hor < 0.1f && hor != 0)
            {
                rb.AddForce(new Vector2(hor * SPEED * 200.0f, 0.0f));
            }
        }
        else if (!jumpTime && !touchingWall[0] && !touchingWall[1])
        {
            rb.velocity = new Vector2(hor * SPEED, ver * SPEED + rb.velocity.y);
        }
        else if (jumpTime && !touchingWall[0] && !touchingWall[1])
        {
            rb.velocity = new Vector2(hor * SPEED, (rb.velocity.y * 0.985f));
        }
        else
        {
            touchingWall[0] = false;
            touchingWall[1] = false;

            //If the Right-side of a Wall is Touched
            if (touchingWall[0] && !touchingWall[1])
            {
                if (hor > 0)
                {
                    rb.velocity = new Vector2(hor * SPEED, JUMPING_HEIGHT);
                }
                else if (hor < 0)
                {
                    rb.velocity = new Vector2(0.0f, ver) * SPEED - new Vector2(0.0f, rb.gravityScale / 10);
                }
                else
                {
                    rb.velocity = new Vector2(0.0f, ver) * SPEED - new Vector2(0.0f, rb.gravityScale);
                }
            }
            //If the Left-side of a Wall is Touched
            else if (!touchingWall[0] && touchingWall[1])
            {
                if (hor < 0)
                {
                    rb.velocity = new Vector2(hor * SPEED, JUMPING_HEIGHT);
                }
                else if (hor > 0)
                {
                    rb.velocity = new Vector2(0.0f, ver) * SPEED - new Vector2(0.0f, rb.gravityScale / 10);
                }
                else
                {
                    rb.velocity = new Vector2(0.0f, ver) * SPEED - new Vector2(0.0f, rb.gravityScale);
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == 8)
        {
            jumping[0] = false;
            jumping[1] = false;
        }
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        if ((col.gameObject.layer == 8) && (col.gameObject.CompareTag("WallTile")) && (hor != 0))
        {
            if (col.gameObject.transform.position.x < transform.position.x)
            {
                //The Right-side of a Wall is Touched
                touchingWall[0] = true;
                touchingWall[1] = false;
            }
            else if (col.gameObject.transform.position.x > transform.position.x)
            {
                //The Left-side of a Wall is Touched
                touchingWall[1] = true;
                touchingWall[0] = false;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if ((col.gameObject.layer == 8) && (col.gameObject.CompareTag("WallTile")))
        {
            //No Wall is Touched
            touchingWall[0] = false;
            touchingWall[1] = false;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        //Check Bounds
        if (col.gameObject.layer == 9)
        {
            ResetPlayer();
        }
    }
}

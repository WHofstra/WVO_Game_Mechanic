using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    const float SPEED = 10.0f;
    const float JUMPING_HEIGHT = 15.0f;

    [SerializeField]
    private Vector3 _begingPosition;

    private Rigidbody2D rb;

    private float hor;
    private float ver;
    private int jumpTimer;

    private bool[] jumping = new bool[2];
    private bool[] touchingWall = new bool[2];

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ResetPlayer();
    }

    private void ResetPlayer()
    {
        transform.position = _begingPosition;

        jumping[0] = false;
        jumping[1] = false;
        touchingWall[0] = false;
        touchingWall[1] = false;
        jumpTimer = 0;
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
        WallVelocity(jumping[1]);
    }

    private void KeyInputsVelocity()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) && !jumping[0])
        {
            jumping[1] = true;
            rb.velocity += new Vector2(0.0f, JUMPING_HEIGHT);
            jumping[0] = true;
        }
    }

    private void CheckJumpTime(bool jumpTime, float secs)
    {
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
    }

    private void WallVelocity(bool jumpTime)
    {
        if (!jumpTime && !touchingWall[0] && !touchingWall[1])
        {
            rb.velocity = new Vector2(hor * SPEED, ver * SPEED + rb.velocity.y);
        }
        else if (jumpTime && !touchingWall[0] && !touchingWall[1])
        {
            rb.velocity = new Vector2(hor * SPEED, rb.velocity.y);
        }
        else
        {
            touchingWall[0] = false;
            touchingWall[1] = false;

            if (touchingWall[0] && !touchingWall[1])
            {
                if (hor > 0)
                {
                    rb.velocity = new Vector2(hor * SPEED, JUMPING_HEIGHT);
                }
                else if (hor < 0)
                {
                    rb.velocity = new Vector2(0.0f, ver) * SPEED - new Vector2(0.0f, rb.gravityScale / 5);
                }
                else
                {
                    rb.velocity = new Vector2(0.0f, ver) * SPEED - new Vector2(0.0f, rb.gravityScale);
                }
            }
            else if (!touchingWall[0] && touchingWall[1])
            {
                if (hor < 0)
                {
                    rb.velocity = new Vector2(hor * SPEED, JUMPING_HEIGHT);
                }
                else if (hor > 0)
                {
                    rb.velocity = new Vector2(0.0f, ver) * SPEED - new Vector2(0.0f, rb.gravityScale / 5);
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
                touchingWall[0] = true;
                touchingWall[1] = false;
            }
            else if (col.gameObject.transform.position.x > transform.position.x)
            {
                touchingWall[1] = true;
                touchingWall[0] = false;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if ((col.gameObject.layer == 8) && (col.gameObject.CompareTag("WallTile")))
        {
            touchingWall[0] = false;
            touchingWall[1] = false;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.layer == 9)
        {
            ResetPlayer();
        }
    }
}

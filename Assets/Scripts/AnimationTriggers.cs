using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTriggers : MonoBehaviour
{
    const float SPEED = 5.0f;

    private Animator animator;
    private PlayerMovement movement;
    private SpriteRenderer renderer;

    private void Start()
    {
        animator = GetComponent<Animator>();
        movement = GetComponentInParent<PlayerMovement>();
        renderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (movement.Hor > 0)
        {
            renderer.flipX = false;
        }
        else if (movement.Hor < 0)
        {
            renderer.flipX = true;
        }

        animator.SetFloat("HorizontalFloat", (movement.Hor * SPEED));
        animator.SetFloat("VerticalFloat", (movement.Ver));
        animator.SetInteger("HorizontalInt", (int)(movement.Hor * SPEED));
        animator.SetInteger("Jumps", movement.Jumps);
        animator.SetBool("Jumping", movement.Jumping);
    }
}

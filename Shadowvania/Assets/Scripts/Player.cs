using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    //Config

    [SerializeField]
    private float runSpeed = 1f;

    [SerializeField]
    private float jumpSpeed = 1f;

    [SerializeField]
    private float climbSpeed = 1f;

    [SerializeField]
    private float jumpDuration = 1f;


    //State

    private bool isAlive = true;
    private bool isJumping = false;


    //Cached component references

    private Rigidbody2D rb;
    private Animator animator;
    private Collider2D collider;
    private float defaultGravityScale;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        collider = GetComponent<Collider2D>();
        defaultGravityScale = rb.gravityScale;
    }

    private void Update()
    {
        Run();
        Jump();
        ClimbRope();
        Flip();
    }

    private void Run()
    {
        float controlThrow = CrossPlatformInputManager.GetAxis("Horizontal"); // -1 to +1
        var velocity = new Vector2(controlThrow * runSpeed, rb.velocity.y);
        rb.velocity = velocity;

        print(velocity);

        bool hasHorizontalSpeed = Mathf.Abs(rb.velocity.x) > Mathf.Epsilon;
        animator.SetBool("IsRunning", hasHorizontalSpeed);
    }

    private void Jump()
    {
        if (!collider.IsTouchingLayers(LayerMask.GetMask("Ground", "Climbing")))
        {
            return;
        }

        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            isJumping = true;
            var jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
            rb.velocity += jumpVelocityToAdd;

            animator.SetTrigger("Jump");

            StartCoroutine(ResetJump());
        }
    }

    private void ClimbRope()
    {
        if (!collider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            animator.SetBool("IsClimbing", false);
            rb.gravityScale = defaultGravityScale;
            return;
        }

        rb.gravityScale = 0f;

        float controlThrow = Input.GetAxis("Vertical");
        Vector2 playerVelocity = new Vector2(rb.velocity.x, controlThrow * climbSpeed);
        rb.velocity = playerVelocity;

        bool playerHasVerticalSpeed = Mathf.Abs(rb.velocity.y) > Mathf.Epsilon;
        bool isOnRope = playerHasVerticalSpeed || !collider.IsTouchingLayers(LayerMask.GetMask("Ground"));
        animator.SetBool("IsClimbing", isOnRope);
    }

    private void Flip()
    {
        var xVelocity = rb.velocity.x;
        bool hasHorizontalSpeed = Mathf.Abs(rb.velocity.x) > Mathf.Epsilon;

        if (hasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(xVelocity), 1f);
        }
    }

    private IEnumerator ResetJump()
    {
        yield return new WaitForSeconds(jumpDuration);
        isJumping = false;
    }

}

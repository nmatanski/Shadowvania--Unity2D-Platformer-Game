using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    [SerializeField]
    private int health = 1;

    [SerializeField]
    private Vector2 knockback = new Vector2(25f, 25f);

    [SerializeField]
    private PhysicsMaterial2D deathMaterial;


    //State

    private bool isAlive = true;
    private bool isJumping = false;
    private bool isTransitioning = false;


    //Cached component references

    private Rigidbody2D rb;
    private Animator animator;
    private CapsuleCollider2D bodyCollider;
    private BoxCollider2D feetCollider;
    private float defaultGravityScale;
    private PhysicsMaterial2D defaultMaterial;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        bodyCollider = GetComponent<CapsuleCollider2D>();
        feetCollider = GetComponent<BoxCollider2D>();
        defaultGravityScale = rb.gravityScale;
        defaultMaterial = GetComponent<BoxCollider2D>().sharedMaterial;
    }

    private void Update()
    {
        if (!isAlive)
        {
            return;
        }

        Run();
        Jump();
        ClimbRope();
        Flip();
        Die();
    }

    private void Run()
    {
        float controlThrow = CrossPlatformInputManager.GetAxis("Horizontal"); // -1 to +1
        var velocity = new Vector2(controlThrow * runSpeed, rb.velocity.y);
        rb.velocity = velocity;

        bool hasHorizontalSpeed = Mathf.Abs(rb.velocity.x) > Mathf.Epsilon;

        bool isClimbing = feetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"));
        animator.SetBool("IsRunning", hasHorizontalSpeed && !isClimbing);
    }

    private void Jump()
    {
        if (!feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground", "Climbing", "Stopper")))
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
        float controlThrow = Input.GetAxis("Vertical");

        if (!feetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")) || isJumping)
        {
            StopClimbing();
            return;
        }
        else if (controlThrow == 0)
#pragma warning disable S1871 // Two branches in a conditional structure should not have exactly the same implementation
        {
            StopClimbing();
            return;
        }
#pragma warning restore S1871 // Two branches in a conditional structure should not have exactly the same implementation

        rb.gravityScale = 0f;

        Vector2 playerVelocity = new Vector2(rb.velocity.x, controlThrow * climbSpeed);
        rb.velocity = playerVelocity;

        bool playerHasVerticalSpeed = Mathf.Abs(rb.velocity.y) > Mathf.Epsilon;
        bool isOnRope = playerHasVerticalSpeed || !feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
        animator.SetBool("IsClimbing", isOnRope);
    }

    private void StopClimbing()
    {
        animator.SetBool("IsClimbing", false);
        rb.gravityScale = defaultGravityScale;
    }

    private void Die()
    {
        if (bodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemy")))
        {
            isAlive = false;
            animator.SetTrigger("Die");
            GetComponent<Rigidbody2D>().velocity = knockback;

            Physics2D.IgnoreLayerCollision(10, 13, true); ///TODO: Make this false on respawn
            GetComponent<BoxCollider2D>().sharedMaterial = deathMaterial;
            StartCoroutine(ChangeToDefaultMaterialAfterSeconds(2f)); /// this also restarts the scene
        }
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

    private IEnumerator ChangeToDefaultMaterialAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        GetComponent<BoxCollider2D>().sharedMaterial = defaultMaterial;
        Physics2D.IgnoreLayerCollision(10, 13, false);
        SceneManager.LoadScene("Sandbox");
    }
}

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
    private float jumpDuration = 1f;

    [Range(0f, 1f)]
    [SerializeField]
    private float jumpHeightCut = .5f;

    [SerializeField]
    private float climbSpeed = 1f;

    [SerializeField]
    private float jumpPressedRememberTime = .1f;

    [SerializeField]
    private float groundedRememberTime = .1f;

    [Range(0f, .5f)]
    [SerializeField]
    private float horizontalDampingWhenStopping = 1f;

    [Range(0f, .5f)]
    [SerializeField]
    private float horizontalDampingWhenTurning = 1f;

    [Range(0f, .5f)]
    [SerializeField]
    private float basicHorizontalDamping = 1f;

    [SerializeField]
    private int health = 1;

    [SerializeField]
    private Vector2 knockback = new Vector2(25f, 25f);

    [SerializeField]
    private PhysicsMaterial2D deathMaterial;



    //State

    private bool isAlive = true;
    private bool isJumping = false;
    private float defaultGravityScale;
    private float jumpPressedRemember = 0f;
    private float groundedRemember = 0f;
    private float defaultStoppingDamping;
    private float defaultTurningDamping;
    private float defaultBasicDamping;


    //Cached component references

    private Rigidbody2D rb;
    private Animator animator;
    private CapsuleCollider2D bodyCollider;
    private BoxCollider2D feetCollider;
    private PhysicsMaterial2D defaultMaterial;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        bodyCollider = GetComponent<CapsuleCollider2D>();
        feetCollider = GetComponent<BoxCollider2D>();
        defaultGravityScale = rb.gravityScale;
        defaultMaterial = GetComponent<BoxCollider2D>().sharedMaterial;

        defaultStoppingDamping = horizontalDampingWhenStopping;
        defaultTurningDamping = horizontalDampingWhenTurning;
        defaultBasicDamping = basicHorizontalDamping;
    }

    private void Update()
    {
        if (!isAlive)
        {
            return;
        }

        RandomizeRunningVelocityDamping();
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

        ///test

        if (Mathf.Abs(CrossPlatformInputManager.GetAxisRaw("Horizontal")) < 0.01f)
        {
            velocity.x *= Mathf.Pow(1f - horizontalDampingWhenStopping, Time.deltaTime * 10f);
        }
        else if (Mathf.Abs(CrossPlatformInputManager.GetAxisRaw("Horizontal")) != Mathf.Sign(velocity.x))
        {
            velocity.x *= Mathf.Pow(1f - horizontalDampingWhenTurning, Time.deltaTime * 10f);
        }
        else
        {
            velocity.x *= Mathf.Pow(1f - basicHorizontalDamping, Time.deltaTime * 10f);
        }

        ///test end

        rb.velocity = velocity;

        bool hasHorizontalSpeed = Mathf.Abs(rb.velocity.x) > Mathf.Epsilon;

        bool isClimbing = feetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"));
        animator.SetBool("IsRunning", hasHorizontalSpeed && !isClimbing);
    }

    private void Jump()
    {
        var hasPressedJumpDown = CrossPlatformInputManager.GetButtonDown("Jump");
        var hasPressedJumpUp = CrossPlatformInputManager.GetButtonUp("Jump");

        var isGrounded = feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground", "Climbing", "Stopper"));


        groundedRemember -= Time.deltaTime;
        if (isGrounded)
        {
            groundedRemember = groundedRememberTime;
        }

        jumpPressedRemember -= Time.deltaTime;
        if (hasPressedJumpDown)
        {
            jumpPressedRemember = jumpPressedRememberTime;
        }

        if (hasPressedJumpUp && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpHeightCut);
        }

        //if (!isGrounded)
        //{
        //    return;
        //}

        if (jumpPressedRemember > 0 && groundedRemember > 0) ///hasPressedJump changed with the timer
        {
            jumpPressedRemember = 0;
            groundedRemember = 0;

            isJumping = true;
            var jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
            rb.velocity = jumpVelocityToAdd; ///it was += but it adds more than the regular when climbing ladder 'cause it already has a velocity and it adds to it

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

    private void RandomizeRunningVelocityDamping()
    {
        horizontalDampingWhenStopping = Random.Range(defaultStoppingDamping - .005f, defaultStoppingDamping + .005f);
        horizontalDampingWhenTurning = Random.Range(defaultTurningDamping - .005f, defaultTurningDamping + .005f);
        basicHorizontalDamping = Random.Range(defaultBasicDamping - .005f, defaultBasicDamping + .005f);
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

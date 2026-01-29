using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float maxMovementSpeed;
    [Space(5)]
    [SerializeField][Range(0, 20)] private float groundAcceleration;
    [SerializeField][Range(0, 20)] private float airAcceleration;

    [Header("Jump")]
    [SerializeField] private float jumpForce;
    [Space(5)]
    [SerializeField] private float jumpGravityForce;
    [SerializeField] private float fallGravityForce;
    [Space(5)]
    [Min(0)][SerializeField] public int countOfJump = 1;
    [Space(5)]
    [SerializeField] private float coyoteTime = 0.1f;

    [Header("Ground detection")]
    [SerializeField] private Transform groundChecker;
    [SerializeField] private Vector2 groundCheckerSize;
    [SerializeField] private LayerMask groundLayer;

    [Header("Technical stuff")]
    [SerializeField] private Rigidbody2D rigidbody;

    float horizontalInput;
    bool isGrounded;
    bool wasGrounded;
    bool jumpIsHeld;
    bool jumpPressed;
    int currentJumps = 0;
    float coyoteTimer;

    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        jumpIsHeld = Input.GetButton("Jump");
        jumpPressed = Input.GetButtonDown("Jump");

        CheckGround();
        JumpManager();

        if (isGrounded) coyoteTimer = coyoteTime;
        else coyoteTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        ApplyGravity();
        Move();
    }

    void Move()
    {
        float targetSpeed = horizontalInput * maxMovementSpeed;
        float acceleration = isGrounded ? groundAcceleration : airAcceleration;

        float newSpeed = Mathf.MoveTowards(rigidbody.velocity.x, targetSpeed, acceleration * Time.fixedDeltaTime);

        rigidbody.velocity = new Vector2(newSpeed, rigidbody.velocity.y);
    }

    void JumpManager()
    {
        if (isGrounded && !wasGrounded)
        {
            currentJumps = 0;

            if (jumpIsHeld)
            {
                Jump();
            }
        }

        if ((isGrounded || (coyoteTimer > 0f && currentJumps == 0)) && jumpPressed)
        {
            Jump();
            return;
        }

        if (!isGrounded && jumpPressed && currentJumps < countOfJump && currentJumps > 0)
        {
            Jump();
        }

    }

    private void Jump()
    {
        rigidbody.velocity = new Vector2(rigidbody.velocity.x,jumpForce);

        currentJumps++;
        coyoteTimer = 0f;
    }

    void CheckGround()
    {
        wasGrounded = isGrounded;

        isGrounded = Physics2D.OverlapBox(groundChecker.position, groundCheckerSize, 0, groundLayer);
    }

    void ApplyGravity()
    {
        if (isGrounded) return;

        rigidbody.gravityScale = (rigidbody.velocity.y > 0) ? jumpGravityForce : fallGravityForce;
    }

    private void OnDrawGizmos()
    {
        if (groundChecker == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(groundChecker.position, (Vector3)groundCheckerSize);
    }
}

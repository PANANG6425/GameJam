using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float acceleration = 30f;
    [SerializeField] private float deceleration = 40f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float jumpCutMultiplier = 0.5f;
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float jumpBufferTime = 0.2f;
    
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);

    private Rigidbody2D rb;
    private float horizontalInput;
    private bool isGrounded;
    private bool isRunning;
    private bool isCrouching;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private CapsuleCollider2D capsuleCollider;
    private float originalHeight;
    private float originalOffset;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        originalHeight = capsuleCollider.size.y;
        originalOffset = capsuleCollider.offset.y;
    }

    private void FixedUpdate()
    {
        // Calculate target speed (cannot run while crouching)
        float targetSpeed = horizontalInput * ((isRunning && !isCrouching) ? runSpeed : walkSpeed);

        // Choose acceleration or deceleration based on whether we are providing input
        float accelRate = (Mathf.Abs(horizontalInput) > 0.01f) ? acceleration : deceleration;

        // Move the current velocity towards the target speed
        float newVelocityX = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, accelRate * Time.fixedDeltaTime);
        
        // Apply the new velocity
        rb.linearVelocity = new Vector2(newVelocityX, rb.linearVelocity.y);
        
        // Check if the player is touching the ground
        isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);

        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.fixedDeltaTime;
        }

        jumpBufferCounter -= Time.fixedDeltaTime;

        // Perform jump if both jump buffer and coyote time are valid
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            
            // Reset counters to prevent double jumps
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
        }
    }

    // Hooked to "Move" action via Player Input Component
    public void OnMove(InputAction.CallbackContext context)
    {
        // Reads Vector2 input and captures the X axis (Left/Right)
        horizontalInput = context.ReadValue<Vector2>().x;
    }

    // Hooked to "Run" action via Player Input Component
    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.performed) isRunning = true;
        else if (context.canceled) isRunning = false;
    }

    // Hooked to "Jump" action via Player Input Component
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            jumpBufferCounter = jumpBufferTime;
        }

        // If the button is released and we are moving upwards, cut the jump short
        if (context.canceled)
        {
            if (rb.linearVelocity.y > 0f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
            }
            
            jumpBufferCounter = 0f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualizes the ground check box in the Editor
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
        }
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isCrouching = true;
            capsuleCollider.size = new Vector2(capsuleCollider.size.x, originalHeight / 2f);
            capsuleCollider.offset = new Vector2(capsuleCollider.offset.x, originalOffset - (originalHeight / 4f));
        }
        else if (context.canceled)
        {
            isCrouching = false;
            capsuleCollider.size = new Vector2(capsuleCollider.size.x, originalHeight);
            capsuleCollider.offset = new Vector2(capsuleCollider.offset.x, originalOffset);
        }
    }
}
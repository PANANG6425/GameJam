using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float jumpCutMultiplier = 0.5f;
    
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);

    private Rigidbody2D rb;
    private float horizontalInput;
    private bool isGrounded;
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
        // Apply horizontal movement
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        
        // Check if the player is touching the ground
        isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);
    }

    // Hooked to "Move" action via Player Input Component
    public void OnMove(InputAction.CallbackContext context)
    {
        // Reads Vector2 input and captures the X axis (Left/Right)
        horizontalInput = context.ReadValue<Vector2>().x;
    }

    // Hooked to "Jump" action via Player Input Component
    public void OnJump(InputAction.CallbackContext context)
    {
        // Check if the button was pressed down this frame AND player is grounded
        if (context.started && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // If the button is released and we are moving upwards, cut the jump short
        if (context.canceled && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
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
            capsuleCollider.size = new Vector2(capsuleCollider.size.x, originalHeight / 2f);
            capsuleCollider.offset = new Vector2(capsuleCollider.offset.x, originalOffset - (originalHeight / 4f));
        }
        else if (context.canceled)
        {
            capsuleCollider.size = new Vector2(capsuleCollider.size.x, originalHeight);
            capsuleCollider.offset = new Vector2(capsuleCollider.offset.x, originalOffset);
        }
    }
}
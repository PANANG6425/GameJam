using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(HitPoint))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    private float walkSpeed = 5f;

    [SerializeField]
    private float runSpeed = 10f;

    [SerializeField]
    private float acceleration = 30f;

    [SerializeField]
    private float deceleration = 40f;

    [SerializeField]
    private float jumpForce = 12f;

    [SerializeField]
    private float jumpCutMultiplier = 0.5f;

    [SerializeField]
    private float coyoteTime = 0.2f;

    [SerializeField]
    private float jumpBufferTime = 0.2f;

    [Header("Ground Check")]
    [SerializeField]
    private Transform groundCheck;

    [SerializeField]
    private LayerMask groundLayer;

    [SerializeField]
    private Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);

    private Rigidbody2D rb;
    private float horizontalInput;
    private bool isGrounded = true;
    private bool isRunning;
    private bool isCrouching;

    private enum LocoState
    {
        Grounded,
        Jumping,
        Falling,
        Landing,
        Crouching,
        Hit,
    }

    private LocoState locoState = LocoState.Grounded;
    public bool IsIdle => isGrounded && Mathf.Abs(horizontalInput) <= 0.01f && !isCrouching;

    // Aiming / firing / reloading own the body's animation.
    private bool CombatBusy =>
        revolver != null && (revolver.IsAiming || revolver.IsQuickFiring || revolver.IsReloading);

    // The player is rooted in place while crouching or busy with the revolver.
    private bool MovementLocked => isCrouching || CombatBusy;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private CapsuleCollider2D capsuleCollider;
    private float originalHeight;
    private float originalOffset;
    private Animator animator;
    private Revolver revolver;
    private Shovel shovel;
    private HitPoint hp;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        originalHeight = capsuleCollider.size.y;
        originalOffset = capsuleCollider.offset.y;
        animator = GetComponentInChildren<Animator>();
        revolver = GetComponentInChildren<Revolver>();
        shovel = GetComponentInChildren<Shovel>();
        hp = GetComponent<HitPoint>();

        if (hp != null)
        {
            hp.onDamageTaken.AddListener(OnPlayerHit);
        }
        GlobalEvent.HealthChange.Invoke(hp.CurrentHP, hp.MaxHP);
    }

    void Start()
    {
        GlobalEvent.IncreaseHealth.AddListener(HealPlayer);
    }

    void HealPlayer(int amount)
    {
        Debug.Log("Heal player: " + amount);
        hp.IncreaseHP(amount);
        GlobalEvent.HealthChange.Invoke(hp.CurrentHP, hp.MaxHP);
    }

    private void OnDestroy()
    {
        if (hp != null)
        {
            hp.onDamageTaken.RemoveListener(OnPlayerHit);
            GlobalEvent.IncreaseHealth.RemoveListener(HealPlayer);
        }
    }

    private bool hitReactRequested = false;

    private void OnPlayerHit()
    {
        if (animator == null)
        {
            return;
        }

        if (shovel != null && shovel.IsAttacking)
        {
            // Hyper armor during melee: ignore the flinch/hit reaction
            return;
        }

        locoState = LocoState.Hit;
        hitReactRequested = true;
        GlobalEvent.HealthChange.Invoke(hp.CurrentHP, hp.MaxHP);
    }

    private void Update()
    {
        bool cantMove = MovementLocked;
        float currentHorizontalInput = cantMove ? 0f : horizontalInput;

        // Flip the player to face the direction they are walking
        if (currentHorizontalInput > 0.01f)
        {
            transform.localScale = new Vector3(
                Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );
        }
        else if (currentHorizontalInput < -0.01f)
        {
            transform.localScale = new Vector3(
                -Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );
        }

        // Update walk animation
        if (animator != null)
        {
            animator.SetBool("IsWalking", Mathf.Abs(currentHorizontalInput) > 0.01f);
            UpdateAirborneAnimation(CombatBusy);
        }
    }

    // Drives the jump / fall / land states. They sit on top of the Idle/Walk/Sprint
    // bool machine: while airborne we Play() the air states directly (they have no
    // transitions, so they hold), then hand control back to the locomotion machine
    // once the landing animation is done.
    private void UpdateAirborneAnimation(bool combatBusy)
    {
        // The hit reaction overrides everything (even combat) until its clip
        // finishes, then hands control back to the locomotion machine.
        if (locoState == LocoState.Hit)
        {
            if (hitReactRequested)
            {
                animator.Play("Anim_Hit", -1, 0f);
                hitReactRequested = false;
                return;
            }

            var info = animator.GetCurrentAnimatorStateInfo(0);
            if (info.IsName("Anim_Hit"))
            {
                if (info.normalizedTime >= 1f)
                    SetLocoState(LocoState.Grounded);
            }
            else if (!animator.GetNextAnimatorStateInfo(0).IsName("Anim_Hit"))
            {
                SetLocoState(LocoState.Grounded);
            }
            return;
        }

        // While a combat animation owns the body (aim/fire/reload), let it play and
        // just keep the air state synced so it resumes correctly afterwards.
        if (combatBusy)
        {
            locoState = LocoState.Grounded;
            return;
        }

        if (!isGrounded)
        {
            SetLocoState(rb.linearVelocity.y > 0.01f ? LocoState.Jumping : LocoState.Falling);
            return;
        }

        // Crouching (grounded) holds the crouch pose over normal locomotion.
        if (isCrouching)
        {
            SetLocoState(LocoState.Crouching);
            return;
        }

        // Grounded: play the landing anim once, then rejoin the locomotion machine.
        if (locoState == LocoState.Jumping || locoState == LocoState.Falling)
        {
            SetLocoState(LocoState.Landing);
        }
        else if (locoState == LocoState.Landing)
        {
            // Let the (non-looping) landing clip play through, but bail early if the
            // player is already moving so control stays responsive.
            var info = animator.GetCurrentAnimatorStateInfo(0);
            bool landingFinished = info.IsName("Anim_Landing") && info.normalizedTime >= 1f;
            bool moving = Mathf.Abs(horizontalInput) > 0.01f;
            if (landingFinished || moving)
            {
                SetLocoState(LocoState.Grounded);
            }
        }
        else if (locoState == LocoState.Crouching)
        {
            // Just stood up - rejoin the Idle/Walk/Sprint machine.
            SetLocoState(LocoState.Grounded);
        }
    }

    private void SetLocoState(LocoState next)
    {
        if (locoState == next)
        {
            return;
        }
        locoState = next;

        switch (next)
        {
            case LocoState.Jumping:
                animator.Play("Anim_Jumping");
                break;
            case LocoState.Falling:
                animator.Play("Anim_Falling");
                break;
            case LocoState.Landing:
                animator.Play("Anim_Landing");
                break;
            case LocoState.Crouching:
                animator.Play("Anim_Crouch");
                break;
            case LocoState.Hit:
                animator.Play("Anim_Hit", -1, 0f);
                break;
            case LocoState.Grounded:
                // Rejoin the Idle/Walk/Sprint machine; its bool transitions take over
                // from Idle immediately if the player is moving.
                animator.Play("Anim_Idle");
                break;
        }
    }

    private void FixedUpdate()
    {
        bool cantMove = MovementLocked;
        float currentHorizontalInput = cantMove ? 0f : horizontalInput;

        // Calculate target speed (cannot run while crouching)
        float targetSpeed =
            currentHorizontalInput * ((isRunning && !isCrouching) ? runSpeed : walkSpeed);

        // Choose acceleration or deceleration based on whether we are providing input
        float accelRate = (Mathf.Abs(currentHorizontalInput) > 0.01f) ? acceleration : deceleration;

        // Move the current velocity towards the target speed
        float newVelocityX = Mathf.MoveTowards(
            rb.linearVelocity.x,
            targetSpeed,
            accelRate * Time.fixedDeltaTime
        );

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

        // Perform jump if both jump buffer and coyote time are valid (and not
        // locked while aiming / quick-firing / reloading)
        if (!cantMove && jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            // Reset counters to prevent double jumps
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
        }
    }

    // Flip the sprite to face a world-space X position. Used by the revolver so
    // shots (and quick-fire bursts) turn the player toward the target.
    public void FaceTowards(float worldX)
    {
        float dx = worldX - transform.position.x;
        if (Mathf.Abs(dx) < 0.01f)
        {
            return;
        }

        float sign = dx >= 0f ? 1f : -1f;
        Vector3 s = transform.localScale;
        transform.localScale = new Vector3(sign * Mathf.Abs(s.x), s.y, s.z);
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
        if (context.performed)
            isRunning = true;
        else if (context.canceled)
            isRunning = false;

        if (animator != null)
        {
            animator.SetBool("IsSprinting", isRunning);
        }
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
                rb.linearVelocity = new Vector2(
                    rb.linearVelocity.x,
                    rb.linearVelocity.y * jumpCutMultiplier
                );
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
            capsuleCollider.offset = new Vector2(
                capsuleCollider.offset.x,
                originalOffset - (originalHeight / 4f)
            );
        }
        else if (context.canceled)
        {
            isCrouching = false;
            capsuleCollider.size = new Vector2(capsuleCollider.size.x, originalHeight);
            capsuleCollider.offset = new Vector2(capsuleCollider.offset.x, originalOffset);
        }
    }
}

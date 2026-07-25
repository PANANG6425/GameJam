using UnityEngine;

public class EnemyPatrolMovement : EnemyMovement
{
    [Header("Patrol Settings")]
    [SerializeField] private float waitTimeMin = 1.5f;
    [SerializeField] private float waitTimeMax = 3f;
    [SerializeField] private float moveTimeMin = 0.5f;
    [SerializeField] private float moveTimeMax = 1.5f;
    
    private float stateTimer;
    private bool isPatrolling = false;
    private int moveDirection = 1;

    protected override void Start()
    {
        base.Start();
        SetWaitState();
    }

    protected override void FixedUpdate()
    {
        if (pauseMovement)
        {
            if (animator != null && !string.IsNullOrEmpty(moveAnimParam)) animator.SetBool(moveAnimParam, false);
            rb.linearVelocityX = 0;
            return;
        }

        isGrounded = groundCheck != null && Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);

        stateTimer -= Time.fixedDeltaTime;
        if (stateTimer <= 0f)
        {
            if (isPatrolling)
                SetWaitState();
            else
                SetPatrolState();
        }

        bool isWalking = false;

        if (!isPatrolling)
        {
            rb.linearVelocityX = 0;
        }
        else if (isGrounded)
        {
            MoveInDirection(moveDirection);
            isWalking = true;
        }

        if (animator != null && !string.IsNullOrEmpty(moveAnimParam))
        {
            animator.SetBool(moveAnimParam, isWalking);
        }
    }

    private void SetWaitState()
    {
        isPatrolling = false;
        stateTimer = Random.Range(waitTimeMin, waitTimeMax);
    }

    private void SetPatrolState()
    {
        isPatrolling = true;
        stateTimer = Random.Range(moveTimeMin, moveTimeMax);
        // Randomly pick left or right
        moveDirection = Random.value > 0.5f ? 1 : -1;
    }

    private void MoveInDirection(int direction)
    {
        rb.linearVelocityX = speed * direction;
        transform.localScale = new Vector3(
            Mathf.Abs(transform.localScale.x) * direction,
            transform.localScale.y,
            transform.localScale.z
        );
    }
}

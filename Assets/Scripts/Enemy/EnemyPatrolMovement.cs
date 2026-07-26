using UnityEngine;

public class EnemyPatrolMovement : EnemyMovement
{
    [Header("Patrol Settings")]
    [SerializeField] private float waitTimeMin = 1.5f;
    [SerializeField] private float waitTimeMax = 3f;
    [SerializeField] private float moveTimeMin = 0.5f;
    [SerializeField] private float moveTimeMax = 1.5f;
    
    [Header("Ledge Detection")]
    [SerializeField] private Transform ledgeCheck;
    [SerializeField] private bool stopAtLedge = true;
    [SerializeField] private float ledgeCheckRadius = 0.1f;

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
            return;
        }

        isGrounded = groundCheck != null && Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);

        bool atLedge = false;
        if (stopAtLedge && ledgeCheck != null)
        {
            // If there's no ground under the ledge check, we are at a ledge
            atLedge = !Physics2D.OverlapCircle(ledgeCheck.position, ledgeCheckRadius, groundLayer);
        }

        bool isWalking = false;

        if (isChasing)
        {
            // Stop patrol logic, focus on chasing
            var distance = Vector3.Distance(transform.position, targetPos);
            nearTarget = distance <= stopDistance;

            if (nearTarget || atLedge)
            {
                rb.linearVelocityX = 0;
            }
            else if (isGrounded)
            {
                MoveToTarget(targetPos);
                isWalking = true;
            }
        }
        else
        {
            // Patrol logic
            stateTimer -= Time.fixedDeltaTime;
            if (stateTimer <= 0f)
            {
                if (isPatrolling)
                    SetWaitState();
                else
                    SetPatrolState();
            }
            else if (isPatrolling && atLedge)
            {
                // Reached a ledge, stop and wait
                SetWaitState();
            }

            if (!isPatrolling)
            {
                rb.linearVelocityX = 0;
            }
            else if (isGrounded)
            {
                MoveInDirection(moveDirection);
                isWalking = true;
            }
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

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        if (ledgeCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(ledgeCheck.position, ledgeCheckRadius);
        }
    }
}

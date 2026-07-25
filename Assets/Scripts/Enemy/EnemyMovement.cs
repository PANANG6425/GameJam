using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    protected Rigidbody2D rb;
    public float speed = 5f;
    public float stopDistance = 1.5f;

    [Header("Ground Check")]
    [SerializeField]
    private Transform groundCheck;

    [SerializeField]
    private LayerMask groundLayer;

    [SerializeField]
    protected Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);

    protected Vector3 targetPos = new();
    protected bool isChasing = false;
    protected bool nearTarget = false;
    protected bool isGrounded = true;

    public bool pauseMovement = false;
    public bool NearTarget => nearTarget;

    [Header("Animation")]
    [SerializeField]
    protected string moveAnimParam = "IsWalking";

    [SerializeField]
    protected Animator animator;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
            if (animator == null) animator = GetComponent<Animator>();
        }
    }

    protected virtual void FixedUpdate()
    {
        if (pauseMovement)
        {
            if (animator != null && !string.IsNullOrEmpty(moveAnimParam)) animator.SetBool(moveAnimParam, false);
            return;
        }

        var distance = Vector3.Distance(transform.position, targetPos);
        nearTarget = distance <= stopDistance;
        isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);

        bool isWalking = false;

        if (nearTarget || !isChasing)
        {
            rb.linearVelocityX = 0;
        }
        else if (isGrounded)
        {
            MoveToTarget(targetPos);
            isWalking = true;
        }

        if (animator != null && !string.IsNullOrEmpty(moveAnimParam))
        {
            animator.SetBool(moveAnimParam, isWalking);
        }
    }


    public void SetTarget(Vector3 position)
    {
        targetPos = position;
    }

    public void SetChasing(bool chasing)
    {
        isChasing = chasing;
    }

    protected virtual void MoveToTarget(Vector3 targetPos)
    {
        if (transform.position.x > targetPos.x)
        {
            rb.linearVelocityX = -speed;
            transform.localScale = new(
                -Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );
        }
        else
        {
            rb.linearVelocityX = speed;
            transform.localScale = new(
                Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );
        }
    }

    protected virtual void OnDrawGizmosSelected()
    {
        // Visualizes the ground check box in the Editor
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
        }
    }
}

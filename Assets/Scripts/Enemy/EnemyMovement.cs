using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    Rigidbody2D rb;
    public float speed = 5f;
    public float stopDistance = 1.5f;

    [Header("Ground Check")]
    [SerializeField]
    private Transform groundCheck;

    [SerializeField]
    private LayerMask groundLayer;

    [SerializeField]
    private Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);

    Vector3 targetPos = new();
    bool isChasing = false;
    bool nearTarget = false;
    bool isGrounded = true;

    public bool NearTarget => nearTarget;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        var distance = Vector3.Distance(transform.position, targetPos);
        nearTarget = distance <= stopDistance;
        isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);

        if (nearTarget || !isChasing)
        {
            rb.linearVelocityX = 0;
        }
        else if (isGrounded)
        {
            MoveToTarget(targetPos);
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

    void MoveToTarget(Vector3 targetPos)
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

    private void OnDrawGizmosSelected()
    {
        // Visualizes the ground check box in the Editor
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
        }
    }
}

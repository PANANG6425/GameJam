using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Rigidbody2D rb;
    public float speed = 5f;
    public float stopDistance = 1.5f;

    [SerializeField]
    Area2D areaDetection;

    [SerializeField]
    HitPoint hp;

    [Header("Ground Check")]
    [SerializeField]
    private Transform groundCheck;

    [SerializeField]
    private LayerMask groundLayer;

    [SerializeField]
    private Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);

    Vector3 playerPos = new();
    bool detectedPlayer = false;
    bool nearPlayer = false;
    bool isGrounded = true;

    // Status effects
    float stunTimer = 0f;
    int burnDamagePerTick;
    float burnTickInterval = 0.5f;
    float burnTickTimer;
    float burnRemaining;

    [Tooltip("How long the enemy rides a knockback (chase disabled) before regaining control.")]
    [SerializeField]
    float knockbackDuration = 0.2f;

    float knockbackTimer = 0f;

    public bool IsStunned => stunTimer > 0f;

    void Start()
    {
        if (areaDetection == null)
        {
            Debug.LogError("Missing Area2D");
            return;
        }
        rb = GetComponent<Rigidbody2D>();
        hp = GetComponent<HitPoint>();
        areaDetection.onEnter.AddListener(OnPlayerEnter);
        areaDetection.onStay.AddListener(OnPlayerStay);
        areaDetection.onExit.AddListener(OnPlayerExit);
    }

    void Update()
    {
        float dt = Time.deltaTime;

        // Shock stun countdown
        if (stunTimer > 0f)
        {
            stunTimer -= dt;
        }

        // Knockback window countdown
        if (knockbackTimer > 0f)
        {
            knockbackTimer -= dt;
        }

        // Incendiary burn damage-over-time
        if (burnRemaining > 0f)
        {
            burnRemaining -= dt;
            burnTickTimer -= dt;
            if (burnTickTimer <= 0f)
            {
                burnTickTimer = burnTickInterval;
                Hit(burnDamagePerTick);
            }
        }
    }

    void FixedUpdate()
    {
        // While being knocked back, let the impulse ride freely (no chase, and
        // don't zero the velocity like the stun does below).
        if (knockbackTimer > 0f)
        {
            return;
        }

        // While stunned the enemy holds still and doesn't chase.
        if (IsStunned)
        {
            if (rb != null)
            {
                rb.linearVelocityX = 0;
            }
            return;
        }

        var distance = Vector3.Distance(transform.position, playerPos);
        nearPlayer = distance <= stopDistance;
        isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);

        if (nearPlayer || !detectedPlayer)
        {
            rb.linearVelocityX = 0;
        }
        else if (isGrounded)
        {
            MoveToPlayer(playerPos);
        }
    }

    void OnDestroy()
    {
        areaDetection.onEnter.RemoveListener(OnPlayerEnter);
        areaDetection.onStay.RemoveListener(OnPlayerStay);
        areaDetection.onExit.RemoveListener(OnPlayerExit);
    }

    void OnPlayerEnter(Collider2D collider)
    {
        var obj = collider.gameObject;
        if (obj.tag == "Player")
        {
            detectedPlayer = true;
            playerPos = obj.transform.position;
            Debug.Log($"[Enemy] detected player at {playerPos}");
        }
    }

    void OnPlayerStay(Collider2D collider)
    {
        var obj = collider.gameObject;
        if (obj.tag == "Player")
        {
            playerPos = obj.transform.position;
        }
    }

    void OnPlayerExit(Collider2D collider)
    {
        var obj = collider.gameObject;
        if (obj.tag == "Player")
        {
            detectedPlayer = false;
        }
    }

    void MoveToPlayer(Vector3 playerPos)
    {
        if (transform.position.x > playerPos.x)
        {
            rb.linearVelocityX = -speed;
        }
        else
        {
            rb.linearVelocityX = speed;
        }
    }

    public void Hit(int damage)
    {
        hp?.DecreaseHP(damage);
        if (hp?.GetCurrentHP() <= 0)
        {
            Destroy(gameObject);
        }
    }

    // Shock bullet - freeze the enemy in place for a short duration.
    public void ApplyStun(float duration)
    {
        stunTimer = Mathf.Max(stunTimer, duration);
    }

    // Melee knockback - launch the enemy with the given velocity and briefly
    // disable its chase so the impulse is visible.
    public void ApplyKnockback(Vector2 velocity)
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        if (rb == null)
        {
            return;
        }
        rb.linearVelocity = velocity;
        knockbackTimer = knockbackDuration;
    }

    // Incendiary bullet - apply fire damage-over-time.
    public void ApplyBurn(int damagePerTick, float tickInterval, float duration)
    {
        burnDamagePerTick = damagePerTick;
        burnTickInterval = Mathf.Max(0.05f, tickInterval);
        burnRemaining = Mathf.Max(burnRemaining, duration);
        if (burnTickTimer <= 0f)
        {
            burnTickTimer = burnTickInterval;
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

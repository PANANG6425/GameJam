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

    void FixedUpdate()
    {
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

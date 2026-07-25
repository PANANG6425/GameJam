using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]
public class Attack : MonoBehaviour
{
    [SerializeField]
    Area2D areaDetection;

    [SerializeField]
    Area2D HitArea;

    public int damage = 1;
    public float attackCooldown = 0.5f;

    [Header("Animation")]
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private string attackAnimationName = "AnimEnemyAttack";

    EnemyMovement movement;
    Collider2D hitAreaCollider;
    HitPoint playerHp;
    Collider2D playerCollider;
    bool detectedPlayer = false;
    bool isAttackWindowOpen = false;
    float cooldownTimer = 0f;

    void Start()
    {
        movement = GetComponent<EnemyMovement>();
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
            if (animator == null) animator = GetComponent<Animator>();
        }

        if (areaDetection == null)
        {
            Debug.LogError("Missing Area2D");
            return;
        }

        areaDetection.onEnter.AddListener(OnPlayerEnter);
        areaDetection.onExit.AddListener(OnPlayerExit);
        if (HitArea == null)
        {
            Debug.LogError("Missing Hit Area");
            return;
        }
        hitAreaCollider = HitArea.GetComponent<Collider2D>();
        HitArea.onEnter.AddListener(HitPlayer);
        HitArea.onStay.AddListener(HitPlayer);
        cooldownTimer = 0;
    }

    void OnDestroy()
    {
        areaDetection.onEnter.RemoveListener(OnPlayerEnter);
        areaDetection.onExit.RemoveListener(OnPlayerExit);
        HitArea.onEnter.RemoveListener(HitPlayer);
        HitArea.onStay.RemoveListener(HitPlayer);
    }

    void FixedUpdate()
    {
        cooldownTimer = Mathf.Max(0f, cooldownTimer - Time.fixedDeltaTime);

        if (!detectedPlayer || cooldownTimer > 0f)
        {
            return;
        }

        if (movement.NearTarget)
        {
            if (!isAttackWindowOpen)
            {
                isAttackWindowOpen = true;
                if (animator != null && !string.IsNullOrEmpty(attackAnimationName))
                {
                    animator.Play(attackAnimationName);
                }
            }
        }

        // A sleeping Rigidbody2D stops Unity from sending OnTriggerStay2D,
        // so check for overlap directly instead of only relying on the event.
        if (
            isAttackWindowOpen
            && playerCollider != null
            && hitAreaCollider.IsTouching(playerCollider)
        )
        {
            HitPlayer(playerCollider);
        }
    }

    void HitPlayer(Collider2D collider)
    {
        // Debug.Log("In hit box");
        if (!isAttackWindowOpen || playerHp == null)
        {
            return;
        }
        isAttackWindowOpen = false;
        playerHp.DecreaseHP(damage);
        cooldownTimer = attackCooldown;
        Debug.Log("Hit Player");
        GlobalEvent.HealthChange.Invoke(playerHp.CurrentHP, playerHp.MaxHP);
        GlobalEvent.IncreaseMadness.Invoke(GlobalData.MADNESS_HIT);
    }

    void OnPlayerEnter(Collider2D collider)
    {
        var obj = collider.gameObject;
        if (obj.tag == "Player")
        {
            detectedPlayer = true;
            playerHp = obj.GetComponent<HitPoint>();
            playerCollider = collider;
        }
    }

    void OnPlayerExit(Collider2D collider)
    {
        var obj = collider.gameObject;
        if (obj.tag == "Player")
        {
            detectedPlayer = false;
            playerHp = null;
            playerCollider = null;
        }
    }
}

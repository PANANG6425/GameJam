using UnityEngine;

// Melee attack (V). A single strike that damages, stuns and knocks the enemy back.
public class Shovel : MonoBehaviour
{
    [SerializeField]
    private Collider2D attackCollider;

    [SerializeField]
    private LayerMask enemyLayers;

    [SerializeField]
    int damage = 2;

    [Tooltip("How long the hit box stays active for a swing.")]
    [SerializeField]
    float attackWindow = 0.15f;

    [Tooltip("Delay before the hit box becomes active after swinging.")]
    [SerializeField]
    float attackDelay = 0.18f;

    [Header("On Hit")]
    [SerializeField]
    float stunDuration = 1f;

    [Tooltip("Horizontal push applied to the enemy, away from the player.")]
    [SerializeField]
    float knockbackForce = 8f;

    [Tooltip("Small upward pop added to the knockback.")]
    [SerializeField]
    float knockbackUp = 2f;

    private bool isAttacking = false;
    private Animator animator;

    private void Awake()
    {
        // Try to find the Animator
        animator = GetComponent<Animator>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (animator == null) animator = GetComponentInParent<Animator>();
    }

    private void Start()
    {
        // Make sure the attack collider starts disabled
        if (attackCollider != null)
        {
            attackCollider.enabled = false;
        }
    }

    // Triggered by the V melee input.
    public void Melee()
    {
        if (isAttacking || attackCollider == null)
        {
            return;
        }

        isAttacking = true;

        if (animator != null)
        {
            animator.Play("Anim_Melee");
        }

        CancelInvoke(nameof(EnableHitbox));
        Invoke(nameof(EnableHitbox), attackDelay);
    }

    private void EnableHitbox()
    {
        if (!isAttacking) return;
        
        attackCollider.enabled = true;

        // Disable the collider after a short window
        CancelInvoke(nameof(EndAttack));
        Invoke(nameof(EndAttack), attackWindow);
    }

    private void EndAttack()
    {
        attackCollider.enabled = false;
        isAttacking = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only register hits during an active attack and on the correct layers
        if (!isAttacking || ((1 << other.gameObject.layer) & enemyLayers) == 0)
        {
            return;
        }

        var enemy = other.gameObject.GetComponent<Enemy>();
        if (enemy == null)
        {
            return;
        }

        enemy.Hit(damage);
        enemy.ApplyStun(stunDuration);

        // Push the enemy away from the player (this component sits on the player root).
        Vector2 dir = other.transform.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f)
        {
            dir = transform.localScale.x >= 0f ? Vector2.right : Vector2.left;
        }
        else
        {
            dir.Normalize();
        }

        Vector2 knockback = dir * knockbackForce + Vector2.up * knockbackUp;
        enemy.ApplyKnockback(knockback);
    }
}

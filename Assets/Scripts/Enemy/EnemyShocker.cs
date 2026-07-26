using UnityEngine;
using System.Collections;

public class EnemyShocker : Enemy
{
    [Header("Shocker Attack")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float attackCooldown = 3f;
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private string attackAnimTrigger = "AnimEnemyShoot";

    private float attackTimer = 0f;
    private bool isAttacking = false;

    protected override void Update()
    {
        base.Update();

        if (isDead || IsStunned || knockbackTimer > 0f) return;

        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        }

        // Distance check (playerPos is updated by Enemy's area detection)
        if (playerPos != Vector3.zero)
        {
            float distance = Vector3.Distance(transform.position, playerPos);
            
            // Face the player when not attacking
            if (!isAttacking && movement != null)
            {
                float sign = (playerPos.x > transform.position.x) ? 1f : -1f;
                transform.localScale = new Vector3(
                    Mathf.Abs(transform.localScale.x) * sign,
                    transform.localScale.y,
                    transform.localScale.z
                );
            }

            if (distance <= attackRange && attackTimer <= 0f && !isAttacking)
            {
                StartCoroutine(AttackSequence());
            }
        }
    }

    private IEnumerator AttackSequence()
    {
        isAttacking = true;
        if (movement != null) movement.pauseMovement = true;
        if (rb != null) rb.linearVelocity = Vector2.zero;

        if (animator != null && !string.IsNullOrEmpty(attackAnimTrigger))
        {
            animator.SetTrigger(attackAnimTrigger);
            
            // Wait roughly half a second for the "shoot" part of the animation
            yield return new WaitForSeconds(0.5f);
            if (!isDead) FireProjectile();
            
            // Wait for animation to finish
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            if (!isDead) FireProjectile();
            yield return new WaitForSeconds(0.5f);
        }

        isAttacking = false;
        if (movement != null) movement.pauseMovement = false;
        attackTimer = attackCooldown;
    }

    // This can also be called via an Animation Event on the Shocker's attack animation
    public void FireProjectile()
    {
        if (projectilePrefab != null && shootPoint != null)
        {
            // Shoot straight depending on which way the enemy is facing
            float angle = transform.localScale.x > 0 ? 0f : 180f;
            
            Instantiate(projectilePrefab, shootPoint.position, Quaternion.Euler(0, 0, angle));
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

using System.Collections;
using UnityEngine;

public class EnemyBomber : Enemy
{
    [Header("Bomber Settings")]
    [SerializeField] float explosionRadius = 2.5f;
    [SerializeField] int explosionDamage = 3;
    [SerializeField] LayerMask playerLayer; 
    [SerializeField] ParticleSystem explosionParticles; // Unique particles for the big explosion

    bool isExploding = false;

    protected override void Update()
    {
        base.Update();

        if (isDead || isExploding || IsStunned) return;

        // Trigger explode when near target (player)
        if (movement != null && movement.NearTarget)
        {
            Die(); // Start the explosion by triggering Die
        }
    }

    public override void Hit(int damage)
    {
        if (isDead || isExploding) return;

        hp?.DecreaseHP(damage);
        if (hp?.GetCurrentHP() <= 0)
        {
            Die();
        }
        // We purposely do NOT call base.Hit() or set "AnimEnemyHit" here 
        // because the bomber has no hit animation.
    }

    protected override void Die()
    {
        if (isExploding) return;
        
        isDead = true; // Mark as dead so Update stops
        
        StartCoroutine(ExplodeSequence());
    }

    private IEnumerator ExplodeSequence()
    {
        isExploding = true;
        
        // Stop moving
        if (movement != null) movement.pauseMovement = true;
        if (rb != null) rb.linearVelocity = Vector2.zero;

        // Play explosion wind-up animation
        if (animator != null) animator.SetTrigger("AnimBBExplode");

        // Wait for the animation to finish (using the same robust method as DeathSequence)
        if (animator != null)
        {
            yield return new WaitUntil(() => 
                animator.GetCurrentAnimatorStateInfo(0).IsName("AnimBBExplode") ||
                animator.GetNextAnimatorStateInfo(0).IsName("AnimBBExplode")
            );

            float elapsed = 0f;
            const float timeout = 3f;
            while (elapsed < timeout)
            {
                var state = animator.GetCurrentAnimatorStateInfo(0);
                if (state.IsName("AnimBBExplode") && state.normalizedTime >= 1f)
                {
                    break;
                }
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(1f);
        }

        // Deal damage to anything in radius
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, playerLayer);
        foreach (var hitCollider in hitColliders)
        {
            HitPoint targetHp = hitCollider.GetComponent<HitPoint>();
            if (targetHp != null)
            {
                targetHp.DecreaseHP(explosionDamage);
            }
        }

        // 💥 CAMERA SHAKE
        // (You can replace this with Cinemachine Impulse Source if you use Cinemachine, 
        // or trigger your own Camera Shake event here)
        if (GlobalEvent.Instance != null)
        {
            // Example if you add a hit stop or shake to GlobalEvent
            GlobalEvent.Instance.TriggerHitStop(0.1f);
        }

        // Play explosion particles if assigned
        if (explosionParticles != null)
        {
            Vector3 worldPos = explosionParticles.transform.position;
            // For the explosion, we want to KEEP the world scale so it spreads fully
            explosionParticles.transform.SetParent(null, true); 
            // Fix negative scale (caused if the enemy was facing left) which breaks particles
            Vector3 ls = explosionParticles.transform.localScale;
            explosionParticles.transform.localScale = new Vector3(Mathf.Abs(ls.x), Mathf.Abs(ls.y), Mathf.Abs(ls.z));

            explosionParticles.transform.position = worldPos;
            explosionParticles.Play();

            var main = explosionParticles.main;
            float lifetime = main.duration + main.startLifetime.constantMax;
            Destroy(explosionParticles.gameObject, lifetime);
        }

        // Kill self instantly (bypasses normal death so it doesn't play standard death animation)
        isDead = true;
        foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
        {
            sr.enabled = false;
        }
        
        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a red circle in the editor so you can easily see/tune the explosion radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}

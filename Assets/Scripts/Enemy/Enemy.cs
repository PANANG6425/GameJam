using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]
public class Enemy : MonoBehaviour
{
    protected EnemyMovement movement;

    [SerializeField]
    protected Area2D areaDetection;

    protected HitPoint hp;

    protected Rigidbody2D rb;
    protected Animator animator;

    [SerializeField]
    protected ParticleSystem deathParticles;

    [Header("Audio")]
    [SerializeField]
    protected AudioClip hitSound;

    [SerializeField]
    protected GameObject hitParticlePrefab;

    [Tooltip("Minimum time between hit particle spawns to prevent spam")]
    [SerializeField]
    protected float hitParticleCooldown = 0.1f;

    protected float lastHitParticleTime = -1f;

    protected bool isDead = false;

    protected Vector3 playerPos = new();

    // Status effects
    protected float stunTimer = 0f;
    protected int burnDamagePerTick;
    protected float burnTickInterval = 0.5f;
    protected float burnTickTimer;
    protected float burnRemaining;

    [Tooltip("How long the enemy rides a knockback (chase disabled) before regaining control.")]
    [SerializeField]
    protected float knockbackDuration = 0.2f;

    protected float knockbackTimer = 0f;

    public bool IsStunned => stunTimer > 0f;

    protected virtual void Start()
    {
        if (areaDetection == null)
        {
            Debug.LogError("Missing Area2D");
            return;
        }
        movement = GetComponent<EnemyMovement>();
        hp = GetComponent<HitPoint>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        if (deathParticles == null)
        {
            deathParticles = GetComponentInChildren<ParticleSystem>(true);
        }
        areaDetection.onEnter.AddListener(OnPlayerEnter);
        areaDetection.onStay.AddListener(OnPlayerStay);
        areaDetection.onExit.AddListener(OnPlayerExit);
    }

    protected virtual void Update()
    {
        if (isDead)
            return;

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

        if (movement != null)
        {
            movement.pauseMovement = (knockbackTimer > 0f || IsStunned);
        }
    }

    protected virtual void FixedUpdate()
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
    }

    protected virtual void OnDestroy()
    {
        areaDetection.onEnter.RemoveListener(OnPlayerEnter);
        areaDetection.onStay.RemoveListener(OnPlayerStay);
        areaDetection.onExit.RemoveListener(OnPlayerExit);
    }

    protected virtual void OnPlayerEnter(Collider2D collider)
    {
        var obj = collider.gameObject;
        if (obj.tag == "Player")
        {
            playerPos = obj.transform.position;
            movement.SetChasing(true);
            movement.SetTarget(playerPos);
        }
    }

    protected virtual void OnPlayerStay(Collider2D collider)
    {
        var obj = collider.gameObject;
        if (obj.tag == "Player")
        {
            playerPos = obj.transform.position;
            movement.SetTarget(playerPos);
        }
    }

    protected virtual void OnPlayerExit(Collider2D collider)
    {
        var obj = collider.gameObject;
        if (obj.tag == "Player")
        {
            movement.SetChasing(false);
        }
    }

    public virtual void Hit(int damage)
    {
        if (isDead)
            return;

        if (hitSound != null)
        {
            GameObject tempAudio = new GameObject("TempHitAudio");
            AudioSource source = tempAudio.AddComponent<AudioSource>();
            source.clip = hitSound;
            // Leave spatialBlend at 0 for 2D sound so it doesn't get quiet
            source.Play();
            Destroy(tempAudio, hitSound.length + 0.1f);
        }

        if (hitParticlePrefab != null && Time.time >= lastHitParticleTime + hitParticleCooldown)
        {
            lastHitParticleTime = Time.time;
            GameObject p = Instantiate(hitParticlePrefab, transform.position, Quaternion.identity);
            ParticleSystem ps = p.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                Destroy(p, ps.main.duration + ps.main.startLifetime.constantMax);
            }
        }

        hp?.DecreaseHP(damage);
        if (hp?.GetCurrentHP() <= 0)
        {
            Die();
        }
        else
        {
            animator?.SetTrigger("AnimEnemyHit");
        }
    }

    protected virtual void Die()
    {
        isDead = true;

        // Disable physics and movement immediately
        if (movement != null)
            movement.enabled = false;

        var col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false; // Stop physics interactions
        }

        // Clear any pending hit trigger (e.g. queued up by a quick-fire burst) so
        // it can't bounce the animator out of the death state and stall the death.
        animator?.ResetTrigger("AnimEnemyHit");
        animator?.SetTrigger("AnimEnemyDie");

        StartCoroutine(DeathSequence());
    }

    protected virtual IEnumerator DeathSequence()
    {
        if (animator != null)
        {
            // Wait for the death animation to finish, but never hang: bail after a
            // safety timeout in case the death state is never cleanly reached.
            float elapsed = 0f;
            const float timeout = 3f;
            while (elapsed < timeout)
            {
                var state = animator.GetCurrentAnimatorStateInfo(0);
                if (state.IsName("AnimEnemyDie") && state.normalizedTime >= 1f)
                {
                    break;
                }
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            // Fallback if there is no animator
            yield return new WaitForSeconds(0.5f);
        }

        // 2. Hide every sprite part of the enemy rig (it's a multi-sprite skeleton,
        // so disabling a single renderer wouldn't hide all of it).
        foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
        {
            sr.enabled = false;
        }

        // 3. Play the death particles. Detach them first so they survive the enemy
        // being destroyed and can finish on their own, then self-destruct.
        if (deathParticles != null)
        {
            Vector3 worldPos = deathParticles.transform.position;
            deathParticles.transform.SetParent(null, false);
            deathParticles.transform.position = worldPos;

            deathParticles.Play();

            var main = deathParticles.main;
            float lifetime = main.duration + main.startLifetime.constantMax;
            Destroy(deathParticles.gameObject, lifetime);
        }

        // 4. Remove the enemy.
        Destroy(gameObject);
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
}

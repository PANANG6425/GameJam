using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 10f;
    public float lifeTime = 5f;
    public int damage = 1;
    
    [Header("Slow Effect")]
    public float slowMultiplier = 0.5f;
    public float slowDuration = 2f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Moves forward based on the rotation set when spawned
        transform.position += transform.right * (speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // Ignore collisions with other enemies
        if (hitInfo.CompareTag("Enemy") || hitInfo.isTrigger)
        {
            return;
        }

        if (hitInfo.CompareTag("Player"))
        {
            // Apply Damage
            var playerHp = hitInfo.GetComponent<HitPoint>();
            if (playerHp != null)
            {
                playerHp.DecreaseHP(damage);
                GlobalEvent.HealthChange.Invoke(playerHp.CurrentHP, playerHp.MaxHP);
                GlobalEvent.PlayerHit.Invoke();
                if (GlobalEvent.Instance != null) GlobalEvent.Instance.TriggerHitStop(0.1f);
            }

            // Apply Slow
            var controller = hitInfo.GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.ApplySlow(slowMultiplier, slowDuration);
            }
        }

        // Destroy on impact with anything (player or environment)
        Destroy(gameObject);
    }
}

using UnityEngine;

// A lingering toxic cloud left on the ground by a Gas bullet. It periodically
// damages any enemy inside its radius, then destroys itself after a duration.
public class GasCloud : MonoBehaviour
{
    [SerializeField]
    float radius = 1.5f;

    [SerializeField]
    int damagePerTick = 1;

    [SerializeField]
    float tickInterval = 0.5f;

    [SerializeField]
    float duration = 5f;

    [SerializeField]
    LayerMask enemyLayers;

    float tickTimer;

    // Optionally called by the Projectile so the cloud hits the same layers the
    // shot was configured for. Leave unused if the prefab already sets enemyLayers.
    public void Configure(LayerMask layers)
    {
        enemyLayers = layers;
    }

    void Start()
    {
        Destroy(gameObject, duration);
    }

    void Update()
    {
        tickTimer -= Time.deltaTime;
        if (tickTimer <= 0f)
        {
            tickTimer = tickInterval;
            DamageEnemiesInRange();
        }
    }

    void DamageEnemiesInRange()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, radius, enemyLayers);
        foreach (var hit in hits)
        {
            var enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.Hit(damagePerTick);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.4f, 1f, 0.2f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}

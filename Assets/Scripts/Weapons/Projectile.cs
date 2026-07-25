using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 15f;
    public float lifeTime = 5f;
    bool markDestroy = false;
    public int damage { get; set; } = 0;

    // Effect payload assigned by the Revolver when the shot is fired.
    public BulletDefinition definition;
    public LayerMask enemyLayers;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += transform.right * (speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // Log the hit for now since there's no health script yet
        Debug.Log("Projectile hit: " + hitInfo.name);

        // Avoid destroying on the player itself if it collides instantly
        if (hitInfo.CompareTag("Player"))
        {
            return;
        }

        Enemy directHit = hitInfo.CompareTag("Enemy") ? hitInfo.GetComponent<Enemy>() : null;

        if (!markDestroy)
        {
            ApplyEffect(directHit);
            GlobalEvent.IncreaseMadness.Invoke(GlobalData.MADNESS_ATK);
        }

        markDestroy = true;
        Destroy(gameObject);
    }

    private void ApplyEffect(Enemy directHit)
    {
        BulletType type = definition != null ? definition.type : BulletType.Normal;

        // Direct impact damage on the enemy actually struck.
        if (directHit != null)
        {
            directHit.Hit(definition != null ? definition.impactDamage : damage);
        }

        if (definition == null)
        {
            return;
        }

        switch (type)
        {
            case BulletType.Incendiary: // Fire DoT
                directHit?.ApplyBurn(
                    definition.burnDamagePerTick,
                    definition.burnTickInterval,
                    definition.burnDuration
                );
                break;

            case BulletType.HighExplosive: // AoE - detonates on any impact
                Explode();
                break;

            case BulletType.Shock: // Short stun
                directHit?.ApplyStun(definition.stunDuration);
                break;

            case BulletType.Gas: // Toxic ground cloud - drops on any impact
                SpawnGasCloud();
                break;

            case BulletType.Flesh:
                // TODO: Drain Madness - to be implemented later.
                break;

            case BulletType.Normal:
            default:
                break;
        }
    }

    private void Explode()
    {
        if (definition.explosionPrefab != null)
        {
            Instantiate(definition.explosionPrefab, transform.position, Quaternion.identity);
        }

        var hits = Physics2D.OverlapCircleAll(
            transform.position,
            definition.explosionRadius,
            enemyLayers
        );
        foreach (var hit in hits)
        {
            var enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.Hit(definition.explosionDamage);
            }
        }
    }

    private void SpawnGasCloud()
    {
        if (definition.gasCloudPrefab == null)
        {
            return;
        }

        var cloudObj = Instantiate(definition.gasCloudPrefab, transform.position, Quaternion.identity);
        var cloud = cloudObj.GetComponent<GasCloud>();
        cloud?.Configure(enemyLayers);
    }
}

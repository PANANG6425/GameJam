using System;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField]
    Area2D explosionArea;

    void Start()
    {
        explosionArea.onEnter.AddListener(OnExplosion);
    }

    void OnDestroy()
    {
        explosionArea.onEnter.RemoveListener(OnExplosion);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        explosionArea.gameObject.SetActive(true);
    }

    void OnExplosion(Collider2D hitInfo)
    {
        var projectile = GetComponent<EnemyProjectile>();
        if (hitInfo.CompareTag("Player"))
        {
            // Apply Damage
            var playerHp = hitInfo.GetComponent<HitPoint>();
            if (playerHp != null)
            {
                playerHp.DecreaseHP(projectile.damage);
                GlobalEvent.HealthChange.Invoke(playerHp.CurrentHP, playerHp.MaxHP);
                GlobalEvent.PlayerHit.Invoke();
                if (GlobalEvent.Instance != null)
                    GlobalEvent.Instance.TriggerHitStop(0.1f);
            }
        }

        // Destroy on impact with anything (player or environment)
        Destroy(gameObject);
    }
}

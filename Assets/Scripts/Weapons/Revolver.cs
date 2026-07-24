using UnityEngine;
using UnityEngine.InputSystem;

public class Revolver : MonoBehaviour
{
    [SerializeField]
    private GameObject projectilePrefab;

    [SerializeField]
    private Transform firePoint;

    [SerializeField]
    int damage = 1;

    [SerializeField]
    float BaseAimAngle = 45;

    [SerializeField]
    float MinAimAngle = 10f;

    [SerializeField]
    float AimSpeed = 0.1f;

    private AccuracyCone accuracyCone;

    private void Awake()
    {
        // Try to find the AccuracyCone script on this object or parent
        accuracyCone = GetComponentInParent<AccuracyCone>();
        if (accuracyCone == null)
        {
            accuracyCone = GetComponent<AccuracyCone>();
        }
    }

    void Start()
    {
        if (accuracyCone == null)
        {
            Debug.LogError("There is no AccuracyCone Componenet");
            return;
        }
        accuracyCone.SetBaseAngle(BaseAimAngle);
        accuracyCone.SetAimSpeed(AimSpeed);
        accuracyCone.SetMinAimAngle(MinAimAngle);
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        // Forward the input to the AccuracyCone script so it can draw the accuracy cone
        accuracyCone?.OnLeftClick(context);

        if (context.canceled)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (projectilePrefab != null && accuracyCone != null)
        {
            float baseAngle = accuracyCone.BaseAngle;
            float spreadAngle = accuracyCone.CurrentAngle;

            // Calculate a random angle within the current accuracy spread
            float finalAngle = baseAngle + Random.Range(-spreadAngle, spreadAngle) * Mathf.Deg2Rad;

            // Convert angle to rotation
            Quaternion rotation = Quaternion.Euler(0, 0, finalAngle * Mathf.Rad2Deg);

            Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
            var bullet = Instantiate(projectilePrefab, spawnPos, rotation)
                .GetComponent<Projectile>();
            bullet.damage = damage;
        }
        else
        {
            Debug.LogWarning("Revolver: Missing Projectile Prefab or AccuracyCone script.");
        }
    }
}

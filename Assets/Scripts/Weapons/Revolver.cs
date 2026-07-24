using UnityEngine;
using UnityEngine.InputSystem;

public class Revolver : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    
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

    public void OnFire(InputAction.CallbackContext context)
    {
        // Forward the input to the AccuracyCone script so it can draw the accuracy cone
        if (accuracyCone != null)
        {
            accuracyCone.OnLeftClick(context);
        }

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
            Instantiate(projectilePrefab, spawnPos, rotation);
        }
        else
        {
            Debug.LogWarning("Revolver: Missing Projectile Prefab or AccuracyCone script.");
        }
    }
}

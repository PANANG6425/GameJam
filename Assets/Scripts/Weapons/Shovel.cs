using UnityEngine;
using UnityEngine.InputSystem;

public class Shovel : MonoBehaviour
{
    [SerializeField] private Collider2D attackCollider;
    [SerializeField] private LayerMask enemyLayers;

    private bool isAttacking = false;

    private void Start()
    {
        // Make sure the attack collider starts disabled
        if (attackCollider != null)
        {
            attackCollider.enabled = false;
        }
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            MeleeAttack();
        }
    }

    private void MeleeAttack()
    {
        if (!isAttacking && attackCollider != null)
        {
            isAttacking = true;
            attackCollider.enabled = true;

            // Disable the collider after a short window
            Invoke(nameof(EndAttack), 0.15f);

            Debug.Log("Shovel swung!");
        }
    }

    private void EndAttack()
    {
        attackCollider.enabled = false;
        isAttacking = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only register hits during an active attack and on the correct layers
        if (isAttacking && ((1 << other.gameObject.layer) & enemyLayers) != 0)
        {
            Debug.Log("Shovel hit: " + other.name);
        }
    }
}

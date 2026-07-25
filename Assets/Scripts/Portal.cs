using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Portal : MonoBehaviour
{
    [SerializeField]
    Portal connectingDoor;

    // True while the player is standing inside this portal's trigger.
    private bool playerInRange;
    Transform playerTranform;

    void Update()
    {
        // Only poll for the key while the player is in range. wasPressedThisFrame
        // reads the keyboard device directly, so no InputAction asset is needed.
        if (playerInRange && Keyboard.current != null && Keyboard.current.wKey.wasPressedThisFrame)
        {
            var pos = connectingDoor.transform.position;
            playerTranform.position = pos;
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            playerTranform = collider.gameObject.transform;
            playerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            playerTranform = null;
            playerInRange = false;
        }
    }
}

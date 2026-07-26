using UnityEngine;
using UnityEngine.InputSystem;

public class Portal : MonoBehaviour
{
    [SerializeField]
    Portal connectingDoor;

    [SerializeField]
    GameObject banner;

    // True while the player is standing inside this portal's trigger.
    private bool playerInRange;
    Transform playerTranform;

    [SerializeField]
    private bool isActive = false;

    void Start()
    {
        banner.SetActive(false);
    }

    void Update()
    {
        // Only poll for the key while the player is in range. wasPressedThisFrame
        // reads the keyboard device directly, so no InputAction asset is needed.
        if (
            playerInRange
            && isActive
            && Keyboard.current != null
            && Keyboard.current.wKey.wasPressedThisFrame
        )
        {
            var pos = connectingDoor.transform.position;
            playerTranform.position = pos;
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player") && isActive)
        {
            playerTranform = collider.gameObject.transform;
            playerInRange = true;
            banner.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            playerTranform = null;
            playerInRange = false;
            banner.SetActive(false);
        }
    }

    public void Open()
    {
        isActive = true;
        connectingDoor.Open();
    }

    public void Close()
    {
        isActive = false;
        connectingDoor.Close();
    }
}

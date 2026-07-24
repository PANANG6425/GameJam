using UnityEngine;

public class Area2D : MonoBehaviour
{
    public readonly Event<Collider2D> onEnter = new();
    public readonly Event<Collider2D> onStay = new();
    public readonly Event<Collider2D> onExit = new();

    void OnTriggerEnter2D(Collider2D collider)
    {
        onEnter.Invoke(collider);
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        onStay.Invoke(collider);
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        onExit.Invoke(collider);
    }
}

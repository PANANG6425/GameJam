using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 15f;
    public float lifeTime = 5f;
    bool markDestroy = false;

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
        if (!hitInfo.CompareTag("Player"))
        {
            if (hitInfo.CompareTag("Enemy") && !markDestroy)
            {
                markDestroy = true;
                var enemy = hitInfo.gameObject.GetComponent<Enemy>();
                enemy.Hit();
            }
            Destroy(gameObject);
        }
    }
}

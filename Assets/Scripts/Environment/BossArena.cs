using UnityEngine;

// วางเป็น trigger box (BoxCollider2D + Is Trigger) คลุมพื้นที่ห้องบอส
// player เข้ามา → ล็อกประตูทางเข้า (arena lock) + ปลุกบอส
// บอสตาย → เปิดประตูทางเข้า/ทางออก + ดรอปไอเทม
public class BossArena : MonoBehaviour
{
    [SerializeField] Door entryDoor;        // ประตูทางเข้า (ล็อกตอนสู้)
    [SerializeField] Door exitDoor;         // ประตูไปโซนถัดไป (เปิดเมื่อบอสตาย) - ปล่อยว่างได้
    [SerializeField] GameObject boss;       // ตัวบอส (ปิด SetActive(false) ไว้ก่อน)
    [SerializeField] HitPoint bossHp;        // HitPoint ของบอส (ไว้เช็คตาย)
    [SerializeField] GameObject dropPrefab;  // ItemPickup (หน้ากาก/เหงือก) ดรอปตอนบอสตาย - ปล่อยว่างได้
    [SerializeField] Transform dropPoint;

    bool started, cleared;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (started || cleared || !other.CompareTag("Player")) return;
        started = true;
        if (entryDoor != null) entryDoor.Close();   // ล็อกทางเข้า
        if (boss != null) boss.SetActive(true);      // ปลุกบอส
    }

    void Update()
    {
        if (started && !cleared && bossHp != null && bossHp.GetCurrentHP() <= 0)
        {
            cleared = true;
            if (entryDoor != null) entryDoor.Open();
            if (exitDoor != null) exitDoor.Open();
            if (dropPrefab != null)
                Instantiate(dropPrefab, dropPoint != null ? dropPoint.position : transform.position, Quaternion.identity);
        }
    }
}

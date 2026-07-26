using UnityEngine;

// มินิบอส = ศัตรูเดิม (Drowner/Bomber) ที่ปรับสถานะให้ "อึด + แรง" แล้ว "ดรอปไอเทมตอนตาย"
//
// วิธีทำมินิบอส:
//   1) สร้าง Prefab Variant จาก Enemy เดิม (คลิกขวา prefab → Create > Prefab Variant)
//   2) ปรับพลังที่คอมโพเนนต์เดิม:
//        - HitPoint (max_hp)  → เพิ่มเลือด (มินิบอส ~4-6 เท่าของมอนปกติ)
//        - Attack (damage)    → เพิ่มดาเมจ
//        - Transform (Scale)  → ขยายตัวให้ดูใหญ่ (เช่น 1.4-1.6)
//   3) แนบสคริปต์นี้ + ลากไอเทมที่จะดรอปใส่ช่อง Drop Prefabs
//
// สคริปต์นี้แค่หย่อนไอเทมตอน HP หมด — ไม่ยุ่งกับ Enemy.cs ของเพื่อน
// (ถ้าอยากได้ห้องล็อก arena ด้วย ใช้ BossArena.cs ที่มีอยู่แล้วครอบแทน)
[RequireComponent(typeof(HitPoint))]
public class MiniBoss : MonoBehaviour
{
    [Header("ไอเทมที่ดรอปตอนตาย")]
    [Tooltip("ลาก prefab ไอเทม เช่น เครื่องราง / กล่องกระสุน / ผ้าพันแผล")]
    [SerializeField] GameObject[] dropPrefabs;

    [SerializeField] Vector2 dropOffset = new Vector2(0f, 0.5f);
    [SerializeField] float spread = 0.6f;   // กระจายไอเทมออกจากกันเล็กน้อย

    HitPoint hp;
    bool dropped;

    void Awake()
    {
        hp = GetComponent<HitPoint>();
    }

    void Update()
    {
        if (dropped || hp == null) return;

        if (hp.GetCurrentHP() <= 0)
        {
            dropped = true;
            SpawnDrops();
        }
    }

    void SpawnDrops()
    {
        if (dropPrefabs == null || dropPrefabs.Length == 0) return;

        Vector3 basePos = transform.position + (Vector3)dropOffset;
        for (int i = 0; i < dropPrefabs.Length; i++)
        {
            if (dropPrefabs[i] == null) continue;
            float dx = (i - (dropPrefabs.Length - 1) * 0.5f) * spread;
            Vector3 pos = basePos + new Vector3(dx, 0f, 0f);
            Instantiate(dropPrefabs[i], pos, Quaternion.identity);
        }
    }
}

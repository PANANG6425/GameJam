using System.Collections;
using UnityEngine;

// TankBoss = บอสรถถัง (Enemy ที่อยู่กับที่ ไม่เดิน) — หย่อนระเบิดลงในพื้นที่เป้าหมายเป็นช่วง ๆ
//   1) dropArea (Collision Rect) กำหนดพื้นที่ที่จะหย่อนระเบิด (สัมพัทธ์กับตำแหน่งบอส)
//      ระเบิดจะสุ่มตำแหน่งตกภายในกรอบสี่เหลี่ยมนี้
//   2) bombPrefab = prefab ระเบิดที่จะหย่อน
//   3) damage = ดาเมจที่ส่งต่อให้ระเบิดแต่ละลูก
// รับเลือด/โดนตี/ตาย ใช้ระบบเดียวกับ Enemy (Hit/Die/สตัน/เบิร์น)
[RequireComponent(typeof(HitPoint))]
public class TankBoss : Enemy
{
    [Header("Bomb")]
    [Tooltip("prefab ระเบิดที่จะหย่อน")]
    [SerializeField]
    GameObject bombPrefab;

    [Tooltip("ดาเมจที่ระเบิดแต่ละลูกจะสร้าง")]
    [SerializeField]
    int damage = 3;

    [Header("Drop Area (Collision Rect - สัมพัทธ์กับตำแหน่งบอส)")]
    [Tooltip("กรอบสี่เหลี่ยมที่ระเบิดจะสุ่มตกลงไปข้างใน")]
    [SerializeField]
    Rect dropArea = new Rect(-3f, -1f, 6f, 2f);

    [Header("Timing")]
    [Tooltip("เวลา (วินาที) ระหว่างการหย่อนระเบิดแต่ละครั้ง")]
    [SerializeField]
    float dropInterval = 2f;
    
    [Tooltip("Trigger name for the attack animation")]
    [SerializeField]
    string attackAnimTrigger = "AnimAttack";
    
    [Tooltip("Delay in seconds from animation start to actually dropping bombs")]
    [SerializeField]
    float attackAnimDelay = 2f;

    [Tooltip("จำนวนระเบิดที่หย่อนต่อครั้ง")]
    [SerializeField]
    int bombsPerDrop = 1;

    float timer;
    bool isAttacking;

    protected override void Update()
    {
        // ให้ Enemy จัดการสตัน/เบิร์น/knockback ตามปกติ
        base.Update();

        // ตายแล้ว หรือกำลังโดนสตัน ก็หยุดหย่อนระเบิด
        if (isDead || IsStunned || isAttacking)
            return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        
        if (animator != null && !string.IsNullOrEmpty(attackAnimTrigger))
        {
            animator.SetTrigger(attackAnimTrigger);
        }
        
        yield return new WaitForSeconds(attackAnimDelay);
        
        // Ensure boss is still alive and not stunned before dropping
        if (!isDead && !IsStunned)
        {
            DropBombs();
        }
        
        timer = dropInterval;
        isAttacking = false;
    }

    void DropBombs()
    {
        if (bombPrefab == null)
            return;

        for (int i = 0; i < bombsPerDrop; i++)
            DropBomb();
    }

    void DropBomb()
    {
        // สุ่มตำแหน่งภายใน Collision Rect (แปลงเป็น world space จากตำแหน่งบอส)
        Vector2 origin = (Vector2)transform.position + dropArea.position;
        Vector2 pos = new Vector2(
            origin.x + Random.value * dropArea.width,
            origin.y + Random.value * dropArea.height
        );

        GameObject bomb = Instantiate(bombPrefab, pos, Quaternion.identity);

        // ส่งค่า damage ของบอสให้ระเบิด ถ้าระเบิดมีคอมโพเนนต์ที่ถือค่าดาเมจ
        var projectile = bomb.GetComponent<EnemyProjectile>();
        if (projectile != null)
            projectile.damage = damage;
    }

    // วาดกรอบพื้นที่หย่อนระเบิดในหน้า editor ให้เห็นตอนตั้งค่า
    void OnDrawGizmosSelected()
    {
        Vector3 center = transform.position + new Vector3(dropArea.center.x, dropArea.center.y, 0f);
        Vector3 size = new Vector3(dropArea.width, dropArea.height, 0f);

        Gizmos.color = new Color(1f, 0.4f, 0f, 0.25f);
        Gizmos.DrawCube(center, size);
        Gizmos.color = new Color(1f, 0.4f, 0f, 1f);
        Gizmos.DrawWireCube(center, size);
    }
}

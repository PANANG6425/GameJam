using UnityEngine;

// โซนบรรยากาศ eldritch: ยิ่งยืนอยู่นาน ยิ่ง corrupt เรื่อย ๆ
// หลัก = เพิ่ม Madness ต่อ tick ผ่าน GlobalEvent.IncreaseMadness (ต่อกับ Madness.cs)
// ตัวเลือก = ลด HP ต่อ tick ด้วย (corruptHpPerTick > 0)
// มี Totem = กันได้ (ไม่โดน)
public class CorruptionZone : MonoBehaviour
{
    [SerializeField] int madnessPerTick = 5;    // เพิ่ม Madness ต่อ tick
    [SerializeField] int corruptHpPerTick = 0;  // 0 = ไม่ลด HP · >0 = corrupt เลือดด้วย
    [SerializeField] float interval = 0.6f;
    [SerializeField] bool blockedByTotem = true;

    float timer;

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (blockedByTotem && other.GetComponentInParent<Totem>() != null) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            if (madnessPerTick > 0) GlobalEvent.IncreaseMadness.Invoke(madnessPerTick);
            if (corruptHpPerTick > 0)
            {
                var hp = other.GetComponentInParent<HitPoint>();
                if (hp != null) hp.DecreaseHP(corruptHpPerTick);
            }
            timer = interval;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) timer = 0f;
    }
}

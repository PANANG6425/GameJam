using UnityEngine;

// วางเป็น trigger box (BoxCollider2D + Is Trigger) คลุมโซนน้ำ
// - ลอยตัว (gravity ต่ำลง) + หน่วงตอนกระโดด/ตก
// - เดินช้าลงจริง ผ่าน PlayerController.ApplySlow() (เรียกต่อเนื่องขณะอยู่ในน้ำ)
// ถ้าอยากให้ "น้ำลึกต้องมีของ" → เพิ่ม HazardZone (Immunity) ทับโซนเดียวกัน
public class WaterZone : MonoBehaviour
{
    [SerializeField] float gravityInWater = 0.35f;   // ยิ่งน้อยยิ่งลอย
    [SerializeField] float dragInWater = 3f;          // หน่วงตอนตก/กระโดด
    [SerializeField] float moveSlowMultiplier = 0.5f; // เดินเหลือ 50% ในน้ำ

    float savedGravity = 1f;
    float savedDrag = 0f;

    void OnTriggerEnter2D(Collider2D other)
    {
        var rb = other.attachedRigidbody;
        if (!other.CompareTag("Player") || rb == null) return;

        savedGravity = rb.gravityScale;
        savedDrag = rb.linearDamping;
        rb.gravityScale = gravityInWater;
        rb.linearDamping = dragInWater;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // เรียกซ้ำทุกเฟรม → PlayerController รีเฟรช slow ตลอดที่อยู่ในน้ำ
        // พอออกจากน้ำ slow หมดเองใน ~0.2 วิ
        var pc = other.GetComponentInParent<PlayerController>();
        if (pc != null) pc.ApplySlow(moveSlowMultiplier, 0.2f);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        var rb = other.attachedRigidbody;
        if (!other.CompareTag("Player") || rb == null) return;

        rb.gravityScale = savedGravity <= 0f ? 1f : savedGravity;
        rb.linearDamping = savedDrag;
    }
}

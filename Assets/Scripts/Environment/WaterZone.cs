using UnityEngine;

// วางเป็น trigger box (BoxCollider2D + Is Trigger) คลุมโซนน้ำ
// ทำให้ player ลอยตัว (gravity ต่ำลง) + หน่วง (drag) ตอนอยู่ในน้ำ
// ถ้าอยากให้ "น้ำลึกต้องมีเหงือก" → เพิ่ม HazardZone (Immunity = Gill) ทับโซนเดียวกัน
public class WaterZone : MonoBehaviour
{
    [SerializeField] float gravityInWater = 0.35f;
    [SerializeField] float dragInWater = 3f;

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

    void OnTriggerExit2D(Collider2D other)
    {
        var rb = other.attachedRigidbody;
        if (!other.CompareTag("Player") || rb == null) return;

        rb.gravityScale = savedGravity <= 0f ? 1f : savedGravity;
        rb.linearDamping = savedDrag;
    }
}

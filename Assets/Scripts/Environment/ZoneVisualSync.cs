using UnityEngine;

// วางที่ตัว "ภาพ" (เช่น Gasvisual) ที่เป็นลูกของโซน (GasZone/CorruptZone/WaterZone)
// ปรับ "ตำแหน่ง + ขนาด" ของภาพให้คลุม BoxCollider2D ของโซนพอดีอัตโนมัติ
// คำนวณจากขนาดจริงของ sprite → ใช้ได้กับ sprite ทุกขนาด/ทุก PPU
// อัปเดตทั้งใน Editor และตอนเล่น
[ExecuteAlways]
[RequireComponent(typeof(SpriteRenderer))]
public class ZoneVisualSync : MonoBehaviour
{
    void Update()
    {
        var box = GetComponentInParent<BoxCollider2D>();
        var sr = GetComponent<SpriteRenderer>();
        if (box == null || sr == null || sr.sprite == null) return;

        Vector2 nativeSize = sr.sprite.bounds.size; // ขนาดจริงของ sprite (world unit) ที่ scale = 1
        if (nativeSize.x <= 0f || nativeSize.y <= 0f) return;

        transform.localPosition = box.offset;
        transform.localScale = new Vector3(
            box.size.x / nativeSize.x,
            box.size.y / nativeSize.y,
            1f
        );
    }
}

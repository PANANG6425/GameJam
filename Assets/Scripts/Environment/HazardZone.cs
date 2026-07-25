using UnityEngine;

// วางเป็น trigger box (BoxCollider2D + Is Trigger) คลุมโซนอันตราย เช่น แก๊สพิษ หรือ น้ำลึก
// ทำ damage-over-time ใส่ player ผ่าน HitPoint
// ถ้า player มี item ที่กำหนด (GasMask / Gill) = เดินผ่านได้ (ใช้เป็น Metroidvania gate)
public class HazardZone : MonoBehaviour
{
    public enum Immunity { None, GasMask, Gill }

    [SerializeField] int damage = 1;
    [SerializeField] float interval = 0.5f;
    [SerializeField] Immunity immunity = Immunity.GasMask; // item ที่ทำให้ผ่านได้

    float timer;

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (IsImmune(other)) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            var hp = other.GetComponentInParent<HitPoint>();
            if (hp != null) hp.DecreaseHP(damage);
            timer = interval;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) timer = 0f;
    }

    bool IsImmune(Collider2D other)
    {
        switch (immunity)
        {
            case Immunity.GasMask: return other.GetComponentInParent<GasMask>() != null;
            case Immunity.Gill:    return other.GetComponentInParent<Gill>() != null;
            default:               return false;
        }
    }
}

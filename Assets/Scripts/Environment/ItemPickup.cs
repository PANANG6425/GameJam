using UnityEngine;

// วางบนไอเทมในฉาก (ต้องมี Collider2D + ติ๊ก Is Trigger)
// player เดินชน → ได้ผลตามชนิด แล้วไอเทมหายไป
public class ItemPickup : MonoBehaviour
{
    public enum Kind { Bandage, AmmoBox, Charm, GasMask, Gill, Totem }

    [SerializeField] Kind kind = Kind.Bandage;
    [SerializeField] int amount = 4;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        var root = other.transform.root.gameObject;

        switch (kind)
        {
            case Kind.Bandage:
                var hp = root.GetComponentInChildren<HitPoint>();
                if (hp != null) hp.IncreaseHP(amount);
                break;

            case Kind.AmmoBox:
                // TODO: ต่อระบบกระสุน (WeaponManager / Revolver) เมื่อพร้อม
                break;

            case Kind.Charm:
                // TODO: ต่อระบบ Madness (ลดค่า) เมื่อ Madness.cs พร้อม
                break;

            case Kind.GasMask:
                if (root.GetComponent<GasMask>() == null) root.AddComponent<GasMask>();
                break;

            case Kind.Gill:
                if (root.GetComponent<Gill>() == null) root.AddComponent<Gill>();
                break;

            case Kind.Totem:
                if (root.GetComponent<Totem>() == null) root.AddComponent<Totem>();
                break;
        }

        Destroy(gameObject);
    }
}

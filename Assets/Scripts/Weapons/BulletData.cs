using UnityEngine;

// The six ammo types the revolver can fire. Normal has unlimited reserve ammo;
// every other type is a "special" capped by Revolver.specialAmmoCapCylinders.
public enum BulletType
{
    Normal,
    Incendiary, // Fire DoT
    HighExplosive, // AoE explosion
    Flesh, // Drain Madness (stubbed for now)
    Shock, // Short stun
    Gas, // Toxic gas cloud on the ground
}

// Per-type tuning + effect payload. Configure one entry per bullet type on the
// Revolver. The values here are read by the Projectile when a shot lands.
[System.Serializable]
public class BulletDefinition
{
    public BulletType type = BulletType.Normal;

    [Tooltip("Direct impact damage dealt to the enemy that is hit.")]
    public int impactDamage = 1;

    [Header("Incendiary (Fire DoT)")]
    public int burnDamagePerTick = 1;
    public float burnTickInterval = 0.5f;
    public float burnDuration = 3f;

    [Header("High Explosive (AoE)")]
    public float explosionRadius = 2.5f;
    public int explosionDamage = 3;

    [Tooltip("Optional VFX spawned at the point of impact.")]
    public GameObject explosionPrefab;

    [Header("Shock (Short Stun)")]
    public float stunDuration = 1.5f;

    [Header("Gas (Toxic ground cloud)")]
    [Tooltip("Gas cloud prefab spawned on the ground at the point of impact.")]
    public GameObject gasCloudPrefab;

    // Flesh (Drain Madness) is intentionally a stub for now - see Projectile.ApplyEffect.
}

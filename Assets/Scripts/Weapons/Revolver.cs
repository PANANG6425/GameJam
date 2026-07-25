using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Six-shot revolver. Driven by the WeaponManager while in Revolver mode:
//   Left-click  -> aim on press, fire on release
//   Right-click -> "scroll" the cylinder to the next bullet type
//
// Ammo model:
//   * The cylinder holds `cylinderCapacity` (6) rounds. Firing spends one round;
//     when the cylinder empties it auto-reloads after `reloadTime`.
//   * Bullet type is a selectable firing mode. Normal has unlimited reserve;
//     each special type's reserve is capped at `specialAmmoCapCylinders` cylinders
//     (10 cylinders = 60 rounds). Firing a special spends both a cylinder round
//     and one reserve round of that type.
public class Revolver : MonoBehaviour
{
    [SerializeField]
    private GameObject projectilePrefab;

    [SerializeField]
    private Transform firePoint;

    [SerializeField]
    int damage = 1;

    [Header("Aiming")]
    [SerializeField]
    float BaseAimAngle = 45;

    [SerializeField]
    float MinAimAngle = 10f;

    [SerializeField]
    float AimSpeed = 0.1f;

    [Header("Cylinder / Ammo")]
    [Tooltip("How many rounds the cylinder holds before an auto-reload.")]
    [SerializeField]
    int cylinderCapacity = 6;

    [Tooltip("Seconds the auto-reload takes once the cylinder is empty.")]
    [SerializeField]
    float reloadTime = 1f;

    [Tooltip("Delay between shots when double-click quick-firing (fanning the hammer).")]
    [SerializeField]
    float quickFireInterval = 0.06f;

    [Tooltip("Reserve capacity per special (non-Normal) type, in CYLINDERS. 1 cylinder = cylinderCapacity rounds.")]
    [SerializeField]
    int specialAmmoCapCylinders = 10;

    [Tooltip("Which enemy layers the fired projectiles / effects affect.")]
    [SerializeField]
    LayerMask enemyLayers;

    [Tooltip("One entry per bullet type. Configure damage & effect tuning here.")]
    [SerializeField]
    BulletDefinition[] bulletDefinitions;

    public bool IsAiming { get; private set; }
    public bool IsReloading { get; private set; }
    public BulletType SelectedType { get; private set; } = BulletType.Normal;
    public int RoundsLoaded { get; private set; }
    public int CylinderCapacity => cylinderCapacity;

    // Reserve capacity per special type, in cylinders and in rounds.
    public int SpecialAmmoCapCylinders => specialAmmoCapCylinders;
    public int SpecialAmmoCapRounds => specialAmmoCapCylinders * cylinderCapacity;

    // Order the cylinder scroll (right-click) cycles through.
    static readonly BulletType[] TypeOrder =
    {
        BulletType.Normal,
        BulletType.Incendiary,
        BulletType.HighExplosive,
        BulletType.Flesh,
        BulletType.Shock,
        BulletType.Gas,
    };

    // Reserve ammo for special types (Normal is unlimited).
    readonly Dictionary<BulletType, int> reserve = new();
    readonly Dictionary<BulletType, BulletDefinition> definitions = new();

    // UI hooks - the Gun Cylinder UI can subscribe to these.
    public event Action<BulletType> OnBulletTypeChanged;
    public event Action OnAmmoChanged;

    private AccuracyCone accuracyCone;
    private Animator animator;
    private bool isQuickFiring;

    private void Awake()
    {
        // Try to find the AccuracyCone script on this object or parent
        accuracyCone = GetComponentInParent<AccuracyCone>();
        if (accuracyCone == null)
        {
            accuracyCone = GetComponent<AccuracyCone>();
        }

        // Try to find the Animator
        animator = GetComponent<Animator>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (animator == null) animator = GetComponentInParent<Animator>();

        // Index the bullet definitions by type for quick lookup.
        if (bulletDefinitions != null)
        {
            foreach (var def in bulletDefinitions)
            {
                if (def != null)
                {
                    definitions[def.type] = def;
                }
            }
        }

        // Start every special type at its cap; Normal stays unlimited.
        foreach (var type in TypeOrder)
        {
            if (type != BulletType.Normal)
            {
                reserve[type] = SpecialAmmoCapRounds;
            }
        }

        RoundsLoaded = cylinderCapacity;
    }

    void Start()
    {
        if (accuracyCone == null)
        {
            Debug.LogError("There is no AccuracyCone Componenet");
            return;
        }
        accuracyCone.SetBaseAngle(BaseAimAngle);
        accuracyCone.SetAimSpeed(AimSpeed);
        accuracyCone.SetMinAimAngle(MinAimAngle);
    }

    // Left-click - forwarded by the WeaponManager only while in Revolver mode.
    public void OnFire(InputAction.CallbackContext context)
    {
        // Forward the input to the AccuracyCone script so it can draw the accuracy cone
        accuracyCone?.OnLeftClick(context);

        if (context.started)
        {
            // Aiming: the animator's Any State -> Anim_Revolver transition (driven
            // by the IsAiming bool) forces the aim pose, overriding walk/run so the
            // player visibly stops and raises the revolver.
            IsAiming = true;
            if (animator != null)
            {
                animator.SetBool("IsAiming", true);
            }
        }
        else if (context.canceled)
        {
            // Clear IsAiming first so the Any State transition releases, then play
            // the fired animation.
            IsAiming = false;
            if (animator != null)
            {
                animator.SetBool("IsAiming", false);
                animator.Play("Anim_Fired");
            }
            Shoot();
        }
    }

    // Right-click - "scroll" the cylinder to the next bullet type.
    public void CycleBulletType(int direction = 1)
    {
        int count = TypeOrder.Length;
        int index = Array.IndexOf(TypeOrder, SelectedType);
        index = ((index + direction) % count + count) % count;
        SelectedType = TypeOrder[index];
        OnBulletTypeChanged?.Invoke(SelectedType);
    }

    // Reserve ammo for a type (Normal reports int.MaxValue = unlimited).
    public int GetReserve(BulletType type)
    {
        if (type == BulletType.Normal)
        {
            return int.MaxValue;
        }
        return reserve.TryGetValue(type, out var n) ? n : 0;
    }

    // For future ammo pickups. Normal is unlimited so it's ignored.
    public void AddAmmo(BulletType type, int amount)
    {
        if (type == BulletType.Normal)
        {
            return;
        }
        int current = reserve.TryGetValue(type, out var n) ? n : 0;
        reserve[type] = Mathf.Clamp(current + amount, 0, SpecialAmmoCapRounds);
        OnAmmoChanged?.Invoke();
    }

    // Called by the WeaponManager when switching away from the revolver.
    public void CancelAim()
    {
        if (IsAiming)
        {
            IsAiming = false;
            if (animator != null)
            {
                animator.SetBool("IsAiming", false);
            }
        }
        accuracyCone?.Hide();
    }

    private void Shoot()
    {
        if (IsReloading || RoundsLoaded <= 0)
        {
            return;
        }

        // Special types require reserve ammo; Normal is unlimited.
        if (SelectedType != BulletType.Normal && GetReserve(SelectedType) <= 0)
        {
            // Dry click on an empty special - scroll to another type.
            return;
        }

        if (projectilePrefab == null || accuracyCone == null)
        {
            Debug.LogWarning("Revolver: Missing Projectile Prefab or AccuracyCone script.");
            return;
        }

        float baseAngle = accuracyCone.BaseAngle;
        float spreadAngle = accuracyCone.CurrentAngle;

        // Calculate a random angle within the current accuracy spread
        float finalAngle =
            baseAngle + UnityEngine.Random.Range(-spreadAngle, spreadAngle) * Mathf.Deg2Rad;

        // Convert angle to rotation
        Quaternion rotation = Quaternion.Euler(0, 0, finalAngle * Mathf.Rad2Deg);

        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
        var bullet = Instantiate(projectilePrefab, spawnPos, rotation).GetComponent<Projectile>();

        BulletDefinition def = definitions.TryGetValue(SelectedType, out var d) ? d : null;
        bullet.definition = def;
        bullet.enemyLayers = enemyLayers;
        bullet.damage = def != null ? def.impactDamage : damage;

        // Consume ammo.
        RoundsLoaded--;
        if (SelectedType != BulletType.Normal)
        {
            reserve[SelectedType] = Mathf.Max(0, reserve[SelectedType] - 1);
        }
        OnAmmoChanged?.Invoke();

        if (RoundsLoaded <= 0)
        {
            StartCoroutine(AutoReload());
        }
    }

    private IEnumerator AutoReload()
    {
        IsReloading = true;
        yield return new WaitForSeconds(reloadTime);
        RoundsLoaded = cylinderCapacity;
        IsReloading = false;
        OnAmmoChanged?.Invoke();
    }

    // Double-click "fan the hammer" - rapidly empty the remaining loaded rounds.
    public void QuickFire()
    {
        if (isQuickFiring || IsReloading || RoundsLoaded <= 0)
        {
            return;
        }
        StartCoroutine(QuickFireRoutine());
    }

    private IEnumerator QuickFireRoutine()
    {
        isQuickFiring = true;

        // Clear any raised aim so the burst isn't gated by the aim pose.
        if (IsAiming)
        {
            IsAiming = false;
            if (animator != null)
            {
                animator.SetBool("IsAiming", false);
            }
        }

        while (RoundsLoaded > 0 && !IsReloading)
        {
            int before = RoundsLoaded;
            Shoot();

            // Shoot() leaves the count unchanged on a dry click (empty special
            // reserve). Stop instead of spinning forever.
            if (RoundsLoaded == before)
            {
                break;
            }

            if (animator != null)
            {
                animator.Play("Anim_Fired");
            }

            yield return new WaitForSeconds(quickFireInterval);
        }

        isQuickFiring = false;
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

// Combat input router. The revolver is the main weapon (always active); melee is
// a secondary attack. There is no weapon-mode switching.
//
//   Left-click  -> aim / fire the revolver
//   Right-click -> quick burst (fan the hammer - dump remaining loaded rounds)
//   Q           -> switch ammo type
//   V           -> melee (stun + knockback)
[RequireComponent(typeof(Revolver))]
[RequireComponent(typeof(Shovel))]
public class WeaponManager : MonoBehaviour
{
    private Revolver revolver;
    private Shovel shovel;

    private void Awake()
    {
        revolver = GetComponentInChildren<Revolver>();
        shovel = GetComponentInChildren<Shovel>();
    }

    private void OnEnable()
    {
        GlobalEvent.PlayerHit.AddListener(OnPlayerHit);
    }

    private void OnDisable()
    {
        GlobalEvent.PlayerHit.RemoveListener(OnPlayerHit);
    }

    // The player took a hit - interrupt whatever weapon action is in progress.
    private void OnPlayerHit()
    {
        revolver?.Interrupt();
        shovel?.CancelAttack();
    }

    // Left-click ("Attack") - aim on press, fire on release.
    public void OnFire(InputAction.CallbackContext context)
    {
        revolver?.OnFire(context);
    }

    // Right-click ("QuickFire") - burst-fire all remaining loaded rounds.
    public void OnQuickBurst(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            revolver?.QuickFire();
        }
    }

    // Q ("SwitchAmmo") - cycle to the next bullet type.
    public void OnSwitchAmmo(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            revolver?.CycleBulletType();
        }
    }

    // V ("MeleeAttack") - melee strike that stuns and knocks the enemy back.
    public void OnMelee(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            shovel?.Melee();
        }
    }
}

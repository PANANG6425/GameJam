using UnityEngine;
using UnityEngine.EventSystems;
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
    private bool suppressFireUntilRelease;

    // IsPointerOverGameObject() reflects last frame's UI state when read from
    // within an input callback (the EventSystem hasn't processed this frame's
    // pointer event yet at that point), so it's cached once per frame here -
    // in LateUpdate, after EventSystem's own Update has run - and the cached
    // value is what OnFire reads.
    private bool pointerOverUI;

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
        // Ignore clicks that start on UI (e.g. buttons) so pressing UI doesn't
        // also trigger aiming/firing. Keep suppressing until release so a drag
        // off the UI element mid-press can't leak a fire.
        if (context.started)
        {
            suppressFireUntilRelease = pointerOverUI;
            if (suppressFireUntilRelease)
            {
                return;
            }
        }
        else if (suppressFireUntilRelease)
        {
            if (context.canceled)
            {
                suppressFireUntilRelease = false;
            }
            return;
        }

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

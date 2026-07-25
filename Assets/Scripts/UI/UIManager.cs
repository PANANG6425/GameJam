using UnityEngine;
using TMPro;
public class UIManager : MonoBehaviour
{
    public ProgressBar healthBar;
    public ProgressBar madnessBar;
    public TMP_Text ammoDisplay;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        GlobalEvent.HealthChange.AddListener(healthBar.OnValueChanged);
        GlobalEvent.MadnessChange.AddListener(madnessBar.OnValueChanged);
        GlobalEvent.AmmoChange.AddListener(UpdateAmmoDisplay);
    }

    void OnDestroy()
    {
        GlobalEvent.HealthChange.RemoveListener(healthBar.OnValueChanged);
        GlobalEvent.MadnessChange.RemoveListener(madnessBar.OnValueChanged);
        GlobalEvent.AmmoChange.RemoveListener(UpdateAmmoDisplay);
    }

    void UpdateAmmoDisplay(int currentAmmo,int maxAmmo)
    {
        ammoDisplay.text = "Ammo: " + currentAmmo.ToString() + " / " + maxAmmo.ToString();
    }

}

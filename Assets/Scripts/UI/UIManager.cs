using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public ProgressBar healthBar;
    public ProgressBar madnessBar;
    public TMP_Text ammoDisplay;
    public TMP_Text ammoTypeDsiplay;
    public Button skillButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        GlobalEvent.HealthChange.AddListener(healthBar.OnValueChanged);
        GlobalEvent.MadnessChange.AddListener(madnessBar.OnValueChanged);
        GlobalEvent.MadnessChange.AddListener(OnMadnessValueChanged);
        GlobalEvent.AmmoChange.AddListener(UpdateAmmoDisplay);
        GlobalEvent.AmmoTypeChange.AddListener(OnAmmoTypeChange);
    }

    void OnDestroy()
    {
        GlobalEvent.HealthChange.RemoveListener(healthBar.OnValueChanged);
        GlobalEvent.MadnessChange.RemoveListener(madnessBar.OnValueChanged);
        GlobalEvent.MadnessChange.RemoveListener(OnMadnessValueChanged);
        GlobalEvent.AmmoChange.RemoveListener(UpdateAmmoDisplay);
        GlobalEvent.AmmoTypeChange.RemoveListener(OnAmmoTypeChange);
    }

    void OnAmmoTypeChange(BulletType type)
    {
        ammoTypeDsiplay.text = "Bullet type: " + type.ToString();
    }

    void OnMadnessValueChanged(int currentMadness, int maxMadness)
    {
        if (currentMadness >= maxMadness)
        {
            skillButton.interactable = true;
        }
        else
        {
            skillButton.interactable = false;
        }
    }

    void UpdateAmmoDisplay(int currentAmmo, int maxAmmo)
    {
        ammoDisplay.text = "Ammo: " + currentAmmo.ToString() + " / " + maxAmmo.ToString();
    }
}

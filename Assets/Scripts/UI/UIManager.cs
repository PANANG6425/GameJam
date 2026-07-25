using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public ProgressBar healthBar;
    public ProgressBar madnessBar;
    public TMP_Text ammoDisplay;
    public Button skillButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        GlobalEvent.HealthChange.AddListener(healthBar.OnValueChanged);
        GlobalEvent.MadnessChange.AddListener(madnessBar.OnValueChanged);
        GlobalEvent.MadnessChange.AddListener(OnMadnessValueChanged);
        GlobalEvent.AmmoChange.AddListener(UpdateAmmoDisplay);
    }

    void OnDestroy()
    {
        GlobalEvent.HealthChange.RemoveListener(healthBar.OnValueChanged);
        GlobalEvent.MadnessChange.RemoveListener(madnessBar.OnValueChanged);
        GlobalEvent.MadnessChange.RemoveListener(OnMadnessValueChanged);
        GlobalEvent.AmmoChange.RemoveListener(UpdateAmmoDisplay);
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

    void UpdateAmmoDisplay(int currentAmmo,int maxAmmo)
    {
        ammoDisplay.text = "Ammo: " + currentAmmo.ToString() + " / " + maxAmmo.ToString();
    }

}

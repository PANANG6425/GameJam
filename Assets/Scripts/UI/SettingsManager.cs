using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

// ใส่ที่ Canvas ของซีน MainMenu (คู่กับ MainMenuManager)
// จัดการ: Volume (เสียงรวม), Fullscreen, Resolution
// ต่อ reference ใน Inspector (ปล่อยว่างช่องไหนก็ได้ ถ้าไม่ใช้)
public class SettingsManager : MonoBehaviour
{
    [Header("Volume")]
    [SerializeField] Slider volumeSlider;

    [Header("Display")]
    [SerializeField] Toggle fullscreenToggle;
    [SerializeField] TMP_Dropdown resolutionDropdown;

    Resolution[] resolutions;

    void Start()
    {
        // --- Volume ---
        if (volumeSlider != null)
        {
            float v = PlayerPrefs.GetFloat("MasterVolume", 1f);
            volumeSlider.value = v;
            AudioListener.volume = v;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        // --- Fullscreen ---
        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = Screen.fullScreen;
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        }

        // --- Resolution ---
        if (resolutionDropdown != null)
        {
            SetupResolutions();
        }
    }

    public void SetVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    public void SetFullscreen(bool isFull)
    {
        Screen.fullScreen = isFull;
    }

    void SetupResolutions()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        var options = new List<string>();
        int current = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            var r = resolutions[i];
            options.Add(r.width + " x " + r.height);
            if (r.width == Screen.width && r.height == Screen.height) current = i;
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = current;
        resolutionDropdown.RefreshShownValue();
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    public void SetResolution(int index)
    {
        if (resolutions == null || index < 0 || index >= resolutions.Length) return;
        var r = resolutions[index];
        Screen.SetResolution(r.width, r.height, Screen.fullScreen);
    }
}

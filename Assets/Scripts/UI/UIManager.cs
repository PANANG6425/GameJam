using UnityEngine;

public class UIManager : MonoBehaviour
{
    public ProgressBar healthBar;
    public ProgressBar madnessBar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GlobalEvent.HealthChange.AddListener(healthBar.OnValueChanged);
        GlobalEvent.MadnessChange.AddListener(madnessBar.OnValueChanged);
    }
}

using UnityEngine;

public class GlobalEvent : MonoBehaviour
{
    public static GlobalEvent Instance { get; private set; }

    public static readonly Event<int> IncreaseMadness = new();
    public static readonly Event ResetMadness = new();
    public static readonly Event<int> IncreaseHealth = new();
    public static readonly Event<int, int> HealthChange = new();
    public static readonly Event<int, int> MadnessChange = new();

    public static readonly Event<int, int> AmmoChange = new();
    public static readonly Event PlayerHit = new();
    public int curPlayerMaxHP;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private bool isHitStopping = false;

    public void TriggerHitStop(float duration = 5f)
    {
        if (isHitStopping)
            return;
        StartCoroutine(HitStopRoutine(duration));
    }

    private System.Collections.IEnumerator HitStopRoutine(float duration)
    {
        isHitStopping = true;
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
        isHitStopping = false;
    }

    public void OnHealthChange(int currentHP, int maxHP)
    {
        curPlayerMaxHP = maxHP;
        Debug.Log("CurrentHP: " + currentHP + ", MaxHP: " + maxHP);
    }

    public void Heal()
    {
        IncreaseHealth.Invoke((int)(curPlayerMaxHP * 0.3));
    }

    public void OnMadnessSkillUse()
    {
        ResetMadness.Invoke();
    }
}

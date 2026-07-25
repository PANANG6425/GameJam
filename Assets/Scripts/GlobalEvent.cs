using System;
using UnityEngine;

public class GlobalEvent : MonoBehaviour
{
    public static GlobalEvent Instance { get; private set; }

    public static readonly Event<int> IncreaseMadness = new();
    public static readonly Event<int> IncreaseHealth = new();
    public static readonly Event<int, int> HealthChange = new();
    public static readonly Event<int, int> MadnessChange = new();

    public static readonly Event<int, int> AmmoChange = new();

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
        HealthChange.AddListener(OnHealthChange);
    }

    void OnDestroy()
    {
        HealthChange.RemoveListener(OnHealthChange);
    }

    public void OnHealthChange(int currentHP, int maxHP)
    {
        curPlayerMaxHP = maxHP;
        Debug.Log("CurrentHP: " + currentHP + ", MaxHP: " + maxHP);
    }

    public void Heal()
    {
        GlobalEvent.IncreaseHealth.Invoke((int)(curPlayerMaxHP * 0.3));
    }
}

using System;
using UnityEngine;

public class GlobalEvent : MonoBehaviour
{
    public static GlobalEvent Instance { get; private set; }

    public static readonly Event<int> IncreaseMadness = new();
    public static readonly Event<int, int> HealthChange = new();
    public static readonly Event<int, int> MadnessChange = new();

    public static readonly Event<int, int> AmmoChange = new();

    // Fired when the player takes a hit - lets the player cancel/interrupt whatever
    // action it was doing (aiming, quick-firing, melee).
    public static readonly Event PlayerHit = new();

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update() { }

    private bool isHitStopping = false;

    public void TriggerHitStop(float duration = 5f)
    {
        if (isHitStopping) return;
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
}

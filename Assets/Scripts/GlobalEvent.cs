using System;
using UnityEngine;

public class GlobalEvent : MonoBehaviour
{
    public static GlobalEvent Instance { get; private set; }

    public static readonly Event<int> IncreaseMadness = new();
    public static readonly Event<int, int> HealthChange = new();
    public static readonly Event<int, int> MadnessChange = new();

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
}

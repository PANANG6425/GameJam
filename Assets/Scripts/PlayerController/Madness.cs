using UnityEngine;

public class Madness : MonoBehaviour
{
    [SerializeField]
    int maxMadness = 100;
    int currentMadness = 0;

    public static float CurrentPercentage { get; private set; }

    void Start()
    {
        GlobalEvent.IncreaseMadness.AddListener(Increase);
        GlobalEvent.MadnessChange.Invoke(currentMadness, maxMadness);
        GlobalEvent.ResetMadness.AddListener(Reset);
    }

    public void OnDestroy()
    {
        GlobalEvent.IncreaseMadness.RemoveListener(Increase);
        GlobalEvent.ResetMadness.RemoveListener(Reset);
    }

    public void Increase(int amount)
    {
        currentMadness = Mathf.Clamp(currentMadness + amount, 0, maxMadness);
        CurrentPercentage = (float)currentMadness / maxMadness;
        GlobalEvent.MadnessChange.Invoke(currentMadness, maxMadness);

        if (currentMadness >= maxMadness)
        {
            Reset();
        }
    }

    public void Reset()
    {
        currentMadness = 0;
        CurrentPercentage = 0f;
        GlobalEvent.MadnessChange.Invoke(currentMadness, maxMadness);
    }
}

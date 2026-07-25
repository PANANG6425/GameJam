using UnityEngine;

public class Madness : MonoBehaviour
{
    [SerializeField]
    int maxMadness = 100;
    int currentMadness = 0;

    void Start()
    {
        GlobalEvent.IncreaseMadness.AddListener(Increase);
        GlobalEvent.MadnessChange.Invoke(currentMadness, maxMadness);
    }

    public void OnDestroy()
    {
        GlobalEvent.IncreaseMadness.RemoveListener(Increase);
    }

    public void Increase(int amount)
    {
        currentMadness += (currentMadness < maxMadness) ? amount : 0;
        GlobalEvent.MadnessChange.Invoke(currentMadness, maxMadness);
    }

    public void Reset()
    {
        currentMadness = 0;
        GlobalEvent.MadnessChange.Invoke(currentMadness, maxMadness);
    }
}

using UnityEngine;
using System.Collections;

public class MadnessDrainer : MonoBehaviour
{
    public void StartDrain(int totalAmount, float duration)
    {
        StartCoroutine(DrainRoutine(totalAmount, duration));
    }

    private IEnumerator DrainRoutine(int totalAmount, float duration)
    {
        // We will drain in ticks to make it look smooth
        int ticks = 4; 
        float tickInterval = duration / ticks;
        int amountPerTick = totalAmount / ticks;

        for (int i = 0; i < ticks; i++)
        {
            yield return new WaitForSeconds(tickInterval);
            GlobalEvent.IncreaseMadness.Invoke(amountPerTick);
        }

        // Add any remaining remainder just to be exact
        int remainder = totalAmount - (amountPerTick * ticks);
        if (remainder != 0)
        {
            GlobalEvent.IncreaseMadness.Invoke(remainder);
        }

        Destroy(gameObject);
    }
}

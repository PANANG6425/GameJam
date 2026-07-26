using UnityEngine;
using UnityEngine.Events;

public class HitPoint : MonoBehaviour
{
    [SerializeField]
    int max_hp = 10;
    int currentHP = 0;

    public UnityEvent onDamageTaken;

    public int MaxHP
    {
        get { return max_hp; }
    }
    public int CurrentHP
    {
        get { return currentHP; }
    }

    void Awake()
    {
        currentHP = max_hp;
    }

    public int GetMaxHP()
    {
        return max_hp;
    }

    public int GetCurrentHP()
    {
        return currentHP;
    }

    public void IncreaseHP(int amount)
    {
        currentHP += amount;
        currentHP = (currentHP > max_hp) ? max_hp : currentHP;
    }

    public void DecreaseHP(int amount)
    {
        if (amount <= 0)
            return;
        currentHP -= amount;
        currentHP = (currentHP < 0) ? 0 : currentHP;
        Debug.Log("CurrentHP:" + currentHP);
        onDamageTaken?.Invoke();
    }
}

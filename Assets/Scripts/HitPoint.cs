using UnityEngine;

public class HitPoint : MonoBehaviour
{
    [SerializeField]
    int max_hp = 10;
    int currentHP;

    void Start()
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
    }

    public void DecreaseHP(int amount)
    {
        currentHP -= amount;
    }
}

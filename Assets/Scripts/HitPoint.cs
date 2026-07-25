using UnityEngine;

public class HitPoint : MonoBehaviour
{
    [SerializeField]
    int max_hp = 10;
    int currentHP;

    public int MaxHP { get { return max_hp; } }
    public int CurrentHP { get { return currentHP; } }

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
        currentHP = (currentHP > max_hp) ? max_hp : currentHP;
    }

    public void DecreaseHP(int amount)
    {
        currentHP -= amount;
        currentHP = (currentHP < 0) ? 0 : currentHP;
        Debug.Log("CurrentHP:" + currentHP);
    }
}

using UnityEngine;

public class GlobalData : MonoBehaviour
{
    static GameObject player;
    public static GlobalData Instance { get; private set; }
    public const int MADNESS_HIT = 10;
    public const int MADNESS_ATK = 5;

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
    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    public static GameObject GetPlayerObj()
    {
        return player;
    }
}

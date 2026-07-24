using UnityEngine;

public class GlobalData : MonoBehaviour
{
    static GameObject player;

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

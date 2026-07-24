using System;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    GameObject enemyPrefab;

    [SerializeField]
    float SpawnFreq = 1f;

    float curTime = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (enemyPrefab == null)
        {
            throw new Exception("didn't set enemyPrefab for" + gameObject.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        curTime += Time.deltaTime;
        if (curTime > SpawnFreq)
        {
            curTime = 0;
            var enemy = Instantiate(enemyPrefab);
            enemy.transform.parent = transform;
            enemy.transform.position = transform.position;
        }
    }
}

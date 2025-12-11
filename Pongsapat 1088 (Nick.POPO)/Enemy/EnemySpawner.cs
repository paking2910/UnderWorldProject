using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject swarmerPrefab;
    [SerializeField] private GameObject bigSwarmerPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private float swarmerInterval = 3.5f;
    [SerializeField] private float bigSwarmerInterval = 10f;

    [Header("Limit Settings")]
    [SerializeField] private int swarmerLimit = 20;    
    [SerializeField] private int bigSwarmerLimit = 5;  

    [Header("Spawn Area")]
    [SerializeField] private float xRange = 5f; 
    [SerializeField] private float yRange = 6f; 

    void Start()
    {
        StartCoroutine(spawnEnemy(swarmerInterval, swarmerPrefab, swarmerLimit));
        StartCoroutine(spawnEnemy(bigSwarmerInterval, bigSwarmerPrefab, bigSwarmerLimit));
    }

    private IEnumerator spawnEnemy(float interval, GameObject enemyPrefab, int limit)
    {
        yield return new WaitForSeconds(interval);

        if (enemyPrefab == null)
        {
            Debug.LogError("Prefab is missing!");
            yield break;
        }

        GameObject[] existingEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (existingEnemies.Length < limit)
        {
            Vector3 randomPos = new Vector3(Random.Range(-xRange, xRange), Random.Range(-yRange, yRange), 0);
            Vector3 spawnPoint = transform.position + randomPos;

            Instantiate(enemyPrefab, spawnPoint, Quaternion.identity);
        }

        StartCoroutine(spawnEnemy(interval, enemyPrefab, limit));
    }
}
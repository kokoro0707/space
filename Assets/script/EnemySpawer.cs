using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;

    public float spawnInterval = 2f;
    float timer = 0;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0;
        }
    }

    void SpawnEnemy()
    {
        Transform p = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(enemyPrefab, p.position, p.rotation);
    }
}
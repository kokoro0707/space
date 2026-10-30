
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("敵のPrefab")]
    public GameObject enemyPrefab;

    [Header("出現位置")]
    public Transform[] spawnPoints;

    [Header("プレイヤー")]
    public Transform player;

    [Header("出現設定")]
    public float spawnInterval = 2f;

    [Min(1)]
    public int totalEnemies = 10;

    private float timer;
    private int spawnedCount;

    void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ConfigureMission(totalEnemies);
        }

        if (player == null)
        {
            Debug.LogError("SpawnerにPlayerが未設定です");
        }
    }

    void Update()
    {
        if (GameManager.Instance != null &&
            GameManager.Instance.IsFinished)
            return;

        if (spawnedCount >= totalEnemies)
            return;

        if (enemyPrefab == null ||
            spawnPoints == null ||
            spawnPoints.Length == 0)
            return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        Transform spawnPoint =
            spawnPoints[Random.Range(0, spawnPoints.Length)];

        if (spawnPoint == null) return;

        GameObject enemyObject = Instantiate(
            enemyPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        EnemyAI enemyAI =
            enemyObject.GetComponent<EnemyAI>();

        if (enemyAI != null)
        {
            enemyAI.SetPlayer(player);
        }

        spawnedCount++;
    }
}

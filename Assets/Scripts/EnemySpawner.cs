using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("Spawn Distance")]
    [SerializeField] private float minSpawnRadius = 10f;
    [SerializeField] private float maxSpawnRadius = 25f;

    [Header("Validation")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float raycastStartHeight = 100f;
    [SerializeField] private float spawnCheckRadius = 1f;

    [Header("Attempts")]
    [SerializeField] private int maxSpawnAttempts = 20;
    [SerializeField] private float cooldown = 2f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= cooldown)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    public void SpawnEnemy()
    {

        GameObject prefab =
        enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        GameObject enemy =
        Instantiate(prefab, transform.position, Quaternion.identity);
    }
}

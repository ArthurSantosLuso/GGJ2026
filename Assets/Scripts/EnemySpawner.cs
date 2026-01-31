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
        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            Vector3 rayStart = GetRandomRayStart();

            if (TryGetGround(rayStart, out Vector3 groundPoint))
            {
                GameObject prefab =
                enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

                GameObject enemy =
                Instantiate(prefab, groundPoint, Quaternion.identity);

                AlignEnemyToGround(enemy, groundPoint);
                return;
            }
        }

        Debug.LogWarning("Spawner: No valid spawn position found.");
    }

    private Vector3 GetRandomRayStart()
    {
        Vector2 circle =
        Random.insideUnitCircle.normalized *
        Random.Range(minSpawnRadius, maxSpawnRadius);

        return new Vector3(
        player.position.x + circle.x,
        raycastStartHeight,
        player.position.z + circle.y
        );
    }

    private bool TryGetGround(Vector3 rayStart, out Vector3 groundPoint)
    {
        groundPoint = Vector3.zero;

        if (!Physics.Raycast(
        rayStart,
        Vector3.down,
        out RaycastHit hit,
        raycastStartHeight * 2f,
        groundLayer))
        {
            return false;
        }

        groundPoint = hit.point;

        if (Physics.CheckSphere(
        groundPoint + Vector3.up * 0.1f,
        spawnCheckRadius,
        obstacleLayer))
        {
            return false;
        }

        return true;
    }

    private void AlignEnemyToGround(GameObject enemy, Vector3 groundPoint)
    {
        Collider col = enemy.GetComponentInChildren<Collider>();
        if (col == null) return;

        float bottomOffset = col.bounds.min.y;
        enemy.transform.position -= Vector3.up * bottomOffset;
    }
}

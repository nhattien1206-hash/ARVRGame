using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public PlayerManager player;
    public Transform imageTarget;   // ✅ Tham chiếu tới ImageTarget
    public int enemyCount = 1;
    public float spawnRadius = 2f;

    void Start()
    {
        if (player == null) player = FindObjectOfType<PlayerManager>();
        if (player != null && imageTarget != null) SpawnEnemies();
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            // Tạo vị trí ngẫu nhiên quanh ImageTarget
            Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
            randomOffset.y = 0; // giữ Enemy trên mặt phẳng

            // ✅ Spawn tại ImageTarget
            Vector3 spawnPos = imageTarget.position + randomOffset;

            // Đặt Enemy cùng độ cao với ImageTarget
            spawnPos.y = imageTarget.position.y;

            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

            EnemyManager enemyManager = enemy.GetComponent<EnemyManager>();
            if (enemyManager != null)
            {
                enemyManager.player = player.gameObject;
            }
        }
    }
}

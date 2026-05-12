using UnityEngine;
using Vuforia;

public class GameManager : MonoBehaviour
{
    [Header("Cấu hình cho game")]
    public GameObject characterPrefab;
    public GameObject environmentPrefab;
    public GameObject imageTarget;
    public GameObject enemyPrefab;
    public int numberOfEnemies;

    private GameObject characterInstance;
    private GameObject[] enemies;
    private GameObject environmentInstance;

    public void OnTargetFound()
    {
        Debug.Log("Target found!");
        if (characterInstance != null) return;

        // Tạo môi trường
        environmentInstance = Instantiate(
            environmentPrefab,
            imageTarget.transform.position,
            Quaternion.identity,
            imageTarget.transform);

        // Sau 3s tạo nhân vật
        Invoke(nameof(SpawnCharacter), 3f);

        // Sau 5s tạo quái
        Invoke(nameof(SpawnChallenges), 5f);
    }

    public void OnTargetLost()
    {
        Debug.Log("Target lost!");
        if (characterInstance != null)
        {
            Destroy(characterInstance);
            characterInstance = null;
        }

        if (enemies != null)
        {
            foreach (var enemy in enemies)
            {
                if (enemy != null) Destroy(enemy);
            }
            enemies = null;
        }

        if (environmentInstance != null)
        {
            Destroy(environmentInstance);
            environmentInstance = null;
        }
    }

    void SpawnCharacter()
    {
        characterInstance = Instantiate(
            characterPrefab,
            imageTarget.transform.position + new Vector3(0, 0, 0.5f),
            Quaternion.identity,
            imageTarget.transform);

        // Đảm bảo PlayerManager hoạt động
        var pm = characterInstance.GetComponent<PlayerManager>();
        if (pm != null)
        {
            pm.enabled = true;
        }
    }

    void SpawnChallenges()
    {
        enemies = new GameObject[numberOfEnemies];
        for (int i = 0; i < numberOfEnemies; i++)
        {
            var enemy = Instantiate(
                enemyPrefab,
                imageTarget.transform.position +
                new Vector3(Random.Range(-5f, 5f), 0, Random.Range(1f, 8f)),
                Quaternion.identity,
                imageTarget.transform);
            enemies[i] = enemy;
        }
    }
}

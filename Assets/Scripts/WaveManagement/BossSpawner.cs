using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UIElements;

public class BossSpawner : MonoBehaviour
{
    public GameObject[] bosses;
    public float spawnRadius;
    public static float healthMultiplier = 1f;
    public static float spawnTimer = 5f;
    private float lastSpawn;
    public static List<GameObject> currentEnemies = new List<GameObject>();
    public int currentWaveNumber;
    private VisualElement document;
    private VisualElement container;
    public int bossHealth = 2000;
    public int bossMaxHealth = 2000;
    public GameObject bossHealthBar;
    public GameObject bigBoss;

    private List<GameObject> shuffledBosses;
    private int currentBossIndex = 0;

    void Awake()
    {
        lastSpawn = spawnTimer;
        shuffledBosses = new List<GameObject>(bosses);
        ShuffleBosses();
    }

    void ShuffleBosses()
    {
        // Fisher-Yates shuffle algorithm
        for (int i = shuffledBosses.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            GameObject temp = shuffledBosses[i];
            shuffledBosses[i] = shuffledBosses[j];
            shuffledBosses[j] = temp;
        }
    }

    public void SpawnBoss()
    {
        GameObject enemyChoice;

        if (GameSettings.waveNumber % 25 == 0)
        {
            enemyChoice = bigBoss;
        }
        else
        {
            if (currentBossIndex >= shuffledBosses.Count)
            {
                // Shuffle the bosses again if all bosses have been spawned
                ShuffleBosses();
                currentBossIndex = 0;
            }

            enemyChoice = shuffledBosses[currentBossIndex];
            currentBossIndex++;
        }

        if (enemyChoice != null)
        {
            Vector3 spawnPosition = transform.position;
            Quaternion spawnRotation = Quaternion.identity;
            GameObject bossInstance = Instantiate(enemyChoice, spawnPosition, spawnRotation);
            currentEnemies.Add(bossInstance);
            Debug.Log("SpawnBoss called. Boss spawned at: " + spawnPosition);
            bossInstance.GetComponent<EnemyBase>().Health = bossHealth;
            document = bossHealthBar.GetComponent<UIDocument>().rootVisualElement;
            container = document.Q<VisualElement>("BossHealthContainer");
            container.visible = true;
            BossHealth.Instance.boss = bossInstance.GetComponent<EnemyBase>();
            BossHealth.Instance.BossMaxHealth = bossHealth;
        }
        else
        {
            Debug.LogError("enemyBossPrefab is not assigned!");
        }
        bossHealth += 2000;
    }
}

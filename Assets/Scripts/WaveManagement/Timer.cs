using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Timer : MonoBehaviour
{
    [SerializeField]
    private ItemPanel itemPanel;

    [SerializeField]
    private GameObject HUD;
    private Label waveNumberText;
    private Label timerText;
    [SerializeField] private BossSpawner bossSpawner;
    [SerializeField] private TargetIndicator targetIndicator; // Add this line

    public float waveLength;
    public float currentTime;
    public int waveNumber;
    bool geninventory = false;
    public static Timer Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        VisualElement document = HUD.GetComponent<UIDocument>().rootVisualElement;

        waveNumberText = document.Q("WaveNumber") as Label;
        timerText = document.Q("Timer") as Label;
    }

    void Start()
    {
        GameSettings.waveNumber = waveNumber;
        currentTime = waveLength;
        waveNumberText.text = "WAVE: " + waveNumber.ToString();
        GameSettings.waveNumber = waveNumber;
    }

    void Update()
    {
        if (GameSettings.gameState == GameState.EndGame || GameSettings.gameState == GameState.BossVictory)
        {
            return;
        }
        setTimerText();

        if (GameSettings.gameState == GameState.InGame && GameSettings.waveNumber % 5 != 0 || TerminalBehaviour.Instance.stopBoss)
        {
            currentTime -= Time.deltaTime;
        }
        else if (GameSettings.gameState == GameState.ItemSelect)
        {
            if (itemPanel.itemChosen)
            {
                NextWave();
            }
        }

        if (currentTime <= 0 || (waveNumber % 5 == 0 && BossHealthBar.Instance.boss == null))
        {
            if (waveNumber == 25 && GameSettings.gameState == GameState.InGame && GameSettings.gameMode == GameMode.Boss)
            {
                GameManager.Instance.BossVictory();
            }
            else
            {
                EndWave();
            }
        }

    }

    public void EndWave()
    {
        GameSettings.gameState = GameState.ItemSelect;
        CullBullets();
        if (waveNumber % 5 == 4)
        {
            CullEnemies();
        }
        if (!geninventory)
        {
            itemPanel.InitializeItemPanel(waveNumber);
            geninventory = true;
        }
    }

    private void setTimerText()
    {
        timerText.text = currentTime.ToString("0") + " s";
        if (currentTime <= 5)
        {
            timerText.style.color = new StyleColor(new Color32(182,39,38,255));
            float fontSize = timerText.resolvedStyle.fontSize;
            if (fontSize < 70f)
            {
                timerText.style.fontSize = fontSize + 1f;
            }
        }
        else
        {
            timerText.style.color = Color.white;
            timerText.style.fontSize = 50;
        }
        if (currentTime <= 0 || waveNumber % 5 == 0)
        {
            timerText.visible = false;
        }
        else
        {
            timerText.visible = true;
        }
    }

    private void NextWave()
    {
        GameSettings.gameState = GameState.InGame;
        waveNumber += 1;
        GameSettings.waveNumber = waveNumber;
        currentTime = waveLength;
        waveNumberText.text = "WAVE: " + waveNumber.ToString();

        if (waveNumber % 5 == 0 && !TerminalBehaviour.Instance.stopBoss)
        {
            bossSpawner.SpawnBoss();
            timerText.visible = false;

            // Activate or deactivate TargetIndicator based on the wave
            if (targetIndicator != null)
            {
                targetIndicator.ActivateIndicator(); // Make sure to have a method for this
            }
        }
        else
        {
            timerText.visible = true;

            // Deactivate TargetIndicator if it's not a boss wave
            if (targetIndicator != null)
            {
                targetIndicator.DeactivateIndicator(); // Make sure to have a method for this
            }
        }

        geninventory = false;
        itemPanel.itemChosen = false;

        // Enemy scaling
        if (waveNumber % 5 == 0)
        {
            EnemySpawner.Instance.EnemyLevel++;
        }

        if (EnemySpawner.Instance.SpawnTimer > 0.1f)
        {
            EnemySpawner.Instance.SpawnTimer -= 0.1f;
        }
        else
        {
            EnemySpawner.Instance.EnemyCap += 2;
        }

        // Scale stats if after wave 25
        if (waveNumber > 25)
        {
            EnemyBase.endlessScalar += 0.1f;
        }
    }

    public static void CullEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            EnemySpawner.Instance.currentEnemies.Remove(enemy);
            Destroy(enemy);
        }
    }

    public static void CullBullets()
    {
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject bullet in bullets)
        {
            Destroy(bullet);
        }
    }
}

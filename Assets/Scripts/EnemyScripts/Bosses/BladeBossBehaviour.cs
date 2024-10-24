using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlasticPipe.Server.MonitorStats;

public class BladeBossBehaviour : EnemyBase
{
    public GameObject player;
    private bool stopCheck;
    private float distance;

    [SerializeField] private Transform bulletPosition;
    [SerializeField] private GameObject bladePrefab;
    [SerializeField] private Transform shotPoint;
    [SerializeField] private GameObject bladeCenterPrefab;
    [SerializeField] private int numberOfBlades = 30;
    [SerializeField] private float radius = 1f;
    [SerializeField] private float spacing = 0.5f;

    private bool isCharging = false;
    private float chargeDuration = 1f;
    private float chargeTimer = 0f;
    private float chargeCooldown;
    private float chargeCooldownMin = 1f;
    private float chargeCooldownMax = 2f;
    private Vector2 targetPosition;
    private bool attacking = false;
    private GameObject attack;

    private void Awake()
    {
        ScaleStats();
        mapManager = FindObjectOfType<MapManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        chargeCooldown = Random.Range(chargeCooldownMin, chargeCooldownMax);
        attack = gameObject.transform.GetChild(0).GetChild(0).gameObject;
        SpawnBlades();
    }


    private void SpawnBlades()
    {
        float angleIncrement = 360f / numberOfBlades;
        float adjustedRadius = radius + (spacing * 5.5f);
        Vector2 centerPosition = Vector2.zero;

        for (int i = 0; i < numberOfBlades; i++)
        {
            float angle = i * angleIncrement * Mathf.Deg2Rad;
            Vector2 bladePosition = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * adjustedRadius + (Vector2)player.transform.position;

            Instantiate(bladePrefab, bladePosition, Quaternion.identity);
            centerPosition += bladePosition;
        }

        centerPosition /= numberOfBlades;
        Instantiate(bladeCenterPrefab, centerPosition, Quaternion.identity);
    }

    void Update()
    {
        if (GameSettings.gameState != GameState.InGame || Dying) { return; }

        if (SkillEffects.Instance.decoyActive && !stopCheck)
        {
            player = GameObject.FindGameObjectWithTag("Decoy");
            stopCheck = true;
        }
        else if (!SkillEffects.Instance.decoyActive && stopCheck)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            stopCheck = false;
        }

        distance = Vector2.Distance(transform.position, player.transform.position);
        Vector2 direction = player.transform.position - transform.position;

        if (SkillEffects.Instance.vanishActive) { return; }
        else
        {
            direction.Normalize();
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.GetChild(0).rotation = Quaternion.Euler(Vector3.forward * angle);
        }

        if (isCharging)
        {
            Charge();
        }
        else if (GameSettings.gameState == GameState.InGame)
        {
            chargeCooldown -= Time.deltaTime;

            if (chargeCooldown <= 0)
            {
                StartCharging();
                chargeCooldown = Random.Range(chargeCooldownMin, chargeCooldownMax);
            }
        }

        Bleed();
    }

    private void StartCharging()
    {
        isCharging = true;
        chargeTimer = 0f;
        Vector2 direction = (player.transform.position - transform.position).normalized;
        float extraDistance = 4f; 
        targetPosition = (Vector2)player.transform.position + direction * extraDistance;
        attack.GetComponent<BoxCollider2D>().enabled = true; //enable the collider
        attack.GetComponent<ChargeAttack>().originEnemy = this;  
        attacking = true;
        attack.SetActive(true); //show the attack
        Debug.Log("BladeBoss is charging towards position: " + targetPosition);
    }

    private void Charge()
    {
     
      
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, Speed * 9 * Time.deltaTime);
        chargeTimer += Time.deltaTime;
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            Debug.Log("BladeBoss has finished charging.");
            isCharging = false;  
            attack.SetActive(false);
            attacking = false;
        }
      
    }


    public override void Move()
    {
        if (SkillEffects.Instance.vanishActive || isCharging) { return; }
        float tileSpeedModifier = mapManager.GetTileWalkingSpeed(transform.position);
        transform.position = Vector2.MoveTowards(this.transform.position, player.transform.position, (Speed * tileSpeedModifier) * Time.deltaTime);
    }

}

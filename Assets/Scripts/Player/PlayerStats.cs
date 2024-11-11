using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;
    //Health
    private const int BASE_MAX_HEALTH = 100;
    private bool spinePlate = false;
    public bool SpinePlate
    {
        get {return spinePlate;}
        set {spinePlate = value;}
    }
    private int spinePercent = 0; //Percentage of damage taken to deal to the enemy
    public int SpinePercent
    {
        get {return spinePercent;}
        set {spinePercent = value;}
    }
    private int flatBonusHealth = 0;
    public int FlatBonusHealth
    {
        get {return flatBonusHealth;}
        set
        {
            flatBonusHealth = value;
            CurrentHealth += Math.Max(0, value); //This will increase the player's health by the flat bonus if it isn't negative
        }
    }
    private int percentBonusHealth = 100; //100% of base health
    public int PercentBonusHealth
    {
        get {return percentBonusHealth;}
        set 
        {
            int oldHealth = MaxHealth;
            percentBonusHealth = value;
            int newHealth = MaxHealth;
            CurrentHealth += Math.Max(0, newHealth - oldHealth);
            //This will increase the player's health by the difference between the old and new max health
            //As long as the new max health is greater than the old max health
        }
    }
    public int MaxHealth
    {
        get {return ((BASE_MAX_HEALTH + FlatBonusHealth) * PercentBonusHealth) / 100;}
    }

    private int currentHealth;
    public int CurrentHealth
    {
        get {return currentHealth;}
        set
        {
            currentHealth = Math.Min(Math.Max(0, value), MaxHealth);
            //Locks the current health to the max health
            HealthBar.Instance.UpdateHealthBar(CurrentHealth, MaxHealth);
        }
    }

    private float dotTick = 1f; //Time between damage over time ticks
    public float DotTick
    {
        get {return dotTick;}
        set {dotTick = value;}
    }

    private int dotDamage = 0; //Percentage of max health to deal per tick
    public int DotDamage
    {
        get {return dotDamage;}
        set {dotDamage = value;}
    }

    private float nextDotTick = 0; //Time of the next damage over time tick

    //Regeneration
    private float healTick = 2; //Time between healing ticks
    public float HealTick
    {
        get {return healTick;}
        set {healTick = value;}
    }

    private float nextRegenerationTick = 0; //Time of the next regeneration tick

    //The regeneration amount can be negative to deal damage over time instead of healing
    private int flatRegenerationPercentage = 0; //Percents of max health to regenerate per tick
    public int FlatRegenerationPercentage
    {
        get {return flatRegenerationPercentage;}
        set {flatRegenerationPercentage = value;}
    }
    private int percentRegenerationPercentage = 100; //100% of base regeneration amount. Use this to double/half regen amount
    public int PercentRegenerationPercentage
    {
        get {return percentRegenerationPercentage;}
        set {percentRegenerationPercentage = value;}
    }
    public int RegenerationPercentage //This returns the % of max health to regenerate per tick
    {
        get {return (FlatRegenerationPercentage * PercentRegenerationPercentage) / 100;}
    }

    //Lifesteal. The names for these are a little strange but I am keeping them the same as the other stats
    private int flatLifestealPercentage = 0; //Flat lifesteal percentage. 0 = no lifesteal, 100 = 100% of weapon damage in lifesteal
    public int FlatLifestealPercentage
    {
        get {return flatLifestealPercentage;}
        set {flatLifestealPercentage = value;}
    }
    private int percentLifestealPercentage = 100; //100% of base lifesteal percentage. 50 = half lifesteal, 200 = double lifesteal
    public int PercentLifestealPercentage
    {
        get {return percentLifestealPercentage;}
        set {percentLifestealPercentage = value;}
    }
    public int LifestealPercentage
    {
        get {return (FlatLifestealPercentage * PercentLifestealPercentage) / 100;}
    }

    //Respawns
    private int respawns;
    public int Respawns
    {
        get {return respawns;}
        set
        {
            respawns = value;
            HealthBar.Instance.UpdateRespawnDisplay(respawns);
        }
    }
    
    //other vars
    [SerializeField] private GameObject damageText;
    public UnityEvent onPlayerRespawn = new UnityEvent();
    public bool deathsDance;
    
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
    void Start()
    {
        CurrentHealth = MaxHealth;
        Respawns = GameSettings.startingRespawns;
    }

    void Update()
    {
        if (GameSettings.gameState != GameState.InGame){return;}
        if (RegenerationPercentage != 0)
        {
            CheckRegeneration();
        }
        if (DotDamage != 0)
        {
            CheckDot();
        }

        if (CurrentHealth <= 0) //if the player dies
        {
            GameSettings.gameState = GameState.Dead;
            StartCoroutine(Timeout.Instance.TimeoutPlayer(gameObject, 1f));
        }
    }

    void CheckRegeneration()
    {
        if (nextRegenerationTick <= 0) //Health property will deal with the max health cap
        {
            nextRegenerationTick = HealTick;
            CurrentHealth += Math.Max(((MaxHealth * RegenerationPercentage) / 100), 1); //Regenerate the % of max health per tick. Minimum 1
        }
        nextRegenerationTick -= Time.deltaTime;
    }

    void CheckDot()
    {
        if (nextDotTick <= 0)
        {
            nextDotTick = DotTick;
            CurrentHealth -= Math.Max(((MaxHealth * DotDamage) / 100), 1); //Deal the % of max health per tick. Minimum 1
        }
        nextDotTick -= Time.deltaTime;
    }

    public void Respawn()
    {
        //This event currently has no listeners, it is here for future use 
        onPlayerRespawn?.Invoke();
        Respawns--;
        CurrentHealth = MaxHealth;
        StartCoroutine(DisableCollisionForDuration(2f));
    }

    public IEnumerator DisableCollisionForDuration(float duration)
    {
        // Set the collision matrix to ignore collisions between the player layer and enemy attacks for the specified duration
        Physics2D.IgnoreLayerCollision(7, 9, true);

        // Wait for the specified duration
        yield return new WaitForSeconds(duration);

        // Re-enable collisions between the player layer and itself
        Physics2D.IgnoreLayerCollision(7, 9, false);
    }

    public void ReceiveDamage(int damageTaken, EnemyBase enemyHealth = null)
    {
        SFXManager.Instance.PlaySFX("PlayerHurt");
        if (SpinePlate && enemyHealth != null)
        {
            enemyHealth?.ReceiveDamage((damageTaken * SpinePercent) / 100, false);
        }
        float randomOffset = UnityEngine.Random.Range(-0.3f, 0.3f);
        GameObject damageTextInst = Instantiate(damageText, new Vector3(transform.position.x + randomOffset, transform.position.y + 1 + randomOffset, transform.position.z), Quaternion.identity);
        damageTextInst.GetComponent<TextMeshPro>().text = damageTaken.ToString();
        CurrentHealth -= damageTaken;
    }
}

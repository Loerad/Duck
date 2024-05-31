using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    //health vars
    public static float maxHealth = 100;
    public static float currentHealth;
    float regenTick = 3f;
    float regenInterval = 3f;
    public static float regenAmount = 0;
    public static bool regenTrue = false;
    public static float lifestealAmount = 0;
    //damage vars
    public static int damage = 20;
    public static int explosionSize = 0;
    public static bool explosiveBullets = false;
    public static float critChance = 0.01f;
    public static bool hasShotgun = false;
    public static int bulletAmount = 0; //this is for the extra bullets spawned by the shotgun item - it should always be even
    //other vars
    public GameObject damageText;
    public List<GameObject> lifeEggs;
    public UnityEvent onPlayerRespawn = new UnityEvent();
 
    
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        Regen();
        if(currentHealth <= 0)
        {
 
            if (lifeEggs.Count > 0)
            {
                Respawn();
            }
            else
            {
                FindObjectOfType<GameManager>().GameOver();
            }
        }
    }
    void Regen()
    {
        regenTick -= Time.deltaTime;
        if (regenTick <= 0 && regenTrue && currentHealth < maxHealth) //only works if the player is missing health
        {
            regenTick = regenInterval;
            currentHealth += regenAmount;
            if (currentHealth > maxHealth)//if the player will regen too much health
            {
                currentHealth = maxHealth;
            }
            Debug.Log($"Regen: {currentHealth}"); 
        }
    }
    void Respawn()
    {
        //This event currently has no listeners, it is here for future use 
        onPlayerRespawn?.Invoke();

        if (lifeEggs.Count > 0) //This should never run if there are no eggs, but this is here just in case
        {
            gameObject.transform.position = lifeEggs[lifeEggs.Count -1].transform.position;
            Destroy(lifeEggs[lifeEggs.Count -1]);
            lifeEggs.Remove(lifeEggs[lifeEggs.Count -1]);
        }

        currentHealth = maxHealth;
    }
    public void ReceiveDamage(int damageTaken)
    {
            GameObject damageTextInst = Instantiate(damageText, gameObject.transform);
            damageTextInst.GetComponent<TextMeshPro>().text = damageTaken.ToString();
    }
}
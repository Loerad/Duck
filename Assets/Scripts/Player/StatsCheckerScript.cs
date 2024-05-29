using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsCheckerScript : MonoBehaviour
{
    // Reference to the ItemController script
    private ItemController itemController;

    // Variables to store initial stats
    public float initialDamage;
    public float initialMaxHealth;
    public float initialMoveSpeed;
    public float initialRegenAmount;
    public float initialFirerate;
    public float initialBleedAmount;
    public float initialLifestealAmount;
    public float initialExplosiveBullets;
    public float initialEggs;
    public float initialCritChance;
    public float initialMaxHelath;
    public float initialShotgun;

    // Start is called before the first frame update
    void Start()
    {
        // Capture and log initial stats
        initialDamage = PlayerHealth.damage;
        initialMaxHealth = PlayerHealth.maxHealth;
        initialMoveSpeed = TopDownMovement.moveSpeed;
        initialRegenAmount = PlayerHealth.regenAmount;
        initialFirerate = Shooting.firerate;
        initialBleedAmount = EnemyHealth.bleedAmount;
        initialLifestealAmount = PlayerHealth.lifestealAmount;
        initialExplosiveBullets = PlayerHealth.explosionSize;
        initialEggs = 3;
        initialCritChance = PlayerHealth.critChance;
        initialMaxHelath = PlayerHealth.maxHealth;
        initialShotgun = PlayerHealth.bulletAmount;
  

        //Debug.Log($"Initial Damage: {initialDamage}");
        //Debug.Log($"Initial Max health: {initialMaxHealth}");
        //Debug.Log($"Initial Speed: {initialMoveSpeed}");
        //Debug.Log($"Initial Regen amount: {initialRegenAmount}");
        //Debug.Log($"Initial Firerate: {initialFirerate}");

        // Find the ItemController component in the scene
        itemController = FindObjectOfType<ItemController>();

        if (itemController == null)
        {
            Debug.LogError("ItemController not found!");
        }
        else
        {
            // Start the coroutine to wait and then call the method
            StartCoroutine(CallItemPickedAfterDelay());
        }
    }

    // Coroutine to wait for 10 seconds and then call ItemPicked
    private IEnumerator CallItemPickedAfterDelay()
    {
        yield return new WaitForSeconds(10); // Wait for 10 seconds

        // Call ItemPicked methods
        itemController.ItemPicked(0);
        itemController.ItemPicked(01);
        itemController.ItemPicked(02);
        itemController.ItemPicked(03);
        itemController.ItemPicked(04);
        itemController.ItemPicked(05);
        itemController.ItemPicked(06);
        itemController.ItemPicked(07);
        itemController.ItemPicked(08);
        itemController.ItemPicked(09);
        itemController.ItemPicked(10);
        itemController.ItemPicked(11);
        itemController.ItemPicked(12);

        // Log updated stats
        //Debug.Log($"Updated Damage: {PlayerHealth.damage}");
        //Debug.Log($"Updated Max health: {PlayerHealth.maxHealth}");
        //Debug.Log($"Updated Speed: {TopDownMovement.moveSpeed}");
        //Debug.Log($"Updated Regen amount: {PlayerHealth.regenAmount}");
        //Debug.Log($"Updated Firerate: {shooting.firerate}");
    }

    // Update is called once per frame
    void Update()
    {
        // No need to call ItemPicked here since it's handled by the coroutine
    }
}







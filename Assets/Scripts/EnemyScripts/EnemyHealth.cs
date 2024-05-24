using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour
{
    //public UnityEvent OnEnemyDeath = new UnityEvent();
    public int baseHealth;
    [HideInInspector] public int health;
    public float bleedTick = 1f;
    public float bleedInterval = 1f;
    public static bool bleedTrue;
    public static int bleedAmount = 0;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Bleed();
        if (health <= 0)
        {
            //save for if we need event
            //OnEnemyDeath?.Invoke();
            ScoreManager.Instance.IncreasePoints(10);
            EnemySpawner.currentEnemies.Remove(gameObject);
            Destroy(gameObject);
        }
    }
    void Bleed() //if game lag increases with lots of enemies on screen, convert this to a job
    {
        bleedTick -= Time.deltaTime;
        if (bleedTick <= 0 && bleedTrue)
        {
            bleedTick = bleedInterval;
            health -= bleedAmount; 
        }
    }
}

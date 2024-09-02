using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyExplosion : MonoBehaviour
{
    public GameObject player;
    private int explosionDamage;
    public int ExplosionDamage
    {
        set { explosionDamage = value; }
    }
    private int explosionSize;
    public int ExplosionSize
    {
        set
        {
            explosionSize = value;
            transform.localScale = new Vector3(explosionSize, explosionSize, explosionSize);
        }
    }

    private bool crit = false;
    public bool Crit
    {
        set { crit = value; }
    }
    void Start()
    {

        ExplosionSize = 6;
 
        StartCoroutine(DestroyExplosion());
    }
    private IEnumerator DestroyExplosion()
    {
        yield return new WaitForSeconds(0.25f);
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerStats>().ReceiveDamage(10);
            Destroy(gameObject);
           
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExplosion : MonoBehaviour
{
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
    [SerializeField] private GameObject radiation;
    void Start()
    {
        StartCoroutine(DestroyExplosion());
    }
    private IEnumerator DestroyExplosion()
    {
        yield return new WaitForSeconds(0.25f);
        if (WeaponStats.Instance.ItemRadioactive)
        {
            GameObject radiationInstance = Instantiate(radiation, transform.position, transform.rotation);
            radiationInstance.transform.localScale = gameObject.transform.localScale * 1.5f; //Radiation size = explosion size
        }
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            //Debug explosion target and damage
            if (other.gameObject.GetComponent<EnemyBase>())
            {
                other.gameObject.GetComponent<EnemyBase>().ReceiveDamage(explosionDamage, crit);
            }
            else
            {
                other.gameObject.GetComponent<EnemyHealth>().ReceiveDamage(explosionDamage, crit);
            }
        }
        if (WeaponStats.Instance.SelfDamageExplosions && other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerStats>().ReceiveDamage(explosionDamage/2);
        }
    }
}

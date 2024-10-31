using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Sword : MonoBehaviour
{
    private bool crit;
    public bool Crit
    {
        get {return crit;}
        set {crit = value;}
    }
    private bool reflecting = false;
    public bool Reflecting
    {
        get {return reflecting;}
        set {reflecting = value;}
    }
    [SerializeField] private GameObject reflectedBullet;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {            
            if (crit)
            {
                other.gameObject.GetComponent<EnemyBase>().ReceiveDamage(WeaponStats.Instance.CritDamage, true);
            }
            else
            {
                other.gameObject.GetComponent<EnemyBase>().ReceiveDamage(WeaponStats.Instance.Damage, false);
            }
        }
        else if (WeaponStats.Instance.HasReflector && reflecting && other.gameObject.CompareTag("Bullet") && other.gameObject.layer == 9)
        {
            SFXManager.Instance.PlaySFX("Deflect");
            Debug.Log("Hit bullet");
            GameObject bulletInstance = Instantiate(reflectedBullet, other.gameObject.transform.position, Quaternion.identity);
            bulletInstance.GetComponent<Rigidbody2D>().velocity = -other.gameObject.GetComponent<Rigidbody2D>().velocity;
            bulletInstance.GetComponent<ReflectedBullet>().Damage = other.gameObject.GetComponent<EnemyBullet>().Damage;

            Destroy(other.gameObject);
        }
    }
}

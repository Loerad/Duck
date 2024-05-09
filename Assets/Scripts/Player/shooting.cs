using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shooting : MonoBehaviour
{
    public GameObject bullet;
    public Transform firePoint;
    public float bulletSpeed = 50;

    public static float firerate = 1;
    public float shootingInterval = 0;


    Vector2 lookDirection;
    float lookAngle;

    public void Shoot()
    {
        // Capture mouse position and calculate shooting direction
        lookDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lookDirection = new Vector2(lookDirection.x - transform.position.x, lookDirection.y - transform.position.y);
        lookAngle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;

        // Rotate firePoint towards mouse position
        firePoint.rotation = Quaternion.Euler(0, 0, lookAngle);

        // Instantiate bullet clone and set its position and rotation
        GameObject bulletClone = Instantiate(bullet);
        bulletClone.transform.position = firePoint.position;
        bulletClone.transform.rotation = Quaternion.Euler(0, 0, lookAngle);

        // Apply velocity to the bullet clone
        bulletClone.GetComponent<Rigidbody2D>().velocity = firePoint.right * bulletSpeed;
    }

    void Update()
    {
        lookDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lookDirection = new Vector2(lookDirection.x - transform.position.x, lookDirection.y - transform.position.y);
        lookAngle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;

        firePoint.rotation = Quaternion.Euler(0, 0, lookAngle);
        HandleShooting();
    }
    void HandleShooting()
    {
        shootingInterval -= Time.deltaTime;
        if (shootingInterval <= 0 && Input.GetKey("space"))
        {
            shootingInterval = firerate; 
            Shooting();
        }
    }


    private void Shooting()
    {
        GameObject bulletClone = Instantiate(bullet, firePoint.position, Quaternion.Euler(0, 0, lookAngle));
        //bulletClone.transform.position = firePoint.position;
        //bulletClone.transform.rotation = Quaternion.Euler(0, 0, lookAngle);
        bulletClone.GetComponent<Rigidbody2D>().velocity = firePoint.right * bulletSpeed;
    }
}
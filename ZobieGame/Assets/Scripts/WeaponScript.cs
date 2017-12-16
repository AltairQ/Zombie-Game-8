using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    public GameObject bullet;
    public GameObject barrel_end;
    public float cooldown, reload_speed, damage;
    public int spread, bullet_count, magazine_size, bullets_left;
    System.Random rnd = new System.Random();

    float current_cooldown, current_reload;

	// Use this for initialization
	void Start ()
    {

    }

    public void Shoot()
    {
        if(current_cooldown <= 0 && bullets_left > 0 && current_reload <= 0)
        {
            for(int i = 0; i < bullet_count; i++)
            {
                GameObject new_bullet = Instantiate(bullet, barrel_end.transform.position, transform.rotation);
                new_bullet.GetComponent<BulletScript>().Initialize(transform.rotation.eulerAngles.y + rnd.Next(-spread, spread), damage);
            }

            bullets_left--;
            current_cooldown = cooldown;
        }
    }

    public void Reload()
    {
        if(current_reload <= 0 && bullets_left < magazine_size)
        {
            bullets_left = magazine_size;
            current_reload = reload_speed;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        current_reload -= Time.deltaTime;
        current_cooldown -= Time.deltaTime;
    }
}

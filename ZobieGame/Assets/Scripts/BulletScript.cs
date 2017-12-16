using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    float bullet_speed = 0.5f, life_left = 2.0f, angle_y, damage;
    Rigidbody rb;
    Vector3 movement = new Vector3();

    public void Initialize(float angle_y, float damage)
    {
        this.angle_y = angle_y;
        this.damage = damage;
        transform.rotation = Quaternion.Euler(0, this.angle_y, 0);
    }

    // Use this for initialization
    void Start ()
    {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        life_left -= Time.deltaTime;

        if (life_left <= 0)
        {
            Destroy(this.gameObject);
        }
	}

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Zombie"))
        {
            other.gameObject.GetComponent<ZombieScript>().Damage(damage);
            Destroy(this.gameObject);
        }
    }

    void FixedUpdate()
    {
        movement = new Vector3(Mathf.Sin(transform.rotation.eulerAngles.y * Mathf.Deg2Rad) * bullet_speed, 0, Mathf.Cos(transform.rotation.eulerAngles.y * Mathf.Deg2Rad) * bullet_speed);
        rb.MovePosition(transform.position + movement);
    }
}

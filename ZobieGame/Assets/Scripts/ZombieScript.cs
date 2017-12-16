using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieScript : MonoBehaviour
{
    float health;

    public void Damage(float damage)
    {
        health -= damage;
    }

	// Use this for initialization
	void Start ()
    {
        health = 100;
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(health <= 0)
        {
            Destroy(this.gameObject);
        }
	}
}

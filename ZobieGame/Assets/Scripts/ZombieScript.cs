using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieScript : MonoBehaviour
{
    float _health;

    public void Damage(float damage)
    {
        _health -= damage;
    }

	// Use this for initialization
	void Start ()
    {
        _health = 100;
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(_health <= 0)
        {
            Destroy(this.gameObject);
        }
	}
}

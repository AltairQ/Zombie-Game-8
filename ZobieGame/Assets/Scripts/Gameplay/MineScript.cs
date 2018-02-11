using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineScript : MonoBehaviour
{
    float _range = 3.0f, _damage = 1000.0f;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if(this.enabled && other.CompareTag("Zombie"))
        {
            GameObject[] zombies = GameObject.FindGameObjectsWithTag("Zombie");

            foreach(GameObject zombie in zombies)
            {
                if(Mathf.Max(0, (_range - (Vector3.Distance(transform.position, zombie.transform.position))) / _range) * _damage > 0)
                    zombie.GetComponent<ZombieScript>().Damage(Mathf.Max(0, (_range - (Vector3.Distance(transform.position, zombie.transform.position))) / _range) * _damage);
            }

            Instantiate(GameSystem.Get().Explosion, transform.position, transform.rotation);
            Destroy(this.gameObject);
        }
    }
}

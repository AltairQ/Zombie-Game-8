using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampScript : MonoBehaviour
{

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.GetChild(0).gameObject.SetActive(GameSystem.Get().Sunlight.intensity < 0.4f);
        transform.GetChild(1).gameObject.SetActive(GameSystem.Get().Sunlight.intensity < 0.4f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.GetComponent<PlayerScript>()._visibility = 1.0f;
        }
    }
}

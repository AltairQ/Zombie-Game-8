using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickupScript : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.Rotate(Vector3.forward, Time.deltaTime * 60);
    }

    void OnTriggerEnter (Collider other)
    { 
        if(other.CompareTag("Player"))
        {
            GameObject audio = Instantiate(GameSystem.Get().AudioItemPickup, transform.position, transform.rotation);
            audio.GetComponent<AudioSource>().Play();
            Destroy(audio, audio.GetComponent<AudioSource>().clip.length * 10);

            other.GetComponent<PlayerScript>().Damage(-25);
            Destroy(this.gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuppliesPickupScript : MonoBehaviour
{
    [SerializeField]
    private int _suppliesAmount = 1;

	// Use this for initialization
	void Start () {
		
	}

    void Update()
    {
        transform.Rotate(Vector3.forward, Time.deltaTime * 60);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject audio = Instantiate(GameSystem.Get().AudioItemPickup, transform.position, transform.rotation);
            audio.GetComponent<AudioSource>().Play();
            Destroy(audio, audio.GetComponent<AudioSource>().clip.length * 10);

            other.GetComponent<PlayerScript>().Supplies += _suppliesAmount;

            Destroy(this.gameObject);
        }
    }
}

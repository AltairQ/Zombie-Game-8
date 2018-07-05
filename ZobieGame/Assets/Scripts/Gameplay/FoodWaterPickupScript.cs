using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodWaterPickupScript : MonoBehaviour
{
    [SerializeField]
    private bool _food;
    private float _foodAmount = 66, _waterAmount = 66;

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

            if(_food)
                other.GetComponent<PlayerScript>().Food += _foodAmount;
            else
                other.GetComponent<PlayerScript>().Water += _waterAmount;

            Destroy(this.gameObject);
        }
    }
}

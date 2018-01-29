using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
    void Awake()
    {
        var exp = GetComponent<ParticleSystem>();
        var aud = GetComponent<AudioSource>();
        exp.Play();
        aud.Play();
        Destroy(gameObject, exp.duration);
    }

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}

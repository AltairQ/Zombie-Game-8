using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlashScript : MonoBehaviour
{
    float _lifeLeft = 0.01f;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (_lifeLeft <= 0)
            Destroy(this.gameObject);

        _lifeLeft -= Time.deltaTime;
	}
}

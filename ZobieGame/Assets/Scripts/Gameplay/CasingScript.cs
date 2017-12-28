using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasingScript : MonoBehaviour
{
    float LifeLeft;

	// Use this for initialization
	void Start ()
    {
        LifeLeft = 5.0f;
	}
	
	// Update is called once per frame
	void Update ()
    {
        LifeLeft -= Time.deltaTime;
        if (LifeLeft <= 0)
            Destroy(this.gameObject);
	}
}

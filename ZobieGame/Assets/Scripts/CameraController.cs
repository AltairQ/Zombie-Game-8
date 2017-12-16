using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public float rotation = 0, distance = 5, distance_y = 10;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        rotation = transform.rotation.eulerAngles.y;
        transform.position = player.transform.position + new Vector3(-Mathf.Sin(rotation * Mathf.Deg2Rad) * distance, distance_y, -Mathf.Cos(rotation * Mathf.Deg2Rad) * distance);
	}
}

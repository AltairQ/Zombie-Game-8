using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject _player = null;
    public float _rotation = 0, _distance = 5, _distance_y = 10;

	// Use this for initialization
	void Start ()
    {
		if(_player == null)
        {
            _player = GameSystem.Get().Player;
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        _rotation = transform.rotation.eulerAngles.y;
        transform.position = _player.transform.position + new Vector3(-Mathf.Sin(_rotation * Mathf.Deg2Rad) * _distance, _distance_y, -Mathf.Cos(_rotation * Mathf.Deg2Rad) * _distance);
	}
}

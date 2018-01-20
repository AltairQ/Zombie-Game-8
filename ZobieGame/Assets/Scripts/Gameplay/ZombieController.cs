using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    Rigidbody _rb;
    NavMeshAgent _nv;
    ZombieScript _zs;

	// Use this for initialization
	void Start ()
    {
        _rb = GetComponent<Rigidbody>();
        _nv = GetComponent<NavMeshAgent>();
        _zs = GetComponent<ZombieScript>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (_zs.Dead)
            _nv.enabled = false;
    }

    void FixedUpdate()
    {
//        rb.MovePosition(transform.position);
    }
}

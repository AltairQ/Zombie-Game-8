using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    Rigidbody _rb;
    NavMeshAgent _nv;

	// Use this for initialization
	void Start ()
    {
        _rb = GetComponent<Rigidbody>();
        _nv = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        _nv.SetDestination(GameSystem.Get().Player.transform.position);
	}

    void FixedUpdate()
    {
//        rb.MovePosition(transform.position);
    }
}

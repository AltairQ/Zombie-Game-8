using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    Rigidbody rb;
    NavMeshAgent nv;

	// Use this for initialization
	void Start ()
    {
        rb = GetComponent<Rigidbody>();
        nv = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        nv.SetDestination(GlobalVariables.Get().player.transform.position);
	}

    void FixedUpdate()
    {
        rb.MovePosition(transform.position);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float _movementSpeed = 1.0f;
    Rigidbody rb;
    Vector3 _movement = new Vector3();

    [SerializeField]
    private KeyCode _ForwardKey = KeyCode.W;
    [SerializeField]
    private KeyCode _BackKey = KeyCode.S;
    [SerializeField]
    private KeyCode _LeftKey = KeyCode.A;
    [SerializeField]
    private KeyCode _RightKey = KeyCode.D;

    // Use this for initialization
    void Start ()
    {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        _movement = new Vector3(0, 0, 0);
        int MX = 0, MZ = 0;

        float rotation = transform.rotation.eulerAngles.y;

        if (Input.GetKey(_ForwardKey))
            MZ = 1;
        if (Input.GetKey(_BackKey))
            MZ = -1;
        if (Input.GetKey(_RightKey))
            MX = -1;
        if (Input.GetKey(_LeftKey))
            MX = 1;

        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, rotation, transform.rotation.eulerAngles.z));

        float new_rotation = Mathf.Atan2(MZ, MX) * Mathf.Rad2Deg + rotation - 90;
        //        float new_rotation = rotation;
        /*
                if (MX != 0 || MZ != 0)
                    movement = new Vector3(Mathf.Sin(Mathf.Deg2Rad * new_rotation) * movement_speed, 0.0f, Mathf.Cos(Mathf.Deg2Rad * new_rotation) * movement_speed);
        */

        _movement = new Vector3(-MX * _movementSpeed, 0.0f, MZ * _movementSpeed);
    }

    private void FixedUpdate()
    {
        rb.MovePosition(transform.position + _movement * 0.1f);
    }
}

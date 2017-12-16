using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float movement_speed = 1.0f;
    Rigidbody rb;
    Vector3 movement = new Vector3();

    public float direction;

    public KeyCode ForwardKey = KeyCode.W;
    public KeyCode BackKey = KeyCode.S;
    public KeyCode LeftKey = KeyCode.A;
    public KeyCode RightKey = KeyCode.D;
    public KeyCode RotateLeft = KeyCode.Q;
    public KeyCode RotateRight = KeyCode.E;

    // Use this for initialization
    void Start ()
    {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        bool moving = false;
        movement = new Vector3(0, 0, 0);
        int MX = 0, MZ = 0;

        float rotation = transform.rotation.eulerAngles.y;

        if (Input.GetKey(ForwardKey))
            MZ = 1;
        if (Input.GetKey(BackKey))
            MZ = -1;
        if (Input.GetKey(RightKey))
            MX = -1;
        if (Input.GetKey(LeftKey))
            MX = 1;

        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, rotation, transform.rotation.eulerAngles.z));

        float new_rotation = Mathf.Atan2(MZ, MX) * Mathf.Rad2Deg + rotation - 90;
        //        float new_rotation = rotation;
        /*
                if (MX != 0 || MZ != 0)
                    movement = new Vector3(Mathf.Sin(Mathf.Deg2Rad * new_rotation) * movement_speed, 0.0f, Mathf.Cos(Mathf.Deg2Rad * new_rotation) * movement_speed);
        */

        movement = new Vector3(-MX * movement_speed, 0.0f, MZ * movement_speed);
    }

    private void FixedUpdate()
    {
        rb.MovePosition(transform.position + movement * 0.1f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public GameObject Weapon;
    public KeyCode ShootKey = KeyCode.Mouse0;
    public Camera camera;
    public float angle_x, angle_y;
    PlayerController controller;

    // Use this for initialization
    void Start ()
    {
        controller = GetComponent<PlayerController>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            Transform objectHit = hit.transform;

            Vector3 rotation = transform.rotation.eulerAngles;

            angle_y = Mathf.Atan2(hit.point.x - transform.position.x, hit.point.z - transform.position.z);
            angle_x = Mathf.Atan2(hit.point.y - transform.position.y, Vector3.Distance(transform.position, hit.point));

            angle_y *= Mathf.Rad2Deg;
            angle_x *= Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(new Vector3(0, angle_y, 0));
        }

        if (Input.GetKey(KeyCode.R))
        {
            Weapon.GetComponent<WeaponScript>().Reload();
        }
        if (Input.GetKey(ShootKey))
        {
            Weapon.GetComponent<WeaponScript>().Shoot();
        }
    }
}

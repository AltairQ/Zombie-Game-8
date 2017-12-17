using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField]
    private GameObject _weapon;

    [SerializeField]
    private KeyCode _ShootKey = KeyCode.Mouse0;
    [SerializeField]
    private KeyCode _PickUpKey = KeyCode.E;

    [SerializeField]
    public Camera _camera;

    [SerializeField]
    public float _angleX, _angleY;
    PlayerController _controller;

    // Use this for initialization
    void Start ()
    {
        _controller = GetComponent<PlayerController>();
	}

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Weapon") && Input.GetKey(_PickUpKey))
        {
            _weapon = other.gameObject;
            _weapon.GetComponent<BoxCollider>().enabled = false;
            _weapon.transform.SetParent(transform);
            _weapon.transform.position = transform.position + new Vector3(0.5f, 0.0f, -0.25f);
            _weapon.transform.rotation = transform.rotation;
            _weapon.GetComponent<WeaponScript>().InitUI();
        }
    }

    // Update is called once per frame
    void Update ()
    {
        RaycastHit hit;
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            Transform objectHit = hit.transform;

            Vector3 rotation = transform.rotation.eulerAngles;

            _angleY = Mathf.Atan2(hit.point.x - transform.position.x, hit.point.z - transform.position.z);
            _angleX = Mathf.Atan2(hit.point.y - transform.position.y, Vector3.Distance(transform.position, hit.point));

            _angleY *= Mathf.Rad2Deg;
            _angleX *= Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(new Vector3(0, _angleY, 0));
        }

        if (Input.GetKey(KeyCode.R) && _weapon != null)
        {
            _weapon.GetComponent<WeaponScript>().Reload();
        }
        if (Input.GetKey(_ShootKey) && _weapon != null)
        {
            _weapon.GetComponent<WeaponScript>().Shoot();
        }
    }
}

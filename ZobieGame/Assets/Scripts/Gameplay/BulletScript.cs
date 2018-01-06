using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    float _bulletSpeed = 0.5f, _lifeLeft = 2.0f, _angleY, _damage;
    Rigidbody _rb;
    Vector3 _movement = new Vector3();

    public void Initialize(float angle_y, float damage)
    {
        this._angleY = angle_y;
        this._damage = damage;
        transform.rotation = Quaternion.Euler(0, this._angleY, 0);
    }

    // Use this for initialization
    void Start ()
    {
        _rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        _lifeLeft -= Time.deltaTime;

        if (_lifeLeft <= 0)
        {
            Destroy(this.gameObject);
        }
	}

    void OnCollisionEnter(Collision col)
    {
        if(!col.gameObject.CompareTag("Bullet"))
            Destroy(this.gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Zombie"))
        {
            other.gameObject.GetComponent<ZombieScript>().Damage(_damage);
            Destroy(this.gameObject);
        }
    }

    void FixedUpdate()
    {
        _movement = new Vector3(Mathf.Sin(transform.rotation.eulerAngles.y * Mathf.Deg2Rad) * _bulletSpeed, 0, Mathf.Cos(transform.rotation.eulerAngles.y * Mathf.Deg2Rad) * _bulletSpeed);
//        transform.position += _movement;
        _rb.MovePosition(transform.position + _movement);
    }
}

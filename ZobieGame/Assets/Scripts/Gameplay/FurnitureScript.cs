using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureScript : MonoBehaviour
{
    float _health = 100, _grabDistance = 2.0f, _currentDamageCooldown, _damageCooldown = 2.0f;
    Vector3 _offset;
    ParticleSystem _ps;
    PlayerScript _playerScript;
    Rigidbody _rb;
    bool _grabbed = false;
    bool _clicked = false;

    public Vector3 Offset { get { return _offset; } set { _offset = value; } }

	// Use this for initialization
	void Start ()
    {
        _ps = transform.GetChild(0).GetComponent<ParticleSystem>();
        _playerScript = GameSystem.Get().Player.GetComponent<PlayerScript>();
        _rb = GetComponent<Rigidbody>();
	}

	void Remove()
    {
        Destroy(this.gameObject, _ps.main.duration);
    }

    // Update is called once per frame
    void Update ()
    {
        if (_currentDamageCooldown > 0)
            _currentDamageCooldown -= Time.deltaTime;
        if (_health <= 0)
            Remove();
        if(_playerScript.GrabbedObject == this.gameObject)
        {
            _grabbed = true;
        }
        else
        {
            _grabbed = false;
        }

        if (Input.GetMouseButtonUp(1))
            _clicked = false;
	}

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Zombie"))
        {
            if(_currentDamageCooldown <= 0)
            {
                _health -= other.GetComponent<ZombieScript>().Attack;
                _currentDamageCooldown = _damageCooldown;
                _ps.Play();
            }
        }
    }

    void OnMouseOver()
    {
        if(Input.GetMouseButton(1) && !_clicked)
        {
            if (_playerScript.GrabbedObject != this.gameObject)
            {
                if (Vector3.Distance(GameSystem.Get().Player.transform.position, transform.position) < _grabDistance)
                {
                    _playerScript.GrabbedObject = this.gameObject;
                    _offset = _playerScript.transform.gameObject.transform.position - transform.position;
                    _offset = Vector3.Normalize(_offset) * (_grabDistance - 0.1f);
                    _offset = new Vector3(_offset.x, 0, _offset.z);
                }
            }
            else
            {
                _playerScript.GrabbedObject = null;
            }

            _clicked = true;
        }
    }

    void OnMouseDown()
    {

    }

    private void FixedUpdate()
    {
        if(_grabbed)
        {
            _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionY;
            Vector3 movement = _playerScript.transform.gameObject.transform.position - _offset;
            movement = new Vector3(movement.x, transform.position.y, movement.z);
            _rb.MovePosition(movement);
        }
        else
        {
            _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
        }
    }
}

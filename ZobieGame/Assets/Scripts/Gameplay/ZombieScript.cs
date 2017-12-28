using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieScript : MonoBehaviour
{
    float _health;
    float _attack = 10, _attackCooldown = 2.0f, _currentAttackCooldown = 0, _attackRange = 2.0f;
    GameObject _player;
    PlayerScript _playerScript;

    public void Damage(float damage)
    {
        _health -= damage;
    }

	// Use this for initialization
	void Start ()
    {
        _health = 100;
        _playerScript = GameSystem.Get().Player.GetComponent<PlayerScript>();
        _player = GameSystem.Get().Player;
    }

    void Attack()
    {
        if(_currentAttackCooldown <= 0 && Vector3.Distance(transform.position, _player.transform.position) <= _attackRange)
        {
            _playerScript.Damage(_attack);
            _currentAttackCooldown = _attackCooldown;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        _currentAttackCooldown -= Time.deltaTime;

        Attack();

        if (_health <= 0)
        {
            Destroy(this.gameObject);
        }
	}
}

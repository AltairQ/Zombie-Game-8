﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieScript : MonoBehaviour, IAIState, IAIActions
{
    int _ID;
    float _health;
    float _attack = 10, _attackCooldown = 2.0f, _currentAttackCooldown = 0, _attackRange = 2.0f, _timeLeft = 5.0f;
    GameObject _player;
    PlayerScript _playerScript;
    Animator _anim;
    bool _dead = false;

    public bool Dead { get { return _dead; } }
    public float Attack { get { return _attack; } }
    public int ID { get { return _ID; } set { _ID = value; } }

    public int GetID()
    {
        return _ID;
    }

    public float CurrentHealth()
    {
        return _health;
    }

    public bool IsAlive()
    {
        return !_dead;
    }

    public float EuclidDistanceToPlayer()
    {
        return Vector3.Distance(transform.position, _player.transform.position);
    }

    public bool MeleeReady()
    {
        return _currentAttackCooldown <= 0 ? true : false;
    }

    public bool PlayerInMeleeRange()
    {
        return EuclidDistanceToPlayer() <= _attackRange ? true : false;
    }

    public void Damage(float damage)
    {
        _health -= damage;
    }

    public bool GoToPlayer()
    {
        return GetComponent<NavMeshAgent>().SetDestination(GameSystem.Get().Player.transform.position);
    }

    public bool AttackMeleePlayer()
    {
        if (MeleeReady() && PlayerInMeleeRange())
        {
            _playerScript.Damage(_attack);
            _currentAttackCooldown = _attackCooldown;
            _anim.Play("Attack 1", 0, 0f);
            return true;
        }
        else
            return false;
    }

    // Use this for initialization
    void Start ()
    {
        _health = 100;
        _playerScript = GameSystem.Get().Player.GetComponent<PlayerScript>();
        _player = GameSystem.Get().Player;
        _anim = transform.GetChild(0).GetComponent<Animator>();
        _anim.Play("Walk");
    }
	
    void Die()
    {
        Destroy(this.gameObject);
    }

	// Update is called once per frame
	void Update ()
    {
        if((_anim.GetCurrentAnimatorStateInfo(0).IsName("Attack 1") && _anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1))
        {
            _anim.Play("Walk");
//            _anim.Play("Idle 1");
        }

        _currentAttackCooldown -= Time.deltaTime;

        if(!_dead)
        {
            AttackMeleePlayer();
            GoToPlayer();
        }

        if (_health <= 0)
        {
            if (_timeLeft >= 5.0f)
            {
                _anim.Play("Die 1");
                _dead = true;
                transform.GetChild(0).Translate(new Vector3(0, 0.3f, 0));

                foreach (CapsuleCollider collider in GetComponents<CapsuleCollider>())
                {
                    collider.enabled = false;
                }
            }

            _timeLeft -= Time.deltaTime;
        }

        if (_timeLeft <= 0)
            Die();
	}
}

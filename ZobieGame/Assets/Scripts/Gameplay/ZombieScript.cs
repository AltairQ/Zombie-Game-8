using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieScript : MonoBehaviour, IAIState, IAIActions
{
    int _ID;
    float _health;
    float _attack = 10, _attackCooldown = 2.0f, _currentAttackCooldown = 0, _attackRange = 2.0f, _timeLeft = 5.0f, _armor;
    GameObject _player;
    PlayerScript _playerScript;
    Animator _anim;
    NavMeshAgent _nv;
    bool _dead = false;
    float _speed, _attackScore = 0;

    public bool Dead { get { return _dead; } }
    public float Attack { get { return _attack; } }
    public int ID { get { return _ID; } set { _ID = value; } }

    public void SetGenes(Genes genes)
    {
        _ID = genes.Id;
        _health = genes.G_health;
        _attack = genes.G_strength;
        _attackRange = genes.G_melee_range;
        _speed = genes.G_speed;
        _armor = genes.G_armor;
        _nv.speed = _speed;
    }

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
        _health -= Mathf.Max(0, damage - _armor);
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
            _attackScore += _attack;
            _currentAttackCooldown = _attackCooldown;
            _anim.SetBool("Attack", true);
            return true;
        }
        else
            return false;
    }

    // Use this for initialization
    // Fixed Start -> Awake
    void Awake ()
    {
        _nv = GetComponent<NavMeshAgent>();
        _health = 100;
        _playerScript = GameSystem.Get().Player.GetComponent<PlayerScript>();
        _player = GameSystem.Get().Player;
        _anim = transform.GetChild(0).GetComponent<Animator>();
//        _anim.Play("Walk");
    }
	
    void Die()
    {
        GameSystem.Get().GD.EnemyDead(_ID, _attackScore, EuclidDistanceToPlayer());
        Destroy(this.gameObject);
    }

	// Update is called once per frame
	void Update ()
    {
        if ((_anim.GetCurrentAnimatorStateInfo(0).IsName("Attack 1")))
            _anim.SetBool("Attack", false);

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

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
    bool _sent = false;
    float _speed, _attackScore = 0;
    int _level = 1;

    AudioSource _audioAttack;
    AudioSource _audioInjured;
    AudioSource _audioDead;

    public bool Dead { get { return _dead; } }
    public float Attack { get { return _attack; } }
    public int ID { get { return _ID; } set { _ID = value; } }

    public void SetGenes(Genes genes)
    {
        //_level = genes.level;
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
        _audioInjured.Play();
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
            _audioAttack.Play();
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
    void Awake()
    {
        _nv = GetComponent<NavMeshAgent>();
        _health = 100;
        _playerScript = GameSystem.Get().Player.GetComponent<PlayerScript>();
        _player = GameSystem.Get().Player;
        _anim = transform.GetChild(0).GetComponent<Animator>();

        AudioSource[] audioSources = GetComponents<AudioSource>();
        _audioAttack = audioSources[0];
        _audioInjured = audioSources[1];
        _audioDead = audioSources[2];

        //        _anim.Play("Walk");
    }

    void Die()
    {
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if ((_anim.GetCurrentAnimatorStateInfo(0).IsName("Attack 1")))
            _anim.SetBool("Attack", false);

        _currentAttackCooldown -= Time.deltaTime;

        if (!_dead)
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

                if (!_sent)
                {
                    _audioDead.Play();
                    GameSystem.Get().Player.GetComponent<PlayerScript>().Score += 10 + _ID;
                    GameSystem.Get().GD.EnemyDead(_ID, _attackScore, EuclidDistanceToPlayer());
                    _sent = true;
                }
            }

            _timeLeft -= Time.deltaTime;
        }

        if (_timeLeft <= 0)
            Die();
    }
}

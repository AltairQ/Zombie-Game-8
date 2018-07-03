using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieScript : MonoBehaviour, IAIState, IAIActions
{
    int _ID;
    float _health;
    float _attack = 10, _attackCooldown = 2.0f, _currentAttackCooldown = 0, _attackRange = 2.0f, _timeLeft = 5.0f, _armor;
    float _currentAICooldown = 0;
    GameObject _player;
    PlayerScript _playerScript;
    Animator _anim;
    NavMeshAgent _nv;
    bool _dead = false;
    bool _sent = false;
    float _speed, _attackScore = 0;
    int _level = 1;

    Stimuli _stimuli;

    AudioSource _audioAttack;
    AudioSource _audioInjured;
    AudioSource _audioDead;

    public bool Dead { get { return _dead; } }
    public float Attack { get { return _attack; } }
    public int ID { get { return _ID; } set { _ID = value; } }

    public void SetGenes(Genotype dna)
    {
        //_level = genes.level;
        _ID = dna.Id;
        _health = dna.genes.G_health;
        _attack = dna.genes.G_strength;
        _attackRange = dna.genes.G_melee_range;
        _speed = dna.genes.G_speed;
        _armor = dna.genes.G_armor;
        _nv.speed = _speed;

        _nv.radius = 0.4f * Mathf.Sqrt(Mathf.Sqrt(Mathf.Log(_health + 10) - 3.5f) * 1.5f);
        _nv.height = 2.0f * Mathf.Sqrt(Mathf.Sqrt(Mathf.Log(_health + 10) - 3.5f) * 1.5f);
        transform.GetChild(0).localScale = new Vector3(Mathf.Sqrt(Mathf.Sqrt(Mathf.Log(_health + 10) - 3.5f) * 1.5f), Mathf.Sqrt(Mathf.Sqrt(Mathf.Log(_health + 10) - 3.5f) * 1.5f), Mathf.Sqrt(Mathf.Sqrt(Mathf.Log(_health + 10) - 3.5f) * 1.5f));

        CapsuleCollider[] colliders = GetComponents<CapsuleCollider>();
        colliders[0].radius = 0.5f * Mathf.Sqrt(Mathf.Sqrt(Mathf.Log(_health + 10) - 3.5f) * 1.5f);
        colliders[0].height = 2.0f * Mathf.Sqrt(Mathf.Sqrt(Mathf.Log(_health + 10) - 3.5f) * 1.5f);
        colliders[1].radius = 0.75f * Mathf.Sqrt(Mathf.Sqrt(Mathf.Log(_health + 10) - 3.5f) * 1.5f);
        colliders[1].height = 2.0f * Mathf.Sqrt(Mathf.Sqrt(Mathf.Log(_health + 10) - 3.5f) * 1.5f);
    }

    public Stimuli CurrentStimuli()
    {
        return _stimuli;
    }

    public void ChangeStimuli(Stimuli st)
    {
        _stimuli = st;
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
        return _nv.SetDestination(GameSystem.Get().Player.transform.position);
    }

    public bool GoToStimuli()
    {
        if (_stimuli != null)
            return GetComponent<NavMeshAgent>().SetDestination(_stimuli.position);
        else
            return false;
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

    public void Suicide()
    {
        // We lower the score so much that the genes will be eliminated
        GameSystem.Get().GD.EnemyDead(_ID, -100.0f, EuclidDistanceToPlayer());

        Die();
    }

    // Update is called once per frame
    void Update()
    {
        if (_stimuli == null)
            _anim.SetBool("Walking", false);
        else
            _anim.SetBool("Walking", true);

        if ((_anim.GetCurrentAnimatorStateInfo(0).IsName("Attack 1")))
            _anim.SetBool("Attack", false);

        _currentAttackCooldown -= Time.deltaTime;
        _currentAICooldown -= Time.deltaTime;

        if (!_dead && _currentAICooldown <= 0)
        {
            GameSystem.Get().GD.Animate(this);

            _currentAICooldown = 0.1f * EuclidDistanceToPlayer();
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

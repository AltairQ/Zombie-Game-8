using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieScript : MonoBehaviour, IAIState, IAIActions
{
    bool _attacking;
    int _ID;
    float _health;
    float _attack = 10, _attackCooldown = 2.0f, _currentAttackCooldown = 0, _attackRange = 1.0f, _timeLeft = 5.0f, _armor, _disengageCooldown;
    float _currentAICooldown = 0;
    float _meleeDelay = 1.0f;
    float _currentMeleeDelay = 0.0f;
    GameObject _player;
    PlayerScript _playerScript;
    Animator _anim;
    NavMeshAgent _nv;
    bool _dead = false;
    bool _sent = false;
    float _speed, _attackScore = 0;
    int _level = 1;

    Stimuli _stimuli;
    Material _material;
    Color _color;

    AudioSource _audioAttack;
    AudioSource _audioInjured;
    AudioSource _audioDead;

    public bool Dead { get { return _dead; } }
    public float Attack { get { return _attack; } }
    public int ID { get { return _ID; } set { _ID = value; } }

    public void SetGenes(Genotype dna)
    {
        //_level = genes.level;
        _color = dna.GetColor();
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
        if ((_disengageCooldown <= 0) && MeleeReady() && PlayerInMeleeRange())
        {
            _audioAttack.Play();
            _currentMeleeDelay = _meleeDelay;
            _attacking = true;
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
        print(_color);
        _material = transform.GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().materials[2];
        _material.color = _color;
        transform.GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().materials[2] = _material;

        _nv = GetComponent<NavMeshAgent>();
        _health = 100;
        _playerScript = GameSystem.Get().Player.GetComponent<PlayerScript>();
        _player = GameSystem.Get().Player;
        _anim = transform.GetChild(0).GetComponent<Animator>();

        RuntimeAnimatorController ac = _anim.runtimeAnimatorController;    //Get Animator controller

        for (int i = 0; i < ac.animationClips.Length; i++)                 //For all animations
        {
            if (ac.animationClips[i].name == "Attack1")        //If it has the same name as your clip
            {
                _attackCooldown = ac.animationClips[i].length * 2.0f;
            }
        }

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
        if (PlayerInMeleeRange())
            _disengageCooldown -= Time.deltaTime;
        else
            _disengageCooldown = _attackCooldown;

        if (_currentMeleeDelay > 0)
            _currentMeleeDelay -= Time.deltaTime;

        if(_attacking && (_currentMeleeDelay <= 0) && PlayerInMeleeRange())
        {
            _attacking = false;
            _playerScript.Damage(_attack);
            _attackScore += _attack;
        }

        if (_stimuli == null)
            _anim.SetBool("Walking", false);
        else
            _anim.SetBool("Walking", true);

        if (!PlayerInMeleeRange())
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    [SerializeField]
    private GameObject _weaponPrimary, _weaponSecondary, _weapon;

    [SerializeField]
    private KeyCode _ShootKey = KeyCode.Mouse0;
    [SerializeField]
    private KeyCode _PickUpKey = KeyCode.E;

    [SerializeField]
    public Camera _camera;

    [SerializeField]
    public float _angleX, _angleY;
    PlayerController _controller;

    [SerializeField]
    private int[] _ammo = new int[6];
    [SerializeField]
    private int[] _ammoMax = new int[6];

    private int _mines;

    private Vector3 _gunPos;
    private bool _primarySelected = false;
    private Image _playerMenu;
    private Image _healthBar;
    private Image _healthBarBG;
    private Image _staminaBar;
    private Image _staminaBarBG;
    private Image _foodBar;
    private Image _foodBarBG;
    private Image _waterBar;
    private Image _waterBarBG;
    private float _health;
    private float _healthMax;
    private float _stamina;
    private float _staminaMax;
    private Text _ammoLeft;
    private float _food;
    private float _foodMax;
    private float _foodRate = 0.1f;
    private float _water;
    private float _waterMax;
    private float _waterRate = 0.2f;
    private int _supplies = 3;
    private int _crackers = 0;
    private int _experience = 0;
    private int _strength = 0;
    private int _endurance = 0;
    private int _dexterity = 0;
    private GameObject _grabbedObject, _grabIndicator;
    System.Random _rnd = new System.Random();
    public float _visibility = 1.0f;

    [SerializeField]
    private Text _leftText;

    AudioSource _audioInjured;

    [SerializeField]
    private Light _flashlight;

    int _score;

    public int Score { get { return _score; } set { _score = value; } }
    public float Stamina { get { return _stamina; } set { _stamina = value; } }
    public float Food { get { return _food; } set { _food = value; } }
    public float Water { get { return _water; } set { _water = value; } }
    public int Supplies { get { return _supplies; } set { _supplies = value; } }
    public int Experience { get { return _experience; } set { _experience = value; } }
    public int Strength { get { return _strength; } set { _strength = value; } }
    public int Endurance { get { return _endurance; } set { _endurance = value; } }
    public int Dexterity { get { return _dexterity; } set { _dexterity = value; } }
    public GameObject Weapon { get { return _weapon; } }
    public int[] Ammo { get { return _ammo; } }
    public int Mines { get { return _mines; } set { _mines = value; } }
    public int Crackers { get { return _crackers; } set { _crackers = value; } }

    public GameObject GrabbedObject { get { return _grabbedObject; } set { _grabbedObject = value; } }

    Animator _torso;
    Rigidbody _rb;
    Text _scoreText;

    public void IncreaseStrength()
    {
        _ammoMax[0] += 12;
        _ammoMax[1] += 6;
        _ammoMax[2] += 30;
        _ammoMax[3] += 8;
        _ammoMax[4] += 30;
        _ammoMax[5] += 5;
        _strength++;
    }

    public void IncreaseEndurance()
    {
        _healthMax += 5;
        _health += 5;
        _staminaMax += 10;
        _endurance++;
    }

    public void IncreaseDexterity()
    {
        _dexterity++;
    }

    public void HAXRefill()
    {
        _weapon.GetComponent<WeaponScript>().HAXRefillAmmo();
    }

    // Use this for initialization
    void Start ()
    {
        _gunPos = GameSystem.Get().GunPos;
        _controller = GetComponent<PlayerController>();
        _leftText = GameSystem.Get().LeftText;

        // ale szmrut
        //      twuj stary
        _healthBar = GameSystem.Get().MainCanvas.transform.GetChild(1).transform.GetChild(1).GetComponent<Image>();
        _healthBarBG = GameSystem.Get().MainCanvas.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>();
        _staminaBar = GameSystem.Get().MainCanvas.transform.GetChild(5).transform.GetChild(1).GetComponent<Image>();
        _staminaBarBG = GameSystem.Get().MainCanvas.transform.GetChild(5).transform.GetChild(0).GetComponent<Image>();
        _foodBar = GameSystem.Get().MainCanvas.transform.GetChild(6).transform.GetChild(1).GetComponent<Image>();
        _foodBarBG = GameSystem.Get().MainCanvas.transform.GetChild(6).transform.GetChild(0).GetComponent<Image>();
        _waterBar = GameSystem.Get().MainCanvas.transform.GetChild(7).transform.GetChild(1).GetComponent<Image>();
        _waterBarBG = GameSystem.Get().MainCanvas.transform.GetChild(7).transform.GetChild(0).GetComponent<Image>();
        _playerMenu = GameSystem.Get().MainCanvas.transform.GetChild(8).GetComponent<Image>();

        _health = 100;
        _healthMax = 100;
        _stamina = 50;
        _staminaMax = 50;
        _food = 100;
        _foodMax = 100;
        _water = 100;
        _waterMax = 100;

        _ammoLeft = GameSystem.Get().MainCanvas.transform.GetChild(2).GetChild(1).GetComponent<Text>();
        _grabIndicator = GameSystem.Get().MainCanvas.transform.GetChild(3).gameObject;
        _rb = GetComponent<Rigidbody>();

        _scoreText = GameSystem.Get().MainCanvas.transform.GetChild(4).gameObject.GetComponent<Text>();

        _audioInjured = GetComponents<AudioSource>()[0];

        _torso = transform.GetChild(1).GetChild(0).GetComponent<Animator>();
    }

    public void Damage(float damage)
    {
        _audioInjured.Play();
        _health -= damage;
    }

    private void PickUpWeapon(GameObject weapon, bool primary)
    {
        weapon.GetComponents<AudioSource>()[2].Play();
        weapon.GetComponent<WeaponScript>().Player = this;

        Vector3 _gunOffset = weapon.GetComponent<WeaponScript>().GunOffset;
        weapon.GetComponent<CapsuleCollider>().enabled = false;
        weapon.GetComponent<WeaponScript>().Held = true;
        weapon.transform.SetParent(transform.GetChild(1));
        weapon.transform.rotation = transform.rotation;
        weapon.transform.position = transform.position;
//        print(_gunPos + " " + (_gunPos + new Vector3(0, 0, weapon.GetComponent<WeaponScript>().Offset)));
        weapon.transform.Translate(_gunPos + _gunOffset);
//        weapon.transform.Translate(_gunPos);

        weapon.GetComponent<WeaponScript>().InitUI();

        if (primary)
        {
            _weaponPrimary = weapon;
//            _weapon = _weaponPrimary;
        }
        else
        {
            _weaponSecondary = weapon;
//            _weapon = _weaponSecondary;
        }

        if (primary != _primarySelected)
            SwitchWeapons();

        _primarySelected = primary;

        if (primary)
            _weapon = _weaponPrimary;
        else
            _weapon = _weaponSecondary;

        //        _weapon.GetComponent<WeaponScript>().InitUI();
    }

    private void DropWeapon(bool primary)
    {
        if(primary)
        {
            _weaponPrimary.GetComponent<CapsuleCollider>().enabled = true;
            _weaponPrimary.transform.SetParent(null);
            _weaponPrimary.GetComponent<WeaponScript>().Held = false;
            _weaponPrimary.transform.rotation = transform.rotation;
            _weaponPrimary.transform.position = transform.position;
            _weaponPrimary = null;
        }
        else
        {
            _weaponSecondary.GetComponent<CapsuleCollider>().enabled = true;
            _weaponSecondary.transform.SetParent(null);
            _weaponSecondary.GetComponent<WeaponScript>().Held = false;
            _weaponSecondary.transform.rotation = transform.rotation;
            _weaponSecondary.transform.position = transform.position;
            _weaponSecondary = null;
        }
    }

    public void Restock(float amount)
    {
        if (_weaponPrimary != null)
            _ammo[_weaponPrimary.GetComponent<WeaponScript>().AmmoType] += _weaponPrimary.GetComponent<WeaponScript>().MagazineSize * 2;
        if (_weaponSecondary != null)
            _ammo[_weaponSecondary.GetComponent<WeaponScript>().AmmoType] += _weaponSecondary.GetComponent<WeaponScript>().MagazineSize * 2;

        /*
                _ammo[0] += 12;
                _ammo[1] += 6;
                _ammo[2] += 30;
                _ammo[3] += 8;
                _ammo[4] += 30;
                _ammo[5] += 5;
        */
                for (int i = 0; i < _ammo.Length; i++)
                {
        /*
                    _ammo[i] += (int)(amount * _ammoMax[i]);
        */
                if (_ammo[i] > _ammoMax[i])
                        _ammo[i] = _ammoMax[i];
                }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Weapon") && Input.GetKeyDown(_PickUpKey))
        {
            if (other.gameObject.GetComponent<WeaponScript>().Primary && _weaponPrimary != null)
                DropWeapon(other.gameObject.GetComponent<WeaponScript>().Primary);
            if (!other.gameObject.GetComponent<WeaponScript>().Primary && _weaponSecondary != null)
                DropWeapon(other.gameObject.GetComponent<WeaponScript>().Primary);

            PickUpWeapon(other.gameObject, other.gameObject.GetComponent<WeaponScript>().Primary);
        }
    }

    // Update is called once per frame
    void Update ()
    {
        transform.GetChild(4).GetComponent<VisualStimulus>()._intensity = 100 * _visibility;

        _leftText.text = "mines: " + Mines + "\n" + "bait: " + Crackers;

        _stamina += Time.deltaTime * 10;

        if (_stamina > _staminaMax)
            _stamina = _staminaMax;

        if (GameSystem.Get().Sunlight.intensity < 0.4f)
            _flashlight.enabled = true;
        else
            _flashlight.enabled = false;

        _scoreText.text = _score.ToString();

        if(_primarySelected && _weapon != null && !_weapon.GetComponent<WeaponScript>().Melee)
        {
            _torso.SetBool("Primary", true);
            _torso.SetBool("Secondary", false);
            _torso.Play("PrimaryWalk", 0, 0);
        }
        else if(_weapon != null)
        {
            _torso.SetBool("Primary", false);
            _torso.SetBool("Secondary", true);
            _torso.Play("SecondaryWalk", 0, 0);
        }
        if (_weapon == null)
        {
            _torso.SetBool("Primary", false);
            _torso.SetBool("Secondary", false);
        }
        else
        {
            if(_weapon.GetComponent<WeaponScript>().CurrentReload > 0)
            {
                _torso.SetBool("Reload", true);
            }
            else
            {
                _torso.SetBool("Reload", false);
            }
        }

        if (GrabbedObject != null)
        {
            _grabIndicator.SetActive(true);
            Vector3 view_pos = GameSystem.Get().MainCamera.WorldToViewportPoint(GrabbedObject.transform.position);
            view_pos = new Vector3(Mathf.Clamp(view_pos.x, 0, 1) * Screen.width, Mathf.Clamp(view_pos.y, 0, 1) * Screen.height, view_pos.z);
            _grabIndicator.transform.position = view_pos;
            _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
        }
        else
        {
            _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            _grabIndicator.SetActive(false);
        }

        if ((_primarySelected && _weaponPrimary == null) || (!_primarySelected && _weaponSecondary == null))
            _ammoLeft.gameObject.SetActive(false);
        else
        {
            _ammoLeft.gameObject.SetActive(true);
            _ammoLeft.text = (_primarySelected ? _ammo[_weaponPrimary.GetComponent<WeaponScript>().AmmoType] : _ammo[_weaponSecondary.GetComponent<WeaponScript>().AmmoType]).ToString();
        }

        _food -= Time.deltaTime * _foodRate;
        _water -= Time.deltaTime * _waterRate;

        if ((_water < 0) || (_food < 0))
            _health -= Time.deltaTime;

        if (_health <= 0)
            Die();

        if (_health > _healthMax)
            _health = _healthMax;
        if (_stamina > _staminaMax)
            _stamina = _staminaMax;
        if (_food > _foodMax)
            _food = _foodMax;
        if (_water > _waterMax)
            _water = _waterMax;

        _healthBar.rectTransform.sizeDelta = new Vector2(_health, 10);
        _healthBarBG.rectTransform.sizeDelta = new Vector2(_healthMax + 4, 14);
        _staminaBar.rectTransform.sizeDelta = new Vector2(_stamina, 10);
        _staminaBarBG.rectTransform.sizeDelta = new Vector2(_staminaMax + 4, 14);
        _foodBar.rectTransform.sizeDelta = new Vector2(_food, 10);
        _foodBarBG.rectTransform.sizeDelta = new Vector2(_foodMax + 4, 14);
        _waterBar.rectTransform.sizeDelta = new Vector2(_water, 10);
        _waterBarBG.rectTransform.sizeDelta = new Vector2(_waterMax + 4, 14);

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
            int x = _weapon.GetComponent<WeaponScript>().MagazineSize - _weapon.GetComponent<WeaponScript>().BulletsLeft;
            if (_weapon.GetComponent<WeaponScript>().CurrentReload > 0)
                x = 0;
            _weapon.GetComponent<WeaponScript>().Reload(_ammo[_weapon.GetComponent<WeaponScript>().AmmoType]);
            _ammo[_weapon.GetComponent<WeaponScript>().AmmoType] -= x;
            _ammo[_weapon.GetComponent<WeaponScript>().AmmoType] = Mathf.Max(_ammo[_weapon.GetComponent<WeaponScript>().AmmoType], 0);
        }
        if (Input.GetKey(_ShootKey) && _weapon != null)
        {
            _weapon.GetComponent<WeaponScript>().Shoot();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwitchWeapons();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if(_mines > 0)
            {
                _mines -= 1;
                Instantiate(GameSystem.Get().Mine, transform.position - new Vector3(0, 1, 0), GameSystem.Get().Mine.transform.rotation);
            }
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (_crackers > 0)
            {
                _crackers -= 1;
                GameObject cracker = Instantiate(GameSystem.Get().Cracker, transform.position + transform.forward, transform.rotation);
                cracker.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 150, 500));
                cracker.transform.rotation = Quaternion.Euler(new Vector3(_rnd.Next(360), _rnd.Next(360), _rnd.Next(360)));
            }
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (GameSystem.Get().PlayerMenuVisible)
                GameSystem.Get().PlayerMenuVisible = false;
            else
                GameSystem.Get().PlayerMenuVisible = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if(_primarySelected != true)
                SwitchWeapons();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (_primarySelected == true)
                SwitchWeapons();
        }

        _playerMenu.gameObject.SetActive(GameSystem.Get().PlayerMenuVisible);
    }

    void Die()
    {
        SceneManager.LoadScene(0);
    }

    void SwitchWeapons()
    {
        if(_weapon == null || (_weapon != null && _weapon.GetComponent<WeaponScript>().CurrentReload <= 0))
        {
            if (!_primarySelected)
            {
                if (_weapon != null)
                {
                    _weapon.transform.position = transform.position;
                    _weapon.transform.Translate(new Vector3(0, -10, 0));
                }
                _weapon = _weaponPrimary;
                if (_weapon != null)
                {
                    _weapon.GetComponent<WeaponScript>().InitUI();
                    _weapon.transform.position = transform.position;
                    _weapon.transform.Translate(_gunPos + _weapon.GetComponent<WeaponScript>().GunOffset);
                }
            }
            if (_primarySelected)
            {
                if (_weapon != null)
                {
                    _weapon.transform.position = transform.position;
                    _weapon.transform.Translate(new Vector3(0, -10, 0));
                }
                _weapon = _weaponSecondary;
                if (_weapon != null)
                {
                    _weapon.GetComponent<WeaponScript>().InitUI();
                    _weapon.transform.position = transform.position;
                    _weapon.transform.Translate(_gunPos + _weapon.GetComponent<WeaponScript>().GunOffset);
                }
            }

            if (_weapon == null)
                WeaponScript.ClearUI();
            _primarySelected = _primarySelected ? false : true;
        }

        _visibility = Mathf.Max(0.1f, GameSystem.Get().Sunlight.intensity);
    }
}

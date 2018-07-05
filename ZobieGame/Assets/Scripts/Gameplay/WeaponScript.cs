using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    [SerializeField]
    private bool _melee;
    [SerializeField]
    private Vector3 _gunOffset;
    [SerializeField]
    private GameObject _bullet, _bulletImage, _barrelEnd, _casing, _slide, _ejectionPort, _mag;
    [SerializeField]
    private float _cooldown, _reloadSpeed, _damage, _offset;
    [SerializeField]
    private int _spread, _bulletCount, _magazineSize, _bulletsLeft, _ammoType, _noise;
    [SerializeField]
    private bool _primary, _dropOnReload, _held = false, _moveSlide;
    System.Random _rnd = new System.Random();
    private PlayerScript _playerScript;
    private bool _silenced;

    public bool Melee {  get { return _melee; } }
    public float CurrentReload { get { return _currentReload; } }
    public int MagazineSize { get { return _magazineSize; } }
    public int BulletsLeft { get { return _bulletsLeft; } }
    public int AmmoType { get { return _ammoType; } set { _ammoType = value; } }
    public bool Held { get { return _held; } set { _held = value; } }
    public bool Primary { get { return _primary; } }
    public float Offset { get { return _offset; } }
    public bool Silenced { get { return _silenced; } }
    public PlayerScript Player { get { return _playerScript; } set { _playerScript = value; } }
    public Vector3 GunOffset { get { return _gunOffset; } }

    AudioSource _shoot, _reload;

    private float _currentReload, _currentCooldown, _slidePos, _slideRec = 0.075f;
    private int _casings;
    GameObject _playerTorso;

    public void Silence()
    {
        GameObject suppresor = Instantiate(GameSystem.Get().Silencer, _barrelEnd.transform.position, transform.rotation);
        suppresor.SetParent(this.gameObject);
        _noise /= 5;
        _silenced = true;
    }

    public static void ClearUI()
    {
        foreach (Transform child in GameSystem.Get().MainCanvas.transform.GetChild(0).transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void InitUI()
    {
        foreach (Transform child in GameSystem.Get().MainCanvas.transform.GetChild(0).transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < _magazineSize; i++)
        {
            GameObject newBullet = Instantiate(_bulletImage, GameSystem.Get().MainCanvas.transform.GetChild(0));
            newBullet.transform.Translate(i * 8, 0, 0);
            if (i >= _bulletsLeft)
                newBullet.SetActive(false);
        }

        GameObject bulletIndicator = Instantiate(_bulletImage, GameSystem.Get().MainCanvas.transform.GetChild(2));
        bulletIndicator.SetParent(GameSystem.Get().MainCanvas.transform.GetChild(0).gameObject);
    }

	// Use this for initialization
	void Start ()
    {
        _held = false;
        _playerTorso = GameSystem.Get().Player;
        _playerTorso = GameSystem.Get().Player.transform.GetChild(1).gameObject;

        if (_moveSlide)
        {
//            _slide = transform.GetChild(1).GetChild(1).gameObject;
            _slidePos = _slide.transform.localPosition.z;
        }

        _reload = GetComponents<AudioSource>()[0];
        _shoot = GetComponents<AudioSource>()[1];
    }

    // You know I had to commit it to em
    public void HAXRefillAmmo()
    {
        GameSystem.Get().Player.GetComponent<PlayerScript>().Ammo[AmmoType] += 100;
        GameSystem.Get().Player.GetComponent<PlayerScript>().Damage(-100);
    }

    void R_Shoot_Melee()
    {
        if (_currentCooldown <= 0)
        {
            GameObject new_bullet = Instantiate(_bullet, _barrelEnd.transform.position, transform.rotation);
            new_bullet.GetComponent<BulletScript>().Initialize(transform.rotation.eulerAngles.y, _damage * (1 + _playerScript.Strength / 100), 0.1f);
            _currentCooldown = _cooldown;

            GameObject soundStimulus = Instantiate(GameSystem.Get().SoundStimulus, transform.position, transform.rotation);
            soundStimulus.GetComponent<SoundStimulus>().Init(_noise, 0);

            _shoot.Play();
        }
    }

    void R_Shoot()
    {
        if (_currentCooldown <= 0 && _bulletsLeft > 0 && _currentReload <= 0)
        {
            for (int i = 0; i < _bulletCount; i++)
            {
                GameObject new_bullet = Instantiate(_bullet, _barrelEnd.transform.position, transform.rotation);
                new_bullet.GetComponent<BulletScript>().Initialize(transform.rotation.eulerAngles.y + _rnd.Next(-_spread * 10, _spread * 10) / 10, _damage);
            }

            if (!_dropOnReload)
            {
                GameObject new_casing = Instantiate(_casing, _ejectionPort.transform.position, transform.rotation);
                new_casing.GetComponent<Rigidbody>().AddRelativeForce(new Vector3((100 + _rnd.Next(-50, 50)) / 10, (25 + _rnd.Next(50)) / 10, _rnd.Next(-25, 25) / 10));
                new_casing.transform.rotation = Quaternion.Euler(new Vector3(_rnd.Next(360), _rnd.Next(360), _rnd.Next(360)));
            }

            GameObject soundStimulus = Instantiate(GameSystem.Get().SoundStimulus, transform.position, transform.rotation);
            soundStimulus.GetComponent<SoundStimulus>().Init(_noise, 0);

            _shoot.Play();
            if(!_silenced)
                Instantiate(GameSystem.Get().MuzzleFlash, _barrelEnd.transform.position, Quaternion.Euler(0, transform.rotation.eulerAngles.y + 90, 0));

            // TMP HACK
            _bulletsLeft--;
            _currentCooldown = _cooldown;
            _casings++;
            GameSystem.Get().MainCanvas.transform.GetChild(0).GetChild(_bulletsLeft).gameObject.SetActive(false);

//            if (_moveSlide)
//                _slide.transform.Translate(0, 0, -0.75f);
        }
    }

    public void Shoot()
    {
        if(!_melee)
        {
            RaycastHit hit;
            var rayDirection = new Vector3(0, 0.3f, 0) + _barrelEnd.transform.position - (GameSystem.Get().Player.transform.position + Vector3.Scale(GameSystem.Get().GunPos, new Vector3(0, 1, 0)));
            rayDirection = Vector3.Scale(rayDirection, new Vector3(1, 0, 1));

            GameSystem.Get().Player.transform.GetComponent<CapsuleCollider>().enabled = false;

            if (Physics.Raycast(transform.position - Vector3.Normalize(rayDirection), rayDirection, out hit))
            {
                if (hit.distance > 2.0f || hit.transform.gameObject.CompareTag("Zombie") || hit.transform.gameObject.CompareTag("Furniture"))
                {
                    R_Shoot();
                }
            }
            else
                R_Shoot();

            GameSystem.Get().Player.transform.GetComponent<CapsuleCollider>().enabled = true;
        }
        else
        {
            R_Shoot_Melee();
        }
    }

    public void Reload(int ammo)
    {
        if(_currentReload <= 0 && _bulletsLeft < _magazineSize && GameSystem.Get().Player.GetComponent<PlayerScript>().Ammo[AmmoType] > 0)
        {
            if (_dropOnReload)
            {
                for (int i = 0; i < _casings; i++)
                {
                    GameObject new_casing = Instantiate(_casing, _ejectionPort.transform.position, transform.rotation);
                    new_casing.GetComponent<Rigidbody>().AddRelativeForce(new Vector3((100 + _rnd.Next(-50, 50)) / 10, (25 + _rnd.Next(50)) / 10, _rnd.Next(-25, 25) / 10));
                    new_casing.transform.rotation = Quaternion.Euler(new Vector3(_rnd.Next(360), _rnd.Next(360), _rnd.Next(360)));
                }

                _casings = 0;
            }
            else
            {
                GameObject NewMag = Instantiate(_mag, _mag.transform.position, _mag.transform.rotation);
                NewMag.GetComponent<Collider>().enabled = true;
                NewMag.GetComponent<Rigidbody>().isKinematic = false;
                NewMag.GetComponent<CasingScript>().enabled = true;
            }
            /*
                        foreach(Transform child in GameSystem.Get().MainCanvas.transform.GetChild(0).transform)
                        {
                            child.gameObject.active = true;
                        }
            */

            _reload.Play();

            _currentReload = ((float)(_magazineSize - _bulletsLeft) / _magazineSize) * _reloadSpeed;
            _bulletsLeft = Mathf.Min(_magazineSize, _bulletsLeft + ammo);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        //        print(_playerTorso.transform.localRotation.eulerAngles);

        if(_held && _melee)
            _playerTorso.transform.localRotation = Quaternion.Euler(new Vector3(
                Mathf.Sin(Mathf.PI * _currentCooldown / _cooldown) * 30,
                _playerTorso.transform.localRotation.eulerAngles.y,
                _playerTorso.transform.localRotation.eulerAngles.z));

        if (!_held && _bulletsLeft == 0)
        {
            Destroy(this.gameObject);
        }

        if (_held == true)
            transform.GetChild(0).gameObject.SetActive(false);
        else
        {
            transform.GetChild(0).gameObject.SetActive(true);
            transform.Rotate(Vector3.up, Time.deltaTime * 60);
        }

        if (!_melee && _currentReload > 0)
        {
            _currentReload -= Time.deltaTime * (1 + _playerScript.Dexterity / 100);

            if ((1 - _currentReload / _reloadSpeed) * _magazineSize <= _bulletsLeft)
                GameSystem.Get().MainCanvas.transform.GetChild(0).GetChild(Mathf.Min((int)((1 - (_currentReload / _reloadSpeed)) * _magazineSize), _magazineSize - 1)).gameObject.SetActive(true);

            if(!_dropOnReload)
                _mag.SetActive(false);
        }
        else
            if (!_melee && !_dropOnReload)
                _mag.SetActive(true);

        if (_currentCooldown > 0)
        {
            _currentCooldown -= Time.deltaTime * (1 + _playerScript.Dexterity / 100);
        }

        if (_currentCooldown < 0)
            _currentCooldown = 0;

        if (_moveSlide)
            _slide.transform.localPosition = new Vector3(_slide.transform.localPosition.x, _slide.transform.localPosition.y, _slidePos - _slideRec * (_bulletsLeft > 0 && _currentReload <= 0 ? (_currentCooldown / _cooldown) : 1));
    }
}

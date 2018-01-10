using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    [SerializeField]
    private GameObject _bullet, _bulletImage, _barrelEnd, _casing;
    [SerializeField]
    private float _cooldown, _reloadSpeed, _damage, _offset;
    [SerializeField]
    private int _spread, _bulletCount, _magazineSize, _bulletsLeft, _ammoType;
    [SerializeField]
    private bool _primary, _dropOnReload;
    public bool Primary{ get { return _primary; } }
    public float Offset{ get { return _offset; } }
    System.Random _rnd = new System.Random();

    public float CurrentReload { get { return _currentReload; } }
    public int MagazineSize { get { return _magazineSize; } }
    public int BulletsLeft { get { return _bulletsLeft; } }
    public int AmmoType { get { return _ammoType; } set { _ammoType = value; } }
    private float _currentReload;
    private float _currentCooldown;
    private int _casings;

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
                newBullet.active = false;
        }
    }

	// Use this for initialization
	void Start ()
    {

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
                GameObject new_casing = Instantiate(_casing, transform.position, transform.rotation);
                new_casing.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(100 + _rnd.Next(-50, 50), 25 + _rnd.Next(50), _rnd.Next(-25, 25)));
                new_casing.transform.rotation = Quaternion.Euler(new Vector3(_rnd.Next(360), _rnd.Next(360), _rnd.Next(360)));
            }

            _bulletsLeft--;
            _currentCooldown = _cooldown;
            _casings++;
            GameSystem.Get().MainCanvas.transform.GetChild(0).GetChild(_bulletsLeft).gameObject.active = false;
        }
    }

    public void Shoot()
    {
        RaycastHit hit;
        var rayDirection = _barrelEnd.transform.position - (GameSystem.Get().Player.transform.position + Vector3.Scale(GameSystem.Get().GunPos, new Vector3(0, 1, 0)));
        rayDirection = Vector3.Scale(rayDirection, new Vector3(1, 0, 1));

        GameSystem.Get().Player.transform.GetComponent<CapsuleCollider>().enabled = false;

        if (Physics.Raycast(transform.position - Vector3.Normalize(rayDirection), rayDirection, out hit))
        {
            if (hit.distance > 2.0f || hit.transform.gameObject.CompareTag("Zombie"))
            {
                R_Shoot();
            }
        }
        else
            R_Shoot();

        GameSystem.Get().Player.transform.GetComponent<CapsuleCollider>().enabled = true;
    }

    public void Reload(int ammo)
    {
        if(_currentReload <= 0 && _bulletsLeft < _magazineSize)
        {
            if (_dropOnReload)
            {
                for (int i = 0; i < _casings; i++)
                {
                    GameObject new_casing = Instantiate(_casing, transform.position, transform.rotation);
                    new_casing.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(100 + _rnd.Next(-50, 50), 25 + _rnd.Next(50), _rnd.Next(-25, 25)));
                    new_casing.transform.rotation = Quaternion.Euler(new Vector3(_rnd.Next(360), _rnd.Next(360), _rnd.Next(360)));
                }

                _casings = 0;
            }
            /*
                        foreach(Transform child in GameSystem.Get().MainCanvas.transform.GetChild(0).transform)
                        {
                            child.gameObject.active = true;
                        }
            */

            _currentReload = ((float)(_magazineSize - _bulletsLeft) / _magazineSize) * _reloadSpeed;
            _bulletsLeft = Mathf.Min(_magazineSize, _bulletsLeft + ammo);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (transform.parent != null)
            transform.GetChild(0).gameObject.SetActive(false);
        else
        {
            transform.GetChild(0).gameObject.SetActive(true);
            transform.Rotate(Vector3.up, Time.deltaTime * 60);
        }

        if (_currentReload > 0)
        {
            _currentReload -= Time.deltaTime;
            if((1 - _currentReload / _reloadSpeed) * _magazineSize <= _bulletsLeft)
                GameSystem.Get().MainCanvas.transform.GetChild(0).GetChild(Mathf.Min((int)((1 - (_currentReload / _reloadSpeed)) * _magazineSize), _magazineSize - 1)).gameObject.active = true;
        }
        if (_currentCooldown > 0)
            _currentCooldown -= Time.deltaTime;
    }
}

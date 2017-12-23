using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    [SerializeField]
    private GameObject _bullet;
    [SerializeField]
    private GameObject _bulletImage;
    [SerializeField]
    private GameObject _barrelEnd;
    [SerializeField]
    private float _cooldown, _reloadSpeed, _damage, _offset;
    [SerializeField]
    private int _spread, _bulletCount, _magazineSize, _bulletsLeft;
    [SerializeField]
    private bool _primary;
    public bool Primary{ get { return _primary; } }
    public float Offset{ get { return _offset; } }
    System.Random _rnd = new System.Random();

    public float CurrentReload { get { return _currentReload; } }
    private float _currentReload;
    private float _currentCooldown;

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

    public void Shoot()
    {
        if(_currentCooldown <= 0 && _bulletsLeft > 0 && _currentReload <= 0)
        {
            for(int i = 0; i < _bulletCount; i++)
            {
                GameObject new__bullet = Instantiate(_bullet, _barrelEnd.transform.position, transform.rotation);
                new__bullet.GetComponent<BulletScript>().Initialize(transform.rotation.eulerAngles.y + _rnd.Next(-_spread * 10, _spread * 10) / 10, _damage);
            }

            _bulletsLeft--;
            _currentCooldown = _cooldown;
            GameSystem.Get().MainCanvas.transform.GetChild(0).GetChild(_bulletsLeft).gameObject.active = false;
        }
    }

    public void Reload()
    {
        if(_currentReload <= 0 && _bulletsLeft < _magazineSize)
        {
            /*
                        foreach(Transform child in GameSystem.Get().MainCanvas.transform.GetChild(0).transform)
                        {
                            child.gameObject.active = true;
                        }
            */

            _currentReload = ((float)(_magazineSize - _bulletsLeft) / _magazineSize) * _reloadSpeed;
            _bulletsLeft = _magazineSize;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (transform.parent != null)
            transform.GetChild(0).gameObject.active = false;
        else
        {
            transform.GetChild(0).gameObject.active = true;
            transform.Rotate(Vector3.up, Time.deltaTime * 60);
        }

        if (_currentReload > 0)
        {
            _currentReload -= Time.deltaTime;
            GameSystem.Get().MainCanvas.transform.GetChild(0).GetChild(Mathf.Min((int)((1 - (_currentReload / _reloadSpeed)) * _magazineSize), _magazineSize - 1)).gameObject.active = true;
        }
        if (_currentCooldown > 0)
            _currentCooldown -= Time.deltaTime;
    }
}

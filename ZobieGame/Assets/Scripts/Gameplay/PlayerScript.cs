using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private Vector3 _gunPos = new Vector3(0.1f, 0.0f, 0.75f);
    private bool _primarySelected = false;
    private Image _healthBar;
    private Image _healthBarBG;
    private float _health;

    // Use this for initialization
    void Start ()
    {
        _controller = GetComponent<PlayerController>();
        _healthBar = GameSystem.Get().MainCanvas.transform.GetChild(1).transform.GetChild(1).GetComponent<Image>();
        _healthBarBG = GameSystem.Get().MainCanvas.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>();
        _health = 100;
    }

    public void Damage(float damage)
    {
        _health -= damage;
    }

    private void PickUpWeapon(GameObject weapon, bool primary)
    {
        weapon.GetComponent<BoxCollider>().enabled = false;
        weapon.transform.SetParent(transform);
        weapon.transform.rotation = transform.rotation;
        weapon.transform.position = transform.position;
//        print(_gunPos + " " + (_gunPos + new Vector3(0, 0, weapon.GetComponent<WeaponScript>().Offset)));
        weapon.transform.Translate(_gunPos + new Vector3(0, 0, weapon.GetComponent<WeaponScript>().Offset));
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
            _weaponPrimary.GetComponent<BoxCollider>().enabled = true;
            _weaponPrimary.transform.SetParent(null);
            _weaponPrimary.transform.rotation = transform.rotation;
            _weaponPrimary.transform.position = transform.position;
            _weaponPrimary = null;
        }
        else
        {
            _weaponSecondary.GetComponent<BoxCollider>().enabled = true;
            _weaponSecondary.transform.SetParent(null);
            _weaponSecondary.transform.rotation = transform.rotation;
            _weaponSecondary.transform.position = transform.position;
            _weaponSecondary = null;
        }
    }

    public void Restock(float amount)
    {
        for (int i = 0; i < _ammo.Length; i++)
        {
            _ammo[i] += (int)(amount * _ammoMax[i]);
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
        if (_health <= 0)
            this.gameObject.active = false;

        _healthBar.rectTransform.sizeDelta = new Vector2(_health, 10);
        _healthBarBG.rectTransform.sizeDelta = new Vector2(_health + 4, 14);

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
                    _weapon.transform.Translate(_gunPos + new Vector3(0, 0, _weapon.GetComponent<WeaponScript>().Offset));
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
                    _weapon.transform.Translate(_gunPos + new Vector3(0, 0, _weapon.GetComponent<WeaponScript>().Offset));
                }
            }

            if (_weapon == null)
                WeaponScript.ClearUI();
            _primarySelected = _primarySelected ? false : true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerMenuScript : MonoBehaviour
{
    [SerializeField]
    private Button _craft1, _craft2, _craft3, _craft4, _upgrade1, _upgrade2, _upgrade3;
    [SerializeField]
    private Text _suppliesText, _experienceText, _strengthText, _enduranceText, _dexterityText, _costText, _leftText;
    private PlayerScript _playerScript;
    private int _expCost = 1;

    [SerializeField]
    private GameObject _medkit;

	// Use this for initialization
	void Start ()
    {
        _playerScript = GameSystem.Get().Player.GetComponent<PlayerScript>();

        _craft1.onClick.AddListener(Craft1);
        _craft2.onClick.AddListener(Craft2);
        _craft3.onClick.AddListener(Craft3);
        _craft4.onClick.AddListener(Craft4);
        _upgrade1.onClick.AddListener(Upgrade1);
        _upgrade2.onClick.AddListener(Upgrade2);
        _upgrade3.onClick.AddListener(Upgrade3);
    }

    void Craft1()
    {
        if(_playerScript.Supplies >= 3)
        {
            _playerScript.Supplies -= 3;
            Instantiate(_medkit, _playerScript.gameObject.transform.position, _playerScript.gameObject.transform.rotation);
        }
    }

    void Craft2()
    {
        print(_playerScript.Weapon);
        print(_playerScript.Weapon.GetComponent<WeaponScript>().Silenced);

        if ((_playerScript.Weapon != null) && !_playerScript.Weapon.GetComponent<WeaponScript>().Silenced && (_playerScript.Supplies >= 2))
        {
            _playerScript.Supplies -= 2;
            _playerScript.Weapon.GetComponent<WeaponScript>().Silence();
        }
    }

    void Craft3()
    {
        if (_playerScript.Supplies >= 2)
        {
            _playerScript.Supplies -= 2;
            _playerScript.Mines += 1;
        }
    }

    void Craft4()
    {
        if (_playerScript.Supplies >= 1)
        {
            _playerScript.Supplies -= 1;
            _playerScript.Crackers += 1;
        }
    }

    void Upgrade1()
    {
        if(_playerScript.Experience >= _expCost)
        {
            _playerScript.IncreaseStrength();
            _playerScript.Experience -= _expCost;
            _expCost++;
        }
    }

    void Upgrade2()
    {
        if (_playerScript.Experience >= _expCost)
        {
            _playerScript.IncreaseEndurance();
            _playerScript.Experience -= _expCost;
            _expCost++;
        }
    }

    void Upgrade3()
    {
        if (_playerScript.Experience >= _expCost)
        {
            _playerScript.IncreaseDexterity();
            _playerScript.Experience -= _expCost;
            _expCost++;
        }
    }

    // Update is called once per frame
    void Update ()
    {
        _suppliesText.text = "Supplies: " + _playerScript.Supplies;
        _experienceText.text = "Experience: " + _playerScript.Experience;
        _strengthText.text = "Str: " + _playerScript.Strength;
        _enduranceText.text = "End: " + _playerScript.Endurance;
        _dexterityText.text = "Dex: " + _playerScript.Dexterity;
        _costText.text = "Cost: " + _expCost;
    }
}

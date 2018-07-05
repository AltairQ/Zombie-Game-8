using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarScript : MonoBehaviour
{
    bool _open = false;
    float _noise = 500.0f;
    float _light = 120.0f;
    float _searchTime = 11.0f;
    float _rotationMultiplier = -1.0f;
    float _rotationLeft = 1;
    float _trashCooldown = 2.0f;
    float _soundCooldown = 2.0f;
    float _currentSoundCooldown = 0.0f;
    Vector3 _trunkPosition;
    GameObject _visualStimulus;
    GameObject _trunk;
    GameObject _lights;
    GameObject _alarm;

    System.Random _random;

    [SerializeField]
    GameObject[] _trash = new GameObject[4];

    // Use this for initialization
    void Start ()
    {
        _trunk = transform.GetChild(0).gameObject;
        _lights = transform.GetChild(1).gameObject;
        _alarm = transform.GetChild(2).gameObject;
        _trunkPosition = transform.position + transform.right * -2.25f;
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButton(1) && (Vector3.Distance(GameSystem.Get().Player.transform.position, _trunkPosition) < 2.0f) && !_open)
        {
            _alarm.SetActive(true);
            _random = new System.Random((int)Time.time);
            _open = true;
//            GameObject soundStimulus = Instantiate(GameSystem.Get().SoundStimulus, transform.position, transform.rotation);
//            soundStimulus.GetComponent<SoundStimulus>().Init(_noise, 0);
            _visualStimulus = Instantiate(GameSystem.Get().VisualStimulus, transform.position + new Vector3(0.0f, 2.0f, 0.0f), transform.rotation);
            _visualStimulus.GetComponent<VisualStimulus>().Init(_light, 0);
            _visualStimulus.SetParent(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if(_open)
        {
            _currentSoundCooldown -= Time.deltaTime;

            if (_currentSoundCooldown < 0)
            {
                GameObject soundStimulus = Instantiate(GameSystem.Get().SoundStimulus, transform.position, transform.rotation);
                soundStimulus.GetComponent<SoundStimulus>().Init(_noise, 0);
                _currentSoundCooldown = _soundCooldown;
            }

            _lights.SetActive(Mathf.Sin(Time.time) > 0);

            if (_rotationLeft > 0)
            {
                _trunk.transform.RotateAround(transform.forward, Time.deltaTime * _rotationMultiplier);
                _rotationLeft -= Time.deltaTime;
            }

            if (Vector3.Distance(GameSystem.Get().Player.transform.position, _trunkPosition) < 2.0f)
            {
                _searchTime -= Time.deltaTime;
                _trashCooldown -= Time.deltaTime;
            }

            if (_trashCooldown < 0)
            {
//                print(transform.right);
                GameObject _newTrash = Instantiate(_trash[_random.Next(4)], transform.position + transform.right * -2.25f + new Vector3(0.0f, 1.0f, 0.0f), transform.rotation);
                _newTrash.GetComponent<Rigidbody>().AddForce(-transform.right * 100 + ((float)_random.Next(-100, 100) / 10) * transform.forward);
                _newTrash.transform.rotation = Quaternion.Euler(new Vector3(_random.Next(360), _random.Next(360), _random.Next(360)));
                _trashCooldown = 2.0f;
            }

            if (_searchTime < 0 && _searchTime > -1.0f)
            {
                var item = GeneratorAssets.Get().LootSettings.GetRandomItem();
                item.transform.position = transform.position + transform.right * -2.75f + new Vector3(0.0f, 0.25f, 0.0f);
                _alarm.SetActive(false);
                _lights.SetActive(false);
                _searchTime = -2.0f;
                Destroy(_visualStimulus);
                Destroy(this);
            }
        }
    }
}

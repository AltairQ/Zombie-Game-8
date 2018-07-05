using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrackerScript : MonoBehaviour
{
    float _lifeLeft = 10.0f;
    float _soundCooldown = 2.0f;
    float _currentSoundCooldown = 0.0f;
    float _noise = 200;
	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        _lifeLeft -= Time.deltaTime;
        _currentSoundCooldown -= Time.deltaTime;

        transform.GetChild(1).GetComponent<Light>().intensity = 1 + Mathf.Sin(Time.time * 2);

        if(_currentSoundCooldown < 0)
        {
            GameObject soundStimulus = Instantiate(GameSystem.Get().SoundStimulus, transform.position, transform.rotation);
            soundStimulus.GetComponent<SoundStimulus>().Init(_noise, 0);
            _currentSoundCooldown = _soundCooldown;
        }

        transform.GetChild(0).position = transform.position + new Vector3(0, 1.0f, 0);

        if (_lifeLeft < 0)
            Destroy(this.gameObject);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundStimulus : MonoBehaviour
{
    float _range, _volume;
    int _type;

	// Use this for initialization
	void Start ()
    {
		
	}

    public void Init(float volume, int type)
    {
        _volume = volume * 2.0f;
        _range = Mathf.Sqrt(volume);
        _type = type;
    }
	
	// Update is called once per frame
	void Update ()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _range);
        SoundStimuli soundStimuli = new SoundStimuli();
        soundStimuli.type = (SoundStimuli._Type)_type;
        soundStimuli.position = transform.position;

        for(int i = 0; i < hitColliders.Length; i++)
        {
            if(hitColliders[i].gameObject.CompareTag("Zombie"))
            {
                soundStimuli.intensity = _volume / (Vector3.Distance(transform.position, hitColliders[i].transform.position) * Vector3.Distance(transform.position, hitColliders[i].transform.position));
                GameSystem.Get().GD.ApplyStimuli(hitColliders[i].gameObject.GetComponent<ZombieScript>(), soundStimuli);
            }
        }

        Destroy(this.gameObject);
    }
}

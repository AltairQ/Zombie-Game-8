    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StimulusNode : MonoBehaviour
{
    float _range = 50;
    float _viewAngle = 85;
    ZombieScript _zs;
    Stimuli _stimuli;
    
	// Use this for initialization
	void Start ()
    {
        _zs = transform.parent.GetComponent<ZombieScript>();
	}
	
    void Update()
    {
        _stimuli = _zs.CurrentStimuli();

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _range);

        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].gameObject.CompareTag("Zombie"))
            {
                if (hitColliders[i].GetComponent<ZombieScript>().CurrentStimuli() == null)
                {
                    RaycastHit hit;
                    var rayDirection = (new Vector3(0, 1.0f, 0) + hitColliders[i].transform.position) - new Vector3(transform.position.x, 1.0f, transform.position.z);
                    rayDirection = Vector3.Scale(rayDirection, new Vector3(1, 0, 1));

                    if (Physics.Raycast(transform.position + new Vector3(0, 1, 0) + Vector3.Normalize(rayDirection), rayDirection, out hit))
                    {
                        if (hit.collider.gameObject == hitColliders[i].gameObject)
                        {
                            Transform trans = new GameObject().transform;
                            trans.position = hitColliders[i].transform.position;
                            trans.LookAt(transform.position);

                            if (Quaternion.Angle(hitColliders[i].transform.rotation, trans.rotation) < _viewAngle)
                            {
                                GameSystem.Get().GD.ApplyStimuli(hitColliders[i].gameObject.GetComponent<ZombieScript>(), _stimuli);
                            }

                            Destroy(trans.gameObject);
                        }
                    }
                }
            }
        }
    }
}

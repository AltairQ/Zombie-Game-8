using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualStimulus : MonoBehaviour
{
    [SerializeField]
    public float _range, _intensity;
    [SerializeField]
    VisualStimuli._Type _type;

    private float _viewAngle = 85.0f;

    // Use this for initialization
    void Start()
    {

    }

    public void Init(float intensity, int type)
    {
        _intensity = intensity;
        _range = intensity;
        _type = (VisualStimuli._Type)type;
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _range);
        VisualStimuli visualStimuli = new VisualStimuli();
        visualStimuli.type = _type;
        visualStimuli.position = transform.position;

        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].gameObject.CompareTag("Zombie"))
            {
                visualStimuli.intensity = _intensity / Vector3.Distance(transform.position, hitColliders[i].transform.position);
                visualStimuli.position = transform.position;

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

                        if(Quaternion.Angle(hitColliders[i].transform.rotation, trans.rotation) < _viewAngle)
                        {
                            GameSystem.Get().GD.ApplyStimuli(hitColliders[i].gameObject.GetComponent<ZombieScript>(), visualStimuli);
                        }

                        Destroy(trans.gameObject);
                    }
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVariables : MonoBehaviour
{
    public GameObject player;
    static GlobalVariables m_Instance;

	// Use this for initialization
	void Start ()
    {
		
	}

    public static GlobalVariables Get()
    {
        if (m_Instance == null)
            m_Instance = FindObjectOfType(typeof(GlobalVariables)) as GlobalVariables;

        return m_Instance;
    }

    // Update is called once per frame
    void Update ()
    {
		
	}
}

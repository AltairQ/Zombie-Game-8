using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject _player;

    private Forest _forest;
    private City _city;
    private void Awake()
    {
        _forest = GetComponent<Forest>();
        _city = GetComponent<City>();
    }
    private void Start()
    {
        
    }
}

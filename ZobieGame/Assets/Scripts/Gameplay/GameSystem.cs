﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameSystem : MonoBehaviour
{
    private static GameSystem _instance = null;
    public static GameSystem Get()
    {
        return _instance;
    }

	private void Awake ()
    {
        _instance = this;
        StartGame();
    }

    private void Start()
    {
        MapSystem.Get().Init(_player);
    }

    [SerializeField]
    private Camera _mainCamera;

    [SerializeField]
    private Canvas _mainCanvas;

    [SerializeField]
    private GameObject _playerPrefab;
    [SerializeField]
    private GameObject _zombiePrefab;
    [SerializeField]
    private GameObject _muzzleFlash;

    private GameObject _player = null;
    private Vector3 _gunPos = new Vector3(0.1f, 0.3f, 0.75f);

    public GameObject Player { get { return _player; } }
    public Vector3 GunPos { get { return _gunPos; } }
    public Canvas MainCanvas { get { return _mainCanvas; } }
    public Camera MainCamera { get { return _mainCamera; } }
    public GameObject MuzzleFlash { get { return _muzzleFlash; } }
    private List<GameObject> _zombies = new List<GameObject>();

    public void BuildNavMesh()
    {
        NavMeshSurface surface = GetComponent<NavMeshSurface>();
        surface.BuildNavMesh();
    }

    public void StartGame()
    { 
        Vector3 startPos = Vector3.up + new Vector3(0.01f, 0f, 0.01f); // to avoid map generation bug
        _player = Instantiate(_playerPrefab, startPos, Quaternion.identity);
        _player.GetComponent<PlayerScript>()._camera = _mainCamera;
        
        _mainCanvas.transform.GetChild(0).transform.position = new Vector3(20, Screen.height - 20, 0);
        _mainCanvas.transform.GetChild(1).transform.position = new Vector3(10, Screen.height - 50, 0);
        _mainCanvas.transform.GetChild(2).transform.position = new Vector3(Screen.width - 70, Screen.height - 20, 0);
    }

    public void EndGame()
    {
        Destroy(_player);
        foreach(var zombie in _zombies)
        {
            Destroy(zombie);
        }
        _zombies.Clear();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
//            Vector2 randomShift = Random.insideUnitCircle * 3;
            Vector2 randomShift = new Vector2(Mathf.Sin(Random.Range(-Mathf.PI, Mathf.PI)) * 5, Mathf.Cos(Random.Range(-Mathf.PI, Mathf.PI)) * 5);
            Vector3 shiftPos = new Vector3(randomShift.x, 1, randomShift.y);
            SpawnZombie(_player.transform.position + shiftPos);
        }
    }

    public void SpawnZombie(Vector3 position)
    {
        var newZombie = Instantiate(_zombiePrefab, position, _zombiePrefab.transform.rotation);
        _zombies.Add(newZombie);
    }
}

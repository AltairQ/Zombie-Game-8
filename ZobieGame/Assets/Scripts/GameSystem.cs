﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField]
    private Camera _mainCamera;

    [SerializeField]
    private Canvas _mainCanvas;

    [SerializeField]
    private GameObject _playerPrefab;

    [SerializeField]
    private GameObject _zombiePrefab;

    private GameObject _player = null;
    public GameObject Player { get { return _player; } }
    public Canvas MainCanvas { get { return _mainCanvas; } }
    private List<GameObject> _zombies = new List<GameObject>();

    public void StartGame()
    {
        Vector3 startPos = Vector3.up;
        _player = Instantiate(_playerPrefab, startPos, Quaternion.identity);
        _player.GetComponent<PlayerScript>()._camera = _mainCamera;
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
            Vector3 shiftPos = new Vector3(randomShift.x, 0, randomShift.y);
            SpawnZombie(_player.transform.position + shiftPos);
        }
    }

    public void SpawnZombie(Vector3 position)
    {
        var newZombie = Instantiate(_zombiePrefab, position, _zombiePrefab.transform.rotation);
        _zombies.Add(newZombie);
    }
}

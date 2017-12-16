using System.Collections;
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
    private GameObject _playerPrefab;

    [SerializeField]
    private GameObject _zombiePrefab;

    private GameObject _player;
    private List<GameObject> _zombies;

    public void StartGame()
    {
        Vector3 startPos = Vector3.zero;
        _player = Instantiate(_playerPrefab, startPos, Quaternion.identity);
        _player.GetComponent<PlayerScript>().camera = _mainCamera;
    }

    public void EndGame()
    {
        Destroy(_player);
        foreach(var zombie in _zombies)
        {
            Destroy(zombie);
        }
    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            SpawnZombie(_player.transform.position + Random.insideUnitSphere * 5);
        }
    }

    public void SpawnZombie(Vector3 position)
    {
        var newZombie = Instantiate(_zombiePrefab, position, _zombiePrefab.transform.rotation);
        _zombies.Add(newZombie);
    }
}

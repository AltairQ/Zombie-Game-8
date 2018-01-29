using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameSystem : MonoBehaviour, IAIEnvActions, IAIEnvState
{
    private static GameSystem _instance = null;
    public static GameSystem Get()
    {
        return _instance;
    }

    public bool _gameDirectorOn = true;
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
    [SerializeField]
    private GameObject _explosion;
    [SerializeField]
    private GameObject _mine;

    private GameObject _player = null;
    private Vector3 _gunPos = new Vector3(0.1f, 0.3f, 0.75f);

    [SerializeField]
    private Light _light;

    [SerializeField]
    private GameObject _audioItemPickup;

    public GameObject AudioItemPickup { get { return _audioItemPickup; } }
    public GameObject Player { get { return _player; } }
    public Vector3 GunPos { get { return _gunPos; } }
    public Canvas MainCanvas { get { return _mainCanvas; } }
    public Camera MainCamera { get { return _mainCamera; } }
    public GameObject MuzzleFlash { get { return _muzzleFlash; } }
    public GameObject Explosion { get { return _explosion; } }
    public GameObject Mine { get { return _mine; } }
    public Light Sunlight { get { return _light; } }
    private List<GameObject> _zombies = new List<GameObject>();

    // (in seconds) next time to execute GameDirector routine (update)
    private float _nextDirectorTime;

    // interval between calls to GameDirector.
    private float _directorInterval = 2.0F;

    private GameDirector _GD = new GameDirector();

    public GameDirector GD { get { return _GD; } }

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
        _mainCanvas.transform.GetChild(4).transform.position = new Vector3(Screen.width / 2, Screen.height - 48, 0);
        _mainCanvas.transform.GetChild(5).transform.position = new Vector3(10, Screen.height - 65, 0);

        _light = GameObject.FindWithTag("Light").GetComponent<Light>();

        _nextDirectorTime = Time.time + _directorInterval; 
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

    public void SpawnEnemy(Genes genes)
    {
        print(genes);
        SpawnRandomZombie(genes, 40.0F);
    }

    private void SpawnRandomZombie(Genes genes, float radius = 30.0F)
    {
        Vector2 randomShift = new Vector2(
            Mathf.Sin(Random.Range(-Mathf.PI, Mathf.PI)) * radius,
            Mathf.Cos(Random.Range(-Mathf.PI, Mathf.PI)) * radius);
        Vector3 shiftPos = new Vector3(randomShift.x, 1, randomShift.y);
        SpawnZombie(_player.transform.position + shiftPos, genes);
    }

    private void Update()
    {
        /*        
                if(Input.GetKeyDown(KeyCode.Z))
                {
                    //            Vector2 randomShift = Random.insideUnitCircle * 3;
                    SpawnRandomZombie(_GD.NewEnemy());            
                }
        */

        _light.intensity = 0.1f + Mathf.Clamp(Mathf.Sin(Time.time * 0.1f) + 0.5f, 0, 1) * 0.9f;
        _light.shadowStrength = _light.intensity;

        if (Time.time >= _nextDirectorTime)
        {
            // GAMEDIRECTOR UPDATE LOL

            _nextDirectorTime = Time.time + _directorInterval;
            if(_gameDirectorOn)
            {
                _GD.WorldTick(this);
            }
            
        }

    }

    public void SpawnZombie(Vector3 position, Genes genes)
    {
        var newZombie = Instantiate(_zombiePrefab, position, _zombiePrefab.transform.rotation);

        newZombie.GetComponent<ZombieScript>().SetGenes(genes);
        _zombies.Add(newZombie);
    }
}

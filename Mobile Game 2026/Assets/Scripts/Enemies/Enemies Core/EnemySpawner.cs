using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;
    private Transform _player;

    [SerializeField] private GameObject _enemyPrefab;

    public bool canSpawn = true;

    [Header("Spawn Area (Rectangle)")]
    [SerializeField] private float _spawnWidth = 30f;
    [SerializeField] private float _spawnHeight = 20f;

    [Header("Spawn Settings")]
    [SerializeField] private float _spawnRate = 1f;
    [SerializeField] private float _maxSpawnRate = 20f;
    private float _timer;

    [Header("Dynamic Difficulty")]
    [SerializeField] private float _difficultyIncreaseRate = 1f;
    private float _difficultyTimer = 0f;
    private float _difficulty = 1f;

    [Header("HP Range")]
    [SerializeField] private Vector2Int _hpRange = new(3, 10);

    [Header("Scaling Curves (Difficulty Multiplier)")]
    [SerializeField] private AnimationCurve _hpCurve = AnimationCurve.Linear(0, 1, 10, 5);
    [SerializeField] private AnimationCurve _spawnCurve = AnimationCurve.Linear(0, 1, 10, 4);

    [Header("Infinite Difficulty")]
    [SerializeField] private float _infiniteHpSlope = 0.25f;
    [SerializeField] private float _infiniteSpawnSlope = 0.15f;

    [Header("Pooling")]
    [SerializeField] private int _initialPoolSize = 50;
    [SerializeField] private int _maxActiveEnemies = 150;

    private int _scaledHpMin;
    private int _scaledHpMax;
    private float _scaledSpawnRate;
    private int _activeEnemies = 0;
    private Queue<BasicEnemy> _pool = new();


    private void OnDisable()
    {
        PlayerManager.Instance.OnPlayerDied -= OnPlayerDie;
        LevelManager.OnBossSpawned -= OnBossSpawned;
    }

    private void OnEnable()
    {
        Instance = this;
    }

    private void Start()
    {
        _player = PlayerManager.Instance.transform;

        PlayerManager.Instance.OnPlayerDied += OnPlayerDie;
        LevelManager.OnBossSpawned += OnBossSpawned;

        for (int i = 0; i < _initialPoolSize; i++)
        {
            BasicEnemy e = CreateEnemy();
            _pool.Enqueue(e);
        }
    }

    private void OnDestroy()
    {
        Instance = null;
        _pool.Clear();
    }

    private void Update()
    {
        if (_player == null || !canSpawn) return;

        _spawnWidth = WorldAreaManager.Instance.spawnWidth;
        _spawnHeight = WorldAreaManager.Instance.spawnHeight;

        _difficultyTimer += Time.deltaTime;
        _difficulty = _difficultyTimer * _difficultyIncreaseRate;

        float hpMultiplier = EvaluateInfinite(_hpCurve, _difficulty, _infiniteHpSlope);
        float spawnMultiplier = EvaluateInfinite(_spawnCurve, _difficulty, _infiniteSpawnSlope);

        _scaledHpMin = Mathf.Max(Mathf.RoundToInt(_hpRange.x * hpMultiplier), 1);
        _scaledHpMax = Mathf.Max(Mathf.RoundToInt(_hpRange.y * hpMultiplier), _scaledHpMin + 1);
        _scaledSpawnRate = Mathf.Min(_spawnRate * spawnMultiplier, _maxSpawnRate);

        _timer += Time.deltaTime;
        if (_timer >= 1f / _scaledSpawnRate)
        {
            _timer = 0f;
            SpawnEnemy();
        }
    }

    private float EvaluateInfinite(AnimationCurve curve, float t, float slope)
    {
        if (curve.keys.Length == 0) return 1f;

        var lastKey = curve.keys[curve.length - 1];

        if (t <= lastKey.time)
            return curve.Evaluate(t);

        float extra = (t - lastKey.time) * slope;
        return lastKey.value + extra;
    }

    private BasicEnemy CreateEnemy()
    {
        GameObject obj = Instantiate(_enemyPrefab);
        obj.SetActive(false);
        return obj.GetComponent<BasicEnemy>();
    }

    private BasicEnemy GetEnemyFromPool()
    {
        if (_pool.Count > 0)
            return _pool.Dequeue();

        return CreateEnemy();
    }

    public void ReturnEnemyToPool(BasicEnemy enemy)
    {
        _activeEnemies = Mathf.Max(_activeEnemies - 1, 0);
        enemy.gameObject.SetActive(false);
        _pool.Enqueue(enemy);
    }

    private void SpawnEnemy()
    {
        if (_activeEnemies >= _maxActiveEnemies) return;

        Vector3 spawnPos = GetRandomPointOnRectangleEdge();

        BasicEnemy e = GetEnemyFromPool();
        e.transform.position = spawnPos;
        e.gameObject.SetActive(true);
        _activeEnemies++;

        int newHp = Random.Range(_scaledHpMin, _scaledHpMax + 1);
        e.Initialize(_player.position, newHp);
    }

    private Vector3 GetRandomPointOnRectangleEdge()
    {
        float halfW = _spawnWidth * 0.5f;
        float halfH = _spawnHeight * 0.5f;

        float totalPerimeter = 2f * (_spawnWidth + _spawnHeight);
        float roll = Random.Range(0f, totalPerimeter);

        Vector3 pos;

        if (roll < _spawnWidth)
            pos = new Vector3(roll - halfW, halfH, 0);
        else if (roll < _spawnWidth * 2f)
            pos = new Vector3((roll - _spawnWidth) - halfW, -halfH, 0);
        else if (roll < _spawnWidth * 2f + _spawnHeight)
            pos = new Vector3(-halfW, (roll - _spawnWidth * 2f) - halfH, 0);
        else
            pos = new Vector3(halfW, (roll - _spawnWidth * 2f - _spawnHeight) - halfH, 0);

        return transform.position + pos;
    }

    private void OnPlayerDie()
    {
        canSpawn = false;
        ReturnAllEnemiesToPool();
        Destroy(gameObject);
    }

    private void OnBossSpawned()
    {
        canSpawn = false;
    }

    private void ReturnAllEnemiesToPool()
    {
        BasicEnemy[] activeInScene = FindObjectsByType<BasicEnemy>(FindObjectsSortMode.None);
        foreach (BasicEnemy e in activeInScene)
        {
            if (e.gameObject.activeSelf)
                ReturnEnemyToPool(e);
        }
    }
}
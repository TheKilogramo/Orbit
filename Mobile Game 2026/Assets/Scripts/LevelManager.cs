using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LevelManager : MonoBehaviour
{
    public static event Action OnBossSpawned;

    [Header("Boss")]
    public float _bossTime;
    public Image _bossTimeBar;
    public GameObject _bossPrefab;

    private bool _bossSpawned;
    private float _timer;

    [Header("Boss Warning")]
    public GameObject _warningObject;
    public int _toggleCount;
    public float _totalDuration;
    public AudioClip _warningSound;


    private void OnEnable()
    {
        BossBase.OnBossDefeated += OnBossDefeated;
    }

    private void OnDisable()
    {
        BossBase.OnBossDefeated -= OnBossDefeated;
    }

    private void Update()
    {
        if (!_bossSpawned)
        {
            if (_timer >= _bossTime)
            {
                _bossSpawned = true;

                OnBossSpawned?.Invoke();
                StartCoroutine(SpawnBoss());
            }

            _bossTimeBar.fillAmount = _timer / _bossTime;
            _timer += Time.deltaTime;
        }
    }

    public IEnumerator SpawnBoss()
    {
        float cycleDuration = _totalDuration / _toggleCount;
        float half = cycleDuration / 2f;
        _warningObject.SetActive(false);

        for (int i = 0; i < _toggleCount; i++)
        {
            // Turn ON
            _warningObject.SetActive(true);
            AudioPlayer.PlayOneShot(_warningSound, .35f);
            yield return new WaitForSeconds(half);

            // Turn OFF
            _warningObject.SetActive(false);
            yield return new WaitForSeconds(half);
        }

        _warningObject.SetActive(false);

        float halfSpawnH = WorldAreaManager.Instance.spawnHeight * 0.5f;

        // Spawn at the top-center of the SPAWN area
        float spawnY = WorldAreaManager.Instance.center.y + halfSpawnH + 0.1f;
        float spawnX = WorldAreaManager.Instance.center.x;

        Vector3 spawnPos = new Vector3(spawnX, spawnY, 0);
        Instantiate(_bossPrefab, spawnPos, Quaternion.identity);
    }

    public void OnBossDefeated()
    {
        StartCoroutine(BackToMenu());
    }

    IEnumerator BackToMenu()
    {
        yield return new WaitForSeconds(2f);
        GameManager.LoadScene(GameManager.SCENE_NAME_MAIN_MENU);
    }
}

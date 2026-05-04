using System.Collections;
using UnityEngine;

public class CometEffect : EffectBase
{
    [Header("Comet Settings")]
    [SerializeField] private GameObject _cometPrefab;
    [SerializeField] private int _cometsPerBurst = 3;
    [SerializeField] private float _spawnInterval = 3f;
    [SerializeField] private float _spawnOffset = 0.5f;
    [SerializeField] private float _angleOffset = 0;
    [SerializeField] private float _damage = 50f;

    [Header("Alert")]
    [SerializeField] private ParticleSystem _alertParticle;
    [SerializeField] private AudioClip _shootSound;
    [SerializeField] private float _alertTime = 1f;

    private bool _initialized = false;

    private float _timer = 0f;

    public override void Initialize()
    {
        StartCoroutine(InitRoutine());
    }

    IEnumerator InitRoutine()
    {
        yield return new WaitForSeconds(1.25f);

        _timer = _spawnInterval - 1.1f;
        _initialized = true;
    }

    public override void OnUpdate()
    {
        if (!_initialized) return;

        _timer += Time.deltaTime;

        if (_timer >= _spawnInterval)
        {
            _timer = 0f;
            StartCoroutine(AlertThenSpawn());
        }
    }

    public override void Disable() { }

    private IEnumerator AlertThenSpawn()
    {
        // spawn alert particles at each comet's future position
        for (int i = 0; i < _cometsPerBurst; i++)
        {
            float angle = ((360f / _cometsPerBurst) * i) + _angleOffset;
            float rad = angle * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
            Vector3 spawnPos = transform.position + (Vector3)(direction * _spawnOffset);

            _alertParticle.Play();
        }

        yield return new WaitForSeconds(_alertTime);

        SpawnComets();
        AudioPlayer.PlayOneShot(_shootSound, .45f);
    }

    private void SpawnComets()
    {
        for (int i = 0; i < _cometsPerBurst; i++)
        {
            float angle = ((360f / _cometsPerBurst) * i) + _angleOffset;
            float rad = angle * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
            Vector3 spawnPos = transform.position + (Vector3)(direction * _spawnOffset);

            GameObject obj = Instantiate(_cometPrefab, spawnPos, Quaternion.identity);
            float rotAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            obj.transform.rotation = Quaternion.Euler(0f, 0f, rotAngle);
            obj.GetComponent<Comet>().Initialize(_damage, direction);
        }
    }
}
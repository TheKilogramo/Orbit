using CandyCoded.HapticFeedback;
using UnityEngine;

public class BasicEnemy : EnemyHP
{
    [Header("Movement")]
    [SerializeField] private float _speed = 2f;

    private float _effectiveSpeed = 2f;

    private const float DESPAWN_PADDING = 1f;
    private Vector3 _targetDirection;
    private Camera _mainCamera;

    [Header("Visuals")]
    [SerializeField] private ParticleSystem _hitParticles;
    [SerializeField] private GameObject _deathParticlesPrefab;
    [Space]
    [SerializeField] private AudioClip _deathSound;

    private Color _myColor;

    [HideInInspector] public int SpawnIndex;

    [Header("Enemy Size Scaling")]
    [SerializeField] private float _baseScale = 1f;
    [SerializeField] private float _scalePerHp = 0.02f;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _effectiveSpeed = _speed;
    }

    private void OnEnable()
    {
        PlayerManager.Instance.OnPlayerDied += OnPlayerDie;
    }

    private void OnDisable()
    {
        PlayerManager.Instance.OnPlayerDied -= OnPlayerDie;

        if (EnemySortManager.Instance)
            EnemySortManager.Instance.UnregisterEnemy(this);
    }

    public override void Initialize(Vector3 playerPosition, int newHp)
    {
        base.Initialize(playerPosition, newHp);

        _targetDirection = (playerPosition - transform.position).normalized;
        _hp = newHp;
        _maxHp = newHp;

        _myColor = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);
        SpriteRendr.color = _myColor;

        ParticleSystem.MainModule main = _hitParticles.main;
        main.startColor = new ParticleSystem.MinMaxGradient(_myColor);

        Vector3 currentRotation = SpriteRendr.transform.localEulerAngles;
        currentRotation.z = Random.Range(0f, 360f);
        SpriteRendr.transform.localEulerAngles = currentRotation;

        if (EnemySortManager.Instance)
            EnemySortManager.Instance.RegisterEnemy(this);
    }

    private void OnPlayerDie()
    {
        _speed = 0f;
        Destroy(this);
    }

    public override void Damage(float damage, bool showDamagedParticles)
    {
        base.Damage(damage);

        if(showDamagedParticles)
            _hitParticles.Play();
    }

    public override void Die()
    {
        GameObject dp = ParticlePool.Instance.Spawn(_deathParticlesPrefab, transform.position, transform.rotation);
        HapticFeedback.MediumFeedback();

        var ps = dp.GetComponent<ParticleSystem>();
        var main = ps.main;
        main.startColor = new ParticleSystem.MinMaxGradient(_myColor);

        base.Die();

        if (AudioPlayer.Instance != null)
            AudioPlayer.PlayBulkOneShot(_deathSound, .1f);

        Initialized = false;

        PlayerManager.Instance?.AddXP(_maxHp);

        if (EnemySortManager.Instance)
            EnemySortManager.Instance.UnregisterEnemy(this);

        EnemySpawner.Instance.ReturnEnemyToPool(this);
    }

    public override void Update()
    {
        base.Update();

        if (_slowed) _effectiveSpeed = _speed * _slowMultiplier;
        else _effectiveSpeed = _speed;

        transform.position += _targetDirection * _effectiveSpeed * Time.deltaTime;

        CheckDespawn();

        float scaleFromHp = _baseScale + (_hp * _scalePerHp);
        transform.localScale = Vector3.one * scaleFromHp;
    }

    private void CheckDespawn()
    {
        if (!_mainCamera) return;

        Vector3 viewPos = _mainCamera.WorldToViewportPoint(transform.position);

        if (viewPos.x < -DESPAWN_PADDING ||
            viewPos.x > 1 + DESPAWN_PADDING ||
            viewPos.y < -DESPAWN_PADDING ||
            viewPos.y > 1 + DESPAWN_PADDING)
        {
            EnemySpawner.Instance.ReturnEnemyToPool(this);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerManager>(out var player))
            player.Damage();
    }
}
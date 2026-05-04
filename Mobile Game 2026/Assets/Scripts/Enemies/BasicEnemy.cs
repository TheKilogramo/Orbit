using CandyCoded.HapticFeedback;
using UnityEngine;

public class BasicEnemy : EnemyBase
{
    //movement
    private Vector3 _targetDirection;

    //despawning
    private const float DESPAWN_PADDING = 1f;
    private Camera _mainCamera;

    //visuals
    private Color _myColor;

    [Header("Enemy Size Scaling")]
    [SerializeField] private float _baseScale = 1f;
    [SerializeField] private float _scalePerHp = 0.02f;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    public override void Initialize(int newHp)
    {
        base.Initialize(newHp);

        //set target direction
        _targetDirection = (PlayerManager.Instance.transform.position - transform.position).normalized;

        //assign random color
        _myColor = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);
        SpriteRendr.color = _myColor;

        //set damageParticles to random color
        ParticleSystem.MainModule main = _damageParticles.main;
        main.startColor = new ParticleSystem.MinMaxGradient(_myColor);

        //set random rotation
        Vector3 currentRotation = SpriteRendr.transform.localEulerAngles;
        currentRotation.z = Random.Range(0f, 360f);
        SpriteRendr.transform.localEulerAngles = currentRotation;
    }

    protected override void Update()
    {
        base.Update();

        //move
        transform.position += _targetDirection * _effectiveSpeed * Time.deltaTime;

        //despawn if out of camera view
        CheckDespawn();

        //change size depending on current hp
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
        //damage player on collision
        if (collision.TryGetComponent<PlayerManager>(out var player))
            player.Damage();
    }
}
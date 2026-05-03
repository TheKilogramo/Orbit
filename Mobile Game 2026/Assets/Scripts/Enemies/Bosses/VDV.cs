using CandyCoded.HapticFeedback;
using JetBrains.Annotations;
using System.Collections;
using UnityEngine;

public class VDV : BossBase
{
    [Header("Movement")]
    [SerializeField] private Vector2 _velocity;
    [SerializeField] private float _cornerSpeedMultiplier = 1.25f;
    private float _cornerMultiplier = 1f;

    [Header("Bounds")]
    [SerializeField] private float _halfWidth = 0.5f;
    [SerializeField] private float _halfHeight = 0.5f;
    [SerializeField] private float _cornerDetectSize = .2f;

    [Header("Settings")]
    [SerializeField] private float _startingSpeed = 4f;

    private float _speed;
    private float _effectiveSpeed;

    [Header("Visuals")]
    [SerializeField] private GameObject _dieParticles;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite _angrySprite;
    [SerializeField] private Sprite _happySprite;
    [SerializeField] private ParticleSystem _confettiParticles;
    [SerializeField] private ParticleSystem _hitParticles;
    [SerializeField] private AudioClip _celebrationSound;
    [SerializeField] private AudioClip _pongWallHitSound;
    [SerializeField] private AudioClip _dieSound;
    [SerializeField] private AudioClip _dieSound2;
    [SerializeField] private AudioClip _spawnSound;
    [SerializeField] private AudioClip _damageSound;

    private Color _myColor;
    private bool _activeMovement = false;
    private Vector2 _direction;
    private Coroutine _happySpriteCoroutine;

    private bool _isHappy = false;

    public override void Initialize()
    {
        base.Initialize();

        do
        {
            _direction = Random.insideUnitCircle.normalized;
        }
        while (Mathf.Abs(_direction.y) < 0.25f || Mathf.Abs(_direction.x) < 0.25f);

        _speed = _startingSpeed;

        if (_slowed)
            _effectiveSpeed = _speed * _slowMultiplier * _cornerMultiplier;
        else
            _effectiveSpeed = _speed * _cornerMultiplier;


        _velocity = _direction * _effectiveSpeed;
        _activeMovement = true;
    }

    public override void Update()
    {
        base.Update();

        if (!_activeMovement || WorldAreaManager.Instance == null) return;

        if (_slowed)
            _effectiveSpeed = _speed * _slowMultiplier * _cornerMultiplier;
        else
            _effectiveSpeed = _speed * _cornerMultiplier;

            // Always recompute velocity from direction + speed so tweaking _speed works live
            _velocity = _direction * _effectiveSpeed;

        transform.position += (Vector3)_velocity * Time.deltaTime;

        float halfPlayW = WorldAreaManager.Instance.playWidth * 0.5f;
        float halfPlayH = WorldAreaManager.Instance.playHeight * 0.5f;

        float minX = WorldAreaManager.Instance.center.x - halfPlayW + _halfWidth;
        float maxX = WorldAreaManager.Instance.center.x + halfPlayW - _halfWidth;
        float minY = WorldAreaManager.Instance.center.y - halfPlayH + _halfHeight;
        float maxY = WorldAreaManager.Instance.center.y + halfPlayH - _halfHeight;

        Vector3 p = transform.position;

        if (p.x <= minX)
        {
            p.x = minX;
            _direction.x = Mathf.Abs(_direction.x);   // bounce: update direction, not velocity
            SetRandomColor();
            AudioPlayer.PlayOneShot(_pongWallHitSound);
        }
        else if (p.x >= maxX)
        {
            p.x = maxX;
            _direction.x = -Mathf.Abs(_direction.x);
            SetRandomColor();
            AudioPlayer.PlayOneShot(_pongWallHitSound);
        }

        if (p.y <= minY)
        {
            p.y = minY;
            _direction.y = Mathf.Abs(_direction.y);
            SetRandomColor();
            AudioPlayer.PlayOneShot(_pongWallHitSound);
        }
        else if (p.y >= maxY)
        {
            p.y = maxY;
            _direction.y = -Mathf.Abs(_direction.y);
            SetRandomColor();
            AudioPlayer.PlayOneShot(_pongWallHitSound);
        }

        bool nearTopLeft = (p.x < minX + _cornerDetectSize) && (p.y > maxY - _cornerDetectSize);
        bool nearTopRight = (p.x > maxX - _cornerDetectSize) && (p.y > maxY - _cornerDetectSize);
        bool nearBotLeft = (p.x < minX + _cornerDetectSize) && (p.y < minY + _cornerDetectSize);
        bool nearBotRight = (p.x > maxX - _cornerDetectSize) && (p.y < minY + _cornerDetectSize);

        if (nearTopLeft || nearTopRight || nearBotLeft || nearBotRight)
            OnCornerHit();

        transform.position = p;
    }

    public override void Damage(float damage, bool showDamagedParticles = true)
    {
        base.Damage(damage);

        if (_canBeDamaged && showDamagedParticles)
        {
            HapticFeedback.MediumFeedback();
            AudioPlayer.PlayOneShot(_damageSound);
            _hitParticles.Play();
        }
    }

    private void OnCornerHit()
    {
        if (_isHappy) return;
        _isHappy = true;

        if (_happySpriteCoroutine != null)
            StopCoroutine(_happySpriteCoroutine);

        _happySpriteCoroutine = StartCoroutine(ResetAngrySprite());

        _cornerMultiplier = _cornerSpeedMultiplier;
    }

    // ResetAngrySprite — only resets its own multiplier
    private IEnumerator ResetAngrySprite()
    {
        _spriteRenderer.sprite = _happySprite;
        _confettiParticles.Play();
        AudioPlayer.PlayOneShot(_celebrationSound);

        yield return new WaitForSeconds(2.5f);

        _spriteRenderer.sprite = _angrySprite;
        _isHappy = false;
        _cornerMultiplier = 1f;  // slow multiplier is untouched — still active if slowed
    }

    private void SetRandomColor()
    {
        _myColor = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);
        _spriteRenderer.color = _myColor;
        ParticleSystem.MainModule main = _hitParticles.main;
        main.startColor = new ParticleSystem.MinMaxGradient(_myColor);
    }

    public override void BossEntryAnim()
    {
        CameraEffectsManager.Instance.ShakeCamera(.2f, .25f);
        Handheld.Vibrate();
        AudioPlayer.PlayOneShot(_spawnSound);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerManager>(out var player))
            player.Damage();
    }

    public override void Die()
    {
        base.Die();
        GameObject go = Instantiate(_dieParticles, transform.position, Quaternion.identity);

        AudioPlayer.PlayOneShot(_dieSound);
        AudioPlayer.PlayOneShot(_dieSound2);

        ParticleSystem.MainModule main = go.GetComponent<ParticleSystem>().main;
        main.startColor = new ParticleSystem.MinMaxGradient(_myColor);

        CameraEffectsManager.Instance.ShakeCamera(.2f, .25f);
        Handheld.Vibrate();

        Destroy(gameObject);
    }
}
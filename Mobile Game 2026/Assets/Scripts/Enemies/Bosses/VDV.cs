using CandyCoded.HapticFeedback;
using JetBrains.Annotations;
using System.Collections;
using UnityEngine;

public class VDV : BossBase
{
    [Header("Corner Hits")]
    [SerializeField] private float _halfWidth = 0.5f;
    [SerializeField] private float _halfHeight = 0.5f;
    [Space]
    [SerializeField] private float _cornerSpeedMultiplier = 1.25f;
    [SerializeField] private float _cornerDetectSize = .2f;

    //movement
    private float _finalSpeed;
    private Vector2 _velocity;
    private bool _isHappy = false;

    [Header("Visuals")]
    [SerializeField] private Sprite _angrySprite;
    [SerializeField] private Sprite _happySprite;
    [SerializeField] private ParticleSystem _confettiParticles;
    [SerializeField] private ParticleSystem _hitParticles;
    [SerializeField] private AudioClip _celebrationSound;
    [SerializeField] private AudioClip _pongWallHitSound;
    [SerializeField] private AudioClip _additionalDeathSound;
    [SerializeField] private AudioClip _spawnSound;
    [SerializeField] private AudioClip _damageSound;

    private Color _myColor;
    private bool _activeMovement = false;
    private Vector2 _direction;
    private Coroutine _happyRoutine;


    public override void Initialize()
    {
        base.Initialize();

        do
        {
            _direction = Random.insideUnitCircle.normalized;
        }
        while (Mathf.Abs(_direction.y) < 0.25f || Mathf.Abs(_direction.x) < 0.25f);

        _finalSpeed = _effectiveSpeed;
        _velocity = _direction * _finalSpeed;

        _activeMovement = true;
    }

    protected override void Update()
    {
        base.Update();

        if (!_activeMovement || WorldAreaManager.Instance == null) return;

        //use speed buff when happy
        if (_isHappy) _finalSpeed = _effectiveSpeed * _cornerSpeedMultiplier;
        else _finalSpeed = _effectiveSpeed;

        //recompute velocity
        _velocity = _direction * _finalSpeed;

        //move
        transform.position += (Vector3)_velocity * Time.deltaTime;

        //bounce off sides
        CheckSides();
    }

    public override void Damage(float damage, bool useDamageVisuals = true)
    {
        base.Damage(damage, useDamageVisuals);

        if (useDamageVisuals)
        {
            HapticFeedback.MediumFeedback();
            AudioPlayer.PlayOneShot(_damageSound);
        }
    }

    private void CheckSides()
    {
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
            _direction.x = Mathf.Abs(_direction.x);
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

    private void OnCornerHit()
    {
        if (_happyRoutine != null)
            StopCoroutine(_happyRoutine);

        _happyRoutine = StartCoroutine(HappyRoutine());
    }

    private IEnumerator HappyRoutine()
    {
        _isHappy = true;

        //visuals / audio
        SpriteRendr.sprite = _happySprite;
        _confettiParticles.Play();
        AudioPlayer.PlayOneShot(_celebrationSound);

        yield return new WaitForSeconds(2.5f);

        SpriteRendr.sprite = _angrySprite;

        _isHappy = false;
    }

    private void SetRandomColor()
    {
        _myColor = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);
        SpriteRendr.color = _myColor;
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
        //damage player
        if (collision.TryGetComponent<PlayerManager>(out var player))
            player.Damage();
    }

    protected override void Die()
    {
        base.Die();

        AudioPlayer.PlayOneShot(_deathSound);
        AudioPlayer.PlayOneShot(_additionalDeathSound);

        GameObject go = Instantiate(_deathParticlesPrefab, transform.position, Quaternion.identity);
        ParticleSystem.MainModule main = go.GetComponent<ParticleSystem>().main;
        main.startColor = new ParticleSystem.MinMaxGradient(_myColor);

        CameraEffectsManager.Instance.ShakeCamera(.2f, .25f);
        Handheld.Vibrate();

        Destroy(gameObject);
    }
}
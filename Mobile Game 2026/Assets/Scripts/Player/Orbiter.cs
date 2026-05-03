using System.Collections.Generic;
using UnityEngine;

public class Orbiter : MonoBehaviour
{
    [Header("Damage")]
    public float _damageInterval = 0.25f;
    private Dictionary<EnemyHP, float> _timers = new();
    private PlayerManager _playerData;
    private bool _initialized;

    [Header("Effects")]
    [SerializeField] private SpriteRenderer _sr;
    [SerializeField] private ParticleSystem _meteorParticles;

    private bool _meteorActive = false;
    private float _fireDamage;
    private float _fireDuration;

    private bool _iceGiantActive = false;
    private float _iceGiantDuration;
    private float _iceGiantSlowMultiplier;

    public void Initialize(PlayerManager playerData)
    {
        _initialized = true;
        _playerData = playerData;
    }

    public void InitializeMeteor(Sprite meteorSprite, Color meteorColor, float fireDuration, float fireDamage)
    {
        _meteorActive = true;
        _sr.sprite = meteorSprite;

        if(!_iceGiantActive)
            _sr.color = meteorColor;

        _fireDuration = fireDuration;
        _fireDamage = fireDamage;
        if (_meteorParticles != null)
            _meteorParticles.Play();

        transform.Rotate(0f, 0f, Random.Range(0f, 360f));
    }

    public void InitializeIceGiant(Sprite iceGiantSprite, Color iceGiantCOlor, float slowDuration, float slowMultiplier)
    {
        _iceGiantActive = true;

        if (!_meteorActive) _sr.sprite = iceGiantSprite;
        _sr.color = iceGiantCOlor;
        _iceGiantDuration = slowDuration;
        _iceGiantSlowMultiplier = slowMultiplier;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_initialized) return;
        if (collision.TryGetComponent<EnemyHP>(out var enemyHp))
        {
            enemyHp.Damage(_playerData.GetDamage());

            if (_meteorActive)
                enemyHp.SetOnFire(_fireDuration, _fireDamage);
            if (_iceGiantActive)
                enemyHp.Slow(_iceGiantDuration, _iceGiantSlowMultiplier);

            _timers[enemyHp] = 0f;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!_initialized) return;
        if (!collision.TryGetComponent<EnemyHP>(out var enemyHp)) return;

        if (!_timers.ContainsKey(enemyHp))
            _timers[enemyHp] = 0f;

        _timers[enemyHp] += Time.deltaTime;

        if (_timers[enemyHp] >= _damageInterval)
        {
            enemyHp.Damage(_playerData.GetDamage());

            if (_meteorActive)
                enemyHp.SetOnFire(_fireDuration, _fireDamage);
            if (_iceGiantActive)
                enemyHp.Slow(_iceGiantDuration, _iceGiantSlowMultiplier);

            _timers[enemyHp] = 0f;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<EnemyHP>(out var enemyHp))
            _timers.Remove(enemyHp);
    }
}
using CandyCoded.HapticFeedback;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public event Action<EnemyBase> OnDied;

    //general
    [HideInInspector] public bool Initialized = false;
    [HideInInspector] public int SpawnIndex; //controls sorting order

    [Header("HP")]
    [SerializeField] protected float _maxHp;
    [SerializeField] protected float _hp;

    protected bool _canBeDamaged = false; //set to true on initialized

    [Header("HP Visuals")]
    [SerializeField] private TextMeshPro _hpText;
    [SerializeField] protected ParticleSystem _damageParticles;
    [Space]
    [SerializeField] protected GameObject _deathParticlesPrefab;
    [SerializeField] protected AudioClip _deathSound;

    public TextMeshPro HpText { get { return _hpText; }}

    [Header("Movement")]
    [SerializeField] private float _speed = 2f;

    protected float _effectiveSpeed; //actual speed after multipliers

    private float _externalSpeedMultiplier;
    private bool _useExternalSpeedMultiplier = false;

    [Header("Effect Particles / Sounds")]
    public SpriteRenderer SpriteRendr;
    [SerializeField] private ParticleSystem _meteorEffectParticles;
    [SerializeField] private ParticleSystem _iceGiantEffectParticles;
    [SerializeField] private ParticleSystem _electricEffectParticles;
    [Space]
    [SerializeField] private AudioClip _sfxElectricity;

    #region Subscriptions (OnEnable / OnDisable)
    protected virtual void OnEnable()
    {
        PlayerManager.Instance.OnPlayerDied += OnPlayerDied;
    }

    protected virtual void OnDisable()
    {
        PlayerManager.Instance.OnPlayerDied -= OnPlayerDied;

        if (EnemySortManager.Instance)
            EnemySortManager.Instance.UnregisterEnemy(this);
    }

    protected virtual void OnPlayerDied()
    {
        //what to do when player dies
    }

    #endregion

    public virtual void Initialize(int newHp)
    {
        //disable external multipliers just in case
        _useExternalSpeedMultiplier = false;
        _externalSpeedMultiplier = 1f;
        _effectiveSpeed = _speed;

        //stop effect particles in case they were active
        _meteorEffectParticles.Stop();
        _iceGiantEffectParticles.Stop();

        //let player damage enemy
        _hp = newHp;
        _maxHp = newHp;
        _canBeDamaged = true;

        //register into enemy sort manager
        if (EnemySortManager.Instance)
            EnemySortManager.Instance.RegisterEnemy(this);

        Initialized = true;
    }

    protected virtual void Start()
    {
        //set hp text in case we're not initialized
        _hpText.text = ((int)_hp).ToString();
    }

    protected virtual void Update()
    {
        if (!Initialized) return;

        //update hp text, die if no hp
        _hpText.text = ((int)_hp).ToString();
        if (_hp <= 0)
            Die();

        //update effective speed
        if (_useExternalSpeedMultiplier) _effectiveSpeed = _speed * _externalSpeedMultiplier;
        else _effectiveSpeed = _speed;
    }

    #region Effects
    public void SetEffect(SpecialEffect effect)
    {
        switch (effect)
        {
            case SpecialEffect.Meteor:
            case SpecialEffect.MeteorAdvanced:
                _meteorEffectParticles.Play();
                break;
            case SpecialEffect.IceGiant:
            case SpecialEffect.IceGiantAdvanced:
                _iceGiantEffectParticles.Play();
                break;
            case SpecialEffect.Electric:
            case SpecialEffect.ElectricAdvanced:
                _electricEffectParticles.Play();
                AudioPlayer.PlayBulkOneShot(_sfxElectricity, 0.05f);
                break;
        }
    }

    public void ClearEffect(SpecialEffect effect)
    {
        switch (effect)
        {
            case SpecialEffect.Meteor:
            case SpecialEffect.MeteorAdvanced:
                _meteorEffectParticles.Stop();
                break;
            case SpecialEffect.IceGiant:
            case SpecialEffect.IceGiantAdvanced:
                _iceGiantEffectParticles.Stop();
                break;
            case SpecialEffect.Electric:
            case SpecialEffect.ElectricAdvanced:

                break;
        }
    }

    public void SetExternalSpeedMultiplier(float multiplier)
    {
        _externalSpeedMultiplier = multiplier;
        _useExternalSpeedMultiplier = true;
    }

    public void ClearExternalSpeedMultiplier()
    {
        _externalSpeedMultiplier = 1f;
        _useExternalSpeedMultiplier = false;
    }

    #endregion

    #region HP

    public virtual void Damage(float damage, bool useDamageVisuals = true)
    {
        if (!_canBeDamaged) return;
        _hp -= damage;

        if (useDamageVisuals)
            _damageParticles.Play();
    }

    protected virtual void Die()
    {
        InvokeOnDiedEvent();

        //create death particles with current sprite's color
        GameObject dp = ParticlePool.Instance.Spawn(_deathParticlesPrefab, transform.position, transform.rotation);

        var ps = dp.GetComponent<ParticleSystem>();
        var main = ps.main;
        main.startColor = new ParticleSystem.MinMaxGradient(SpriteRendr.color);

        //vibrate
        HapticFeedback.MediumFeedback();

        //play death sound
        if (AudioPlayer.Instance != null)
            AudioPlayer.PlayBulkOneShot(_deathSound, .1f);

        //award player xp
        PlayerManager.Instance?.AddXP(_maxHp);

        //de initialize
        Initialized = false;

        //unregister from sort manager
        if (EnemySortManager.Instance)
            EnemySortManager.Instance.UnregisterEnemy(this);

        //return to enemy object pool
        EnemySpawner.Instance.ReturnEnemyToPool(this);
    }

    protected void InvokeOnDiedEvent()
    {
        OnDied?.Invoke(this);
        OnDied = null; // clear all subscribers so nothing lingers
    }
    #endregion
}
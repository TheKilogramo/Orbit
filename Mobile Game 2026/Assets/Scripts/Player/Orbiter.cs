using System;
using System.Collections.Generic;
using UnityEngine;

public class Orbiter : MonoBehaviour
{
    public event Action<EnemyBase> OnHit;

    [Header("Damage")]
    public float _damageInterval = 0.25f;
    private Dictionary<EnemyBase, float> _timers = new();
    private PlayerManager _playerData;
    private bool _initialized;

    [Header("Effect Visuals")]
    [SerializeField] private SpriteRenderer _sr;
    [SerializeField] private ParticleSystem _meteorParticles;
    [SerializeField] private ParticleSystem _electricParticles;

    [Header("Sprites")]
    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private Sprite _meteorSprite;
    [SerializeField] private Sprite _iceGiantSprite;

    [Header("Colors")]
    [SerializeField] private Color _defaultColor = Color.white;
    [SerializeField] private Color _meteorColor;
    [SerializeField] private Color _iceGiantColor;

    //tracking effects
    private List<SpecialEffect> _activeEffects = new List<SpecialEffect>();


    public void Initialize(PlayerManager playerData)
    {
        _initialized = true;
        _playerData = playerData;

        //set random rotation
        transform.Rotate(0f, 0f, UnityEngine.Random.Range(0f, 360f));

        // apply any currently active effects
        foreach (SpecialEffect effect in EffectsManager.Instance.ActiveEffects)
            SetEffect(effect);
    }

    public void SetEffect(SpecialEffect effect)
    {
        switch (effect)
        {
            case SpecialEffect.Meteor:
                _meteorParticles.Play();
                break;
            case SpecialEffect.MeteorAdvanced:
                _meteorParticles.Play();
                break;
            case SpecialEffect.IceGiant:
                break;
            case SpecialEffect.IceGiantAdvanced:
                break;
            case SpecialEffect.Electric:
                _electricParticles.Play();
                break;
            case SpecialEffect.ElectricAdvanced:
                _electricParticles.Play();
                break;
            case SpecialEffect.Saturn:
                break;
            case SpecialEffect.SaturnAdvanced:
                break;
            case SpecialEffect.SolarFlare:
                break;
            case SpecialEffect.SolarFlareAdvanced:
                break;
            case SpecialEffect.Comet:
                break;
            case SpecialEffect.CometAdvanced:
                break;
            case SpecialEffect.Gravity:
                break;
            case SpecialEffect.GravityAdvanced:
                break;
            case SpecialEffect.Lightsaber:
                break;
            case SpecialEffect.LightsaberAdvanced:
                break;
            case SpecialEffect.Smash:
                break;
            case SpecialEffect.SmashAdvanced:
                break;
            case SpecialEffect.FrozenLightning:
                break;
            case SpecialEffect.FrozenLightningAdvanced:
                break;
        }

        if (!_activeEffects.Contains(effect))
            _activeEffects.Add(effect);

        UpdateSprite();
    }

    public void RemoveEffect(SpecialEffect effect)
    {
        switch (effect)
        {
            case SpecialEffect.Meteor:
                _meteorParticles.Stop();
                break;
            case SpecialEffect.MeteorAdvanced:
                _meteorParticles.Stop();
                break;
            case SpecialEffect.IceGiant:
                break;
            case SpecialEffect.IceGiantAdvanced:
                break;
            case SpecialEffect.Electric:
                _electricParticles.Stop();
                break;
            case SpecialEffect.ElectricAdvanced:
                _electricParticles.Stop();
                break;
            case SpecialEffect.Saturn:
                break;
            case SpecialEffect.SaturnAdvanced:
                break;
            case SpecialEffect.SolarFlare:
                break;
            case SpecialEffect.SolarFlareAdvanced:
                break;
            case SpecialEffect.Comet:
                break;
            case SpecialEffect.CometAdvanced:
                break;
            case SpecialEffect.Gravity:
                break;
            case SpecialEffect.GravityAdvanced:
                break;
            case SpecialEffect.Lightsaber:
                break;
            case SpecialEffect.LightsaberAdvanced:
                break;
            case SpecialEffect.Smash:
                break;
            case SpecialEffect.SmashAdvanced:
                break;
            case SpecialEffect.FrozenLightning:
                break;
            case SpecialEffect.FrozenLightningAdvanced:
                break;
        }

        _activeEffects.Remove(effect);
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        // sprite priority: meteor > iceGiant > default
        if (_activeEffects.Contains(SpecialEffect.Meteor) || _activeEffects.Contains(SpecialEffect.MeteorAdvanced))
        {
            _sr.sprite = _meteorSprite;
        }
        else if (_activeEffects.Contains(SpecialEffect.IceGiant) || _activeEffects.Contains(SpecialEffect.IceGiantAdvanced))
        {
            _sr.sprite = _iceGiantSprite;
        }
        else
        {
            _sr.sprite = _defaultSprite;
        }

        // color priority: iceGiant > meteor > default
        if (_activeEffects.Contains(SpecialEffect.IceGiant) || _activeEffects.Contains(SpecialEffect.IceGiantAdvanced))
        {
            _sr.color = _iceGiantColor;
        }
        else if (_activeEffects.Contains(SpecialEffect.Meteor) || _activeEffects.Contains(SpecialEffect.MeteorAdvanced))
        {
            _sr.color = _meteorColor;
        }
        else
        {
            _sr.color = _defaultColor;
        }
    }


    #region Collision Checks
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_initialized) return;
        if (collision.TryGetComponent<EnemyBase>(out var enemyHp))
        {
            DamageEnemy(enemyHp);

            _timers[enemyHp] = 0f;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!_initialized) return;
        if (!collision.TryGetComponent<EnemyBase>(out var enemyHp)) return;

        if (!_timers.ContainsKey(enemyHp))
            _timers[enemyHp] = 0f;

        _timers[enemyHp] += Time.deltaTime;

        if (_timers[enemyHp] >= _damageInterval)
        {
            DamageEnemy(enemyHp);

            _timers[enemyHp] = 0f;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<EnemyBase>(out var enemyHp))
            _timers.Remove(enemyHp);
    }

    #endregion

    private void DamageEnemy(EnemyBase enemy)
    {
        enemy.Damage(_playerData.GetDamage());

        //call onhit event
        OnHit?.Invoke(enemy); //adds whatever effect we need to the enemy
    }
}
using System.Collections.Generic;
using UnityEngine;
using static Upgrade;

public class EffectsManager : MonoBehaviour
{
    public static EffectsManager Instance;
    [SerializeField] private List<EffectBase> _activeEffects = new();

    [Header("Effects")]
    [SerializeField] private EffectBase _saturn;
    [SerializeField] private EffectBase _saturnAdvanced;
    [SerializeField] private EffectBase _meteor;
    [SerializeField] private EffectBase _meteorAdvanced;
    [SerializeField] private EffectBase _electric;
    [SerializeField] private EffectBase _electricAdvanced;
    [SerializeField] private EffectBase _iceGiant;
    [SerializeField] private EffectBase _iceGiantAdvanced;
    [SerializeField] private EffectBase _solarFlare;
    [SerializeField] private EffectBase _solarFlareAdvanced;
    [SerializeField] private EffectBase _comet;
    [SerializeField] private EffectBase _cometAdvanced;
    [SerializeField] private EffectBase _gravity;
    [SerializeField] private EffectBase _gravityAdvanced;
    [SerializeField] private EffectBase _lightsaber;
    [SerializeField] private EffectBase _lightsaberAdvanced;
    [SerializeField] private EffectBase _smash;
    [SerializeField] private EffectBase _smashAdvanced;
    [SerializeField] private EffectBase _frozenLighting;
    [SerializeField] private EffectBase _frozenLightingAdvanced;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        foreach(EffectBase e in _activeEffects)
        {
            e.Initialize();
        }

        PlayerManager.Instance.OnPlayerDied += PlayerDied;
    }

    private void OnDisable()
    {
        PlayerManager.Instance.OnPlayerDied -= PlayerDied;
    }

    private void PlayerDied()
    {
        enabled = false;
    }

    private void Update()
    {
        foreach (var effect in _activeEffects)
            effect.OnUpdate();
    }

    public void ApplyEffect(SpecialEffect effect)
    {
        switch (effect)
        {
            case SpecialEffect.Saturn: Activate(_saturn); break;
            case SpecialEffect.SaturnAdvanced: Upgrade(_saturn, _saturnAdvanced); break;
            case SpecialEffect.Meteor: Activate(_meteor); break;
            case SpecialEffect.MeteorAdvanced: Upgrade(_meteor, _meteorAdvanced); break;
            case SpecialEffect.Electric: Activate(_electric); break;
            case SpecialEffect.ElectricAdvanced: Upgrade(_electric, _electricAdvanced); break;
            case SpecialEffect.IceGiant: Activate(_iceGiant); break;
            case SpecialEffect.IceGiantAdvanced: Upgrade(_iceGiant, _iceGiantAdvanced); break;
            case SpecialEffect.SolarFlare: Activate(_solarFlare); break;
            case SpecialEffect.SolarFlareAdvanced: Upgrade(_solarFlare, _solarFlareAdvanced); break;
            case SpecialEffect.Comet: Activate(_comet); break;
            case SpecialEffect.CometAdvanced: Upgrade(_comet, _cometAdvanced); break;
            case SpecialEffect.Gravity: Activate(_gravity); break;
            case SpecialEffect.GravityAdvanced: Upgrade(_gravity, _gravityAdvanced); break;
            case SpecialEffect.Lightsaber: Activate(_lightsaber); break;
            case SpecialEffect.LightsaberAdvanced: Upgrade(_lightsaber, _lightsaberAdvanced); break;
            case SpecialEffect.Smash: Activate(_smash); break;
            case SpecialEffect.SmashAdvanced: Upgrade(_smash, _smashAdvanced); break;
            case SpecialEffect.FrozenLightning: Activate(_frozenLighting); break;
            case SpecialEffect.FrozenLightningAdvanced: Upgrade(_frozenLighting, _frozenLightingAdvanced); break;
        }
    }

    private void Activate(EffectBase effect)
    {
        _activeEffects.Add(effect);
        effect.Initialize();
    }

    private void Upgrade(EffectBase medium, EffectBase advanced)
    {
        if (_activeEffects.Contains(medium))
        {
            medium.Disable();
            _activeEffects.Remove(medium);
        }

        _activeEffects.Add(advanced);
        advanced.Initialize();
    }
}
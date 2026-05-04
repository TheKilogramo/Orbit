using System.Collections.Generic;
using UnityEngine;
using static Upgrade;

public class EffectsManager : MonoBehaviour
{
    public static EffectsManager Instance;
    [SerializeField] private List<EffectBase> _activeEffects = new();

    [HideInInspector] public List<SpecialEffect> ActiveEffects = new();

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
            case SpecialEffect.Saturn: Activate(_saturn, SpecialEffect.Saturn); break;
            case SpecialEffect.SaturnAdvanced: Upgrade(_saturn, _saturnAdvanced, SpecialEffect.Saturn, SpecialEffect.SaturnAdvanced); break;
            case SpecialEffect.Meteor: Activate(_meteor, SpecialEffect.Meteor); break;
            case SpecialEffect.MeteorAdvanced: Upgrade(_meteor, _meteorAdvanced, SpecialEffect.Meteor, SpecialEffect.MeteorAdvanced); break;
            case SpecialEffect.Electric: Activate(_electric, SpecialEffect.Electric); break;
            case SpecialEffect.ElectricAdvanced: Upgrade(_electric, _electricAdvanced, SpecialEffect.Electric, SpecialEffect.ElectricAdvanced); break;
            case SpecialEffect.IceGiant: Activate(_iceGiant, SpecialEffect.IceGiant); break;
            case SpecialEffect.IceGiantAdvanced: Upgrade(_iceGiant, _iceGiantAdvanced, SpecialEffect.IceGiant, SpecialEffect.IceGiantAdvanced); break;
            case SpecialEffect.SolarFlare: Activate(_solarFlare, SpecialEffect.SolarFlare); break;
            case SpecialEffect.SolarFlareAdvanced: Upgrade(_solarFlare, _solarFlareAdvanced, SpecialEffect.SolarFlare, SpecialEffect.SolarFlareAdvanced); break;
            case SpecialEffect.Comet: Activate(_comet, SpecialEffect.Comet); break;
            case SpecialEffect.CometAdvanced: Upgrade(_comet, _cometAdvanced, SpecialEffect.Comet, SpecialEffect.CometAdvanced); break;
            case SpecialEffect.Gravity: Activate(_gravity, SpecialEffect.Gravity); break;
            case SpecialEffect.GravityAdvanced: Upgrade(_gravity, _gravityAdvanced, SpecialEffect.Gravity, SpecialEffect.GravityAdvanced); break;
            case SpecialEffect.Lightsaber: Activate(_lightsaber, SpecialEffect.Lightsaber); break;
            case SpecialEffect.LightsaberAdvanced: Upgrade(_lightsaber, _lightsaberAdvanced, SpecialEffect.Lightsaber, SpecialEffect.LightsaberAdvanced); break;
            case SpecialEffect.Smash: Activate(_smash, SpecialEffect.Smash); break;
            case SpecialEffect.SmashAdvanced: Upgrade(_smash, _smashAdvanced, SpecialEffect.Smash, SpecialEffect.SmashAdvanced); break;
            case SpecialEffect.FrozenLightning: Activate(_frozenLighting, SpecialEffect.FrozenLightning); break;
            case SpecialEffect.FrozenLightningAdvanced: Upgrade(_frozenLighting, _frozenLightingAdvanced, SpecialEffect.FrozenLightning, SpecialEffect.FrozenLightningAdvanced); break;
        }
    }

    private void Activate(EffectBase effect, SpecialEffect effectType)
    {
        _activeEffects.Add(effect);
        ActiveEffects.Add(effectType);
        effect.Initialize();
    }

    private void Upgrade(EffectBase medium, EffectBase advanced, SpecialEffect mediumType, SpecialEffect advancedType)
    {
        if (_activeEffects.Contains(medium))
        {
            medium.Disable();
            _activeEffects.Remove(medium);
            ActiveEffects.Remove(mediumType);
        }

        _activeEffects.Add(advanced);
        ActiveEffects.Add(advancedType);
        advanced.Initialize();
    }
}
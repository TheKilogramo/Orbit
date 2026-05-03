using System.Collections.Generic;
using UnityEngine;

public class IceGiantAdvancedEffect : EffectBase
{
    [SerializeField] private bool _debug = true;

    [Header("Ice Giant Settings")]
    [SerializeField] private Sprite _iceGiantSprite;
    [SerializeField] private Color _iceGiantColor;
    [SerializeField] private float _slowMultiplier;
    [SerializeField] private float _slowDuration;

    public override void Initialize()
    {
        if (_debug) Debug.Log("Initialized IceGiantAdvancedEffect");

        // initialize existing orbiters
        foreach (var orbit in OrbitManager.Instance.GetOrbits())
            foreach (var orbiter in orbit.GetOrbiters())
                orbiter.InitializeIceGiant(_iceGiantSprite, _iceGiantColor, _slowDuration, _slowMultiplier);

        // subscribe for future orbiters
        OrbitManager.Instance.OnOrbiterAdded += OnNewOrbiterAdded;
    }

    private void OnNewOrbiterAdded(Orbiter orbiter)
    {
        orbiter.InitializeIceGiant(_iceGiantSprite, _iceGiantColor, _slowDuration, _slowMultiplier);
    }

    public override void OnUpdate()
    {
        if (_debug) Debug.Log("Updated IceGiantAdvancedEffect");
    }

    public override void Disable()
    {
        if (_debug) Debug.Log("Disabled IceGiantAdvancedEffect");
        OrbitManager.Instance.OnOrbiterAdded -= OnNewOrbiterAdded;
    }
}
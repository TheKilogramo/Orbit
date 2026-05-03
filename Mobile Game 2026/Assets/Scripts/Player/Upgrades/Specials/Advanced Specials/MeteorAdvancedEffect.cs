using System.Collections.Generic;
using UnityEngine;

public class MeteorAdvancedEffect : EffectBase
{
    [SerializeField] private bool _debug = true;

    [Header("Meteor Settings")]
    [SerializeField] private Sprite _meteorSprite;
    [SerializeField] private Color _meteorColor;
    [SerializeField] private float _fireDamagePerSecond;
    [SerializeField] private float _fireDuration;

    public override void Initialize()
    {
        if (_debug) Debug.Log("Initialized Advanced Meteor");

        // initialize existing orbiters
        foreach (var orbit in OrbitManager.Instance.GetOrbits())
            foreach (var orbiter in orbit.GetOrbiters())
                orbiter.InitializeMeteor(_meteorSprite, _meteorColor, _fireDuration, _fireDamagePerSecond);

        // subscribe for future orbiters
        OrbitManager.Instance.OnOrbiterAdded += OnNewOrbiterAdded;
    }

    private void OnNewOrbiterAdded(Orbiter orbiter)
    {
        orbiter.InitializeMeteor(_meteorSprite, _meteorColor, _fireDuration, _fireDamagePerSecond);
    }

    public override void OnUpdate()
    {
        if (_debug) Debug.Log("Updated Advanced Meteor");
    }

    public override void Disable()
    {
        if (_debug) Debug.Log("Disabled Advanced Meteor");
        OrbitManager.Instance.OnOrbiterAdded -= OnNewOrbiterAdded;
    }
}
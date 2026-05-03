using UnityEngine;

public class SolarFlareAdvancedEffect : EffectBase
{
    [SerializeField] private bool _debug = true;

    public override void Initialize()
    {
        if (_debug) Debug.Log("Initialized Solar Flare Advanced");
    }

    public override void OnUpdate()
    {
        if (_debug) Debug.Log("Updated Solar Flare Advanced");
    }

    public override void Disable()
    {
        if (_debug) Debug.Log("Disabled Solar Flare Advanced");
    }
}

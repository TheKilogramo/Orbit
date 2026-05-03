using UnityEngine;

public class IceGiantAdvancedEffect : EffectBase
{
    [SerializeField] private bool _debug = true;

    public override void Initialize()
    {
        if (_debug) Debug.Log("Initialized Ice Giant Advanced");
    }

    public override void OnUpdate()
    {
        if (_debug) Debug.Log("Updated Ice Giant Advanced");
    }

    public override void Disable()
    {
        if (_debug) Debug.Log("Disabled Ice Giant Advanced");
    }
}

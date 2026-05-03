using UnityEngine;

public class SaturnAdvancedEffect : EffectBase
{
    [SerializeField] private bool _debug = true;

    public override void Initialize()
    {
        if (_debug) Debug.Log("Initialized Saturn Advanced");
    }

    public override void OnUpdate()
    {
        if (_debug) Debug.Log("Updated Saturn Advanced");
    }

    public override void Disable()
    {
        if (_debug) Debug.Log("Disabled Saturn Advanced");
    }
}

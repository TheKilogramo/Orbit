using UnityEngine;

public class ElectricAdvancedEffect : EffectBase
{
    [SerializeField] private bool _debug = true;

    public override void Initialize()
    {
        if (_debug) Debug.Log("Initialized Electric Advanced");
    }

    public override void OnUpdate()
    {
        if (_debug) Debug.Log("Updated Electric Advanced");
    }

    public override void Disable()
    {
        if (_debug) Debug.Log("Disabled Electric Advanced");
    }
}

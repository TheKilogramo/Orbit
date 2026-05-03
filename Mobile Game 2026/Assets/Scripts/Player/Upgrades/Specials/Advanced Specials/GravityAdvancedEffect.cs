using UnityEngine;

public class GravityAdvancedEffect : EffectBase
{
    [SerializeField] private bool _debug = true;

    public override void Initialize()
    {
        if (_debug) Debug.Log("Initialized Gravity Advanced");
    }

    public override void OnUpdate()
    {
        if (_debug) Debug.Log("Updated Gravity Advanced");
    }

    public override void Disable()
    {
        if (_debug) Debug.Log("Disabled Gravity Advanced");
    }
}

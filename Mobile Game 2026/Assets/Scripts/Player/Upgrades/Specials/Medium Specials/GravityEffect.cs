using UnityEngine;

public class GravityEffect : EffectBase
{
    [SerializeField] private bool _debug = true;

    public override void Initialize()
    {
        if (_debug) Debug.Log("Initialized Gravity");
    }

    public override void OnUpdate()
    {
        if (_debug) Debug.Log("Updated Gravity");
    }

    public override void Disable()
    {
        if (_debug) Debug.Log("Disabled Gravity");
    }
}

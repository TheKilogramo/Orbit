using UnityEngine;

public class SaturnEffect : EffectBase
{
    [SerializeField] private bool _debug = true;

    public override void Initialize()
    {
        if (_debug) Debug.Log("Initialized Saturn");
    }

    public override void OnUpdate()
    {
        if (_debug) Debug.Log("Updated Saturn");
    }

    public override void Disable()
    {
        if (_debug) Debug.Log("Disabled Saturn");
    }
}

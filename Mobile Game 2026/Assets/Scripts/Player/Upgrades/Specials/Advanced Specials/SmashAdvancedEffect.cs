using UnityEngine;

public class SmashAdvancedEffect : EffectBase
{
    [SerializeField] private bool _debug = true;

    public override void Initialize()
    {
        if (_debug) Debug.Log("Initialized Smash Advanced");
    }

    public override void OnUpdate()
    {
        if (_debug) Debug.Log("Updated Smash Advanced");
    }

    public override void Disable()
    {
        if (_debug) Debug.Log("Disabled Smash Advanced");
    }
}

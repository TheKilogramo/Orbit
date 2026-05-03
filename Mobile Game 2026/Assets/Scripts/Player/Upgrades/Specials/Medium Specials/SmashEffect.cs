using UnityEngine;

public class SmashEffect : EffectBase
{
    [SerializeField] private bool _debug = true;

    public override void Initialize()
    {
        if (_debug) Debug.Log("Initialized Smash");
    }

    public override void OnUpdate()
    {
        if (_debug) Debug.Log("Updated Smash");
    }

    public override void Disable()
    {
        if (_debug) Debug.Log("Disabled Smash");
    }
}

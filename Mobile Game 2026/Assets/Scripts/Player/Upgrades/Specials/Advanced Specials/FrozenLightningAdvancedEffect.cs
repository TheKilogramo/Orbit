using UnityEngine;

public class FrozenLightningAdvancedEffect : EffectBase
{
    [SerializeField] private bool _debug = true;

    public override void Initialize()
    {
        if (_debug) Debug.Log("Initialized Frozen Lightning Advanced");
    }

    public override void OnUpdate()
    {
        if (_debug) Debug.Log("Updated Frozen Lightning Advanced");
    }

    public override void Disable()
    {
        if (_debug) Debug.Log("Disabled Frozen Lightning Advanced");
    }
}

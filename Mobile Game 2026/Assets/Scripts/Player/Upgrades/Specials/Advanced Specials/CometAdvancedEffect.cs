using UnityEngine;

public class CometAdvancedEffect : EffectBase
{
    [SerializeField] private bool _debug = true;

    public override void Initialize()
    {
        if (_debug) Debug.Log("Initialized Comet Advanced");
    }

    public override void OnUpdate()
    {
        if (_debug) Debug.Log("Updated Comet Advanced");
    }

    public override void Disable()
    {
        if (_debug) Debug.Log("Disabled Comet Advanced");
    }
}

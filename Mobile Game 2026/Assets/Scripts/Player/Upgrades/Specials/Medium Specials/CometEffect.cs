using UnityEngine;

public class CometEffect : EffectBase
{
    [SerializeField] private bool _debug = true;

    public override void Initialize()
    {
        if (_debug) Debug.Log("Initialized Comet");
    }

    public override void OnUpdate()
    {
        if (_debug) Debug.Log("Updated Comet");
    }

    public override void Disable()
    {
        if (_debug) Debug.Log("Disabled Comet");
    }
}

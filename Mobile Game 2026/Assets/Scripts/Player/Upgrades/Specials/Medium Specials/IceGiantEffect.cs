using UnityEngine;

public class IceGiantEffect : EffectBase
{
    [SerializeField] private bool _debug = true;

    public override void Initialize()
    {
        if (_debug) Debug.Log("Initialized Ice Giant");
    }

    public override void OnUpdate()
    {
        if (_debug) Debug.Log("Updated Ice Giant");
    }

    public override void Disable()
    {
        if (_debug) Debug.Log("Disabled Ice Giant");
    }
}

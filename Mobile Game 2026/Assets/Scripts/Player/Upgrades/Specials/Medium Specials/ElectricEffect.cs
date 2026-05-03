using UnityEngine;

public class ElectricEffect : EffectBase
{
    [SerializeField] private bool _debug = true;

    public override void Initialize()
    {
        if (_debug) Debug.Log("Initialized Electric");
    }

    public override void OnUpdate()
    {
        if (_debug) Debug.Log("Updated Electric");
    }

    public override void Disable()
    {
        if (_debug) Debug.Log("Disabled Electric");
    }
}

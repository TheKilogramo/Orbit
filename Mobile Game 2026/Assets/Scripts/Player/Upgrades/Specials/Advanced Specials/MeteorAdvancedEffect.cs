using UnityEngine;

public class MeteorAdvancedEffect : EffectBase
{
    [SerializeField] private bool _debug = true;

    public override void Initialize()
    {
        if (_debug) Debug.Log("Initialized Meteor Advanced");
    }

    public override void OnUpdate()
    {
        if (_debug) Debug.Log("Updated Meteor Advanced");
    }

    public override void Disable()
    {
        if (_debug) Debug.Log("Disabled Meteor Advanced");
    }
}

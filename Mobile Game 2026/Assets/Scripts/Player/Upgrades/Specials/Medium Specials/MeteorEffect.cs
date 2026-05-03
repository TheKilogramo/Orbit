using UnityEngine;

public class MeteorEffect : EffectBase
{
    [SerializeField] private bool _debug = true;

    public override void Initialize()
    {
        if (_debug) Debug.Log("Initialized Meteor");
    }

    public override void OnUpdate()
    {
        if (_debug) Debug.Log("Updated Meteor");
    }

    public override void Disable()
    {
        if (_debug) Debug.Log("Disabled Meteor");
    }
}

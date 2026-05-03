using UnityEngine;

public class LightsaberAdvancedEffect : EffectBase
{
    [SerializeField] private bool _debug = true;

    public override void Initialize()
    {
        if (_debug) Debug.Log("Initialized Lightsaber Advanced");
    }

    public override void OnUpdate()
    {
        if (_debug) Debug.Log("Updated Lightsaber Advanced");
    }

    public override void Disable()
    {
        if (_debug) Debug.Log("Disabled Lightsaber Advanced");
    }
}

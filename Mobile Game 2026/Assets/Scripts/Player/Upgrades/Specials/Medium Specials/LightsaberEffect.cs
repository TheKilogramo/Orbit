using UnityEngine;

public class LightsaberEffect : EffectBase
{
    [SerializeField] private bool _debug = true;

    public override void Initialize()
    {
        if (_debug) Debug.Log("Initialized Lightsaber");
    }

    public override void OnUpdate()
    {
        if (_debug) Debug.Log("Updated Lightsaber");
    }

    public override void Disable()
    {
        if (_debug) Debug.Log("Disabled Lightsaber");
    }
}

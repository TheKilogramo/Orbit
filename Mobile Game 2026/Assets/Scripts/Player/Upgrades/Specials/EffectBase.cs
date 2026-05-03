using UnityEngine;

public abstract class EffectBase : MonoBehaviour
{
    public virtual void Initialize() { }
    public virtual void OnUpdate() { }
    public virtual void Disable() { }
}
using UnityEngine;

public abstract class EffectBase : MonoBehaviour
{

    private void Start()
    {
        PlayerManager.Instance.OnPlayerDied += OnPlayerDied;
    }

    private void OnPlayerDied()
    {
        enabled = false;
    }

    private void OnDisable()
    {
        PlayerManager.Instance.OnPlayerDied -= OnPlayerDied;
    }

    public virtual void Initialize() { }
    public virtual void OnUpdate() { }
    public virtual void Disable() { }
}
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ScaleLight2D : MonoBehaviour
{
    [SerializeField] private float _defaultValue = 0.15f;
    [SerializeField] private Transform _scaleReference;
    [SerializeField] private Light2D _light2D;

    private float _initialScale;

    private void Start()
    {
        _initialScale = _scaleReference.localScale.x;
    }

    private void Update()
    {
        float scaleMultiplier = _scaleReference.localScale.x / _initialScale;
        _light2D.pointLightOuterRadius = _defaultValue * scaleMultiplier;
    }
}
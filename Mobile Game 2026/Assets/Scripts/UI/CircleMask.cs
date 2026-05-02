using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

[ExecuteAlways]
public class CircleMask : Image
{
    [Tooltip("Invert mask stencil comparison (NotEqual).")]
    public bool inverted = false;

    private Material _runtimeMaterial;

    protected override void OnEnable()
    {
        base.OnEnable();
        RebuildMaterial();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (_runtimeMaterial != null)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(_runtimeMaterial);
            else
                Destroy(_runtimeMaterial);
#else
            Destroy(_runtimeMaterial);
#endif
            _runtimeMaterial = null;
        }
    }

    protected override void OnCanvasHierarchyChanged()
    {
        base.OnCanvasHierarchyChanged();
        RebuildMaterial();
    }

    private void RebuildMaterial()
    {
        // destroy old material safely
        if (_runtimeMaterial != null)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(_runtimeMaterial);
            else
                Destroy(_runtimeMaterial);
#else
            Destroy(_runtimeMaterial);
#endif
        }

        // IMPORTANT: build from shader, not base.materialForRendering
        Shader uiShader = Shader.Find("UI/Default");
        _runtimeMaterial = new Material(uiShader);
        _runtimeMaterial.hideFlags = HideFlags.HideAndDontSave;  // <- FIXES BUILD ERROR

        ApplyStencilMode();
        UpdateMaterial();
    }

    private void ApplyStencilMode()
    {
        _runtimeMaterial.SetInt("_StencilComp", inverted ?
            (int)CompareFunction.NotEqual :
            (int)CompareFunction.Equal);
    }

    public override Material materialForRendering
    {
        get
        {
            if (_runtimeMaterial != null)
                ApplyStencilMode();

            return _runtimeMaterial;
        }
    }
}

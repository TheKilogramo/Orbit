Shader "Unlit/InfiniteGrid2D_QuadSafe"
{
    Properties
    {
        _MinorColour("Minor Grid Colour", Color) = (.7, .7, .7, 1)
        _MajorColour("Major Grid Colour", Color) = (1, 1, 1, 1)

        _GridSize("Minor Grid Spacing (world units)", Float) = 1.0
        _MajorGridMultiplier("Major Line Every N Cells", Float) = 10

        _MinorThickness("Minor Line Thickness (px)", Float) = 1.0
        _MajorThickness("Major Line Thickness (px)", Float) = 2.0

        _FadeStart("Minor Fade Start Zoom", Float) = 15
        _FadeEnd("Minor Fade End Zoom", Float) = 30

        _CameraOrthographicSize("Camera Ortho Size", Float) = 5
        _CameraWorldPos("Camera World Pos (XY)", Vector) = (0, 0, 0, 0)

        _Alpha("Alpha", Range(0,1)) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue"="Background"
            "RenderType"="Transparent"
        }

        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _MinorColour;
            float4 _MajorColour;

            float _GridSize;
            float _MajorGridMultiplier;
            float _MinorThickness;
            float _MajorThickness;

            float _FadeStart;
            float _FadeEnd;

            float _CameraOrthographicSize;
            float4 _CameraWorldPos;

            float _Alpha;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                float3 wp = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldPos = wp;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float GridLineDist(float coord, float cell)
            {
                float x = frac(coord / cell);
                return min(x, 1 - x);
            }

            float LineMask(float dist, float worldThickness)
            {
                return 1 - smoothstep(0, worldThickness, dist);
            }

            float3 RotateWorldToObject(float3 worldPos)
{
    // Extract rotation-only 3x3 matrix from unity_WorldToObject.
    // Normalize each row to remove scale.
    float3x3 rot;
    rot[0] = normalize(unity_WorldToObject._m00_m01_m02);
    rot[1] = normalize(unity_WorldToObject._m10_m11_m12);
    rot[2] = normalize(unity_WorldToObject._m20_m21_m22);

    // Apply rotation (translation removed)
    return mul(rot, worldPos - unity_ObjectToWorld._m03_m13_m23);
}

            float4 frag(v2f IN) : SV_Target
            {
// Rotate worldPos into object-rotated space, without scaling it
float3 local = RotateWorldToObject(IN.worldPos);

// Use rotated X/Y for grid sampling
float2 wp = local.xy;

                float worldPerPixel = (2.0 * _CameraOrthographicSize) / _ScreenParams.y;

                float minorT = _MinorThickness * worldPerPixel;
                float majorT = _MajorThickness * worldPerPixel;

                float minorMask = max(
                    LineMask(GridLineDist(wp.x, _GridSize), minorT),
                    LineMask(GridLineDist(wp.y, _GridSize), minorT)
                );

                float majorSpacing = _GridSize * _MajorGridMultiplier;

                float majorMask = max(
                    LineMask(GridLineDist(wp.x, majorSpacing), majorT),
                    LineMask(GridLineDist(wp.y, majorSpacing), majorT)
                );

                float zoom = _CameraOrthographicSize;
                float fadeT = saturate((zoom - _FadeStart) / (_FadeEnd - _FadeStart));
                minorMask *= (1 - fadeT);

                float4 col =
                    _MinorColour * minorMask +
                    _MajorColour * majorMask;

                col.a = max(minorMask, majorMask) * _Alpha;

                return col;
            }

            ENDCG
        }
    }
}

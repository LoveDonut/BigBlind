Shader "Custom/GoodCircle"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Radius ("Radius", Range(0, 0.5)) = 0.4
        _OutlineWidth ("Outline Width", Range(0, 0.1)) = 0.02
        _WrinkleAmount ("Wrinkle Amount", Range(0, 0.1)) = 0.02
        _WrinkleFrequency ("Wrinkle Frequency", Float) = 20
        _NoiseScale ("Noise Scale", Float) = 10
        _NoiseSpeed ("Noise Speed", Float) = 0.5
    }
    
    SubShader
    {
        Tags {"RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline"="UniversalPipeline"}
        
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        ENDHLSL

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                float _Radius;
                float _OutlineWidth;
                float _WrinkleAmount;
                float _WrinkleFrequency;
                float _NoiseScale;
                float _NoiseSpeed;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            float2 unity_gradientNoise_dir(float2 p)
            {
                p = p % 289;
                float x = (34 * p.x + 1) * p.x % 289 + p.y;
                x = (34 * x + 1) * x % 289;
                x = frac(x / 41) * 2 - 1;
                return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
            }

            float unity_gradientNoise(float2 p)
            {
                float2 ip = floor(p);
                float2 fp = frac(p);
                float d00 = dot(unity_gradientNoise_dir(ip), fp);
                float d01 = dot(unity_gradientNoise_dir(ip + float2(0, 1)), fp - float2(0, 1));
                float d10 = dot(unity_gradientNoise_dir(ip + float2(1, 0)), fp - float2(1, 0));
                float d11 = dot(unity_gradientNoise_dir(ip + float2(1, 1)), fp - float2(1, 1));
                fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
                return lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x);
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv - 0.5;
                float time = _Time.y * _NoiseSpeed;
                
                float noise = unity_gradientNoise(uv * _NoiseScale + time) * 2 - 1;
                float angle = atan2(uv.y, uv.x);
                float wrinkle = sin(angle * _WrinkleFrequency + noise * 10) * _WrinkleAmount;
                
                float dist = length(uv);
                float outerEdge = _Radius + _OutlineWidth / 2 + wrinkle;
                float innerEdge = _Radius - _OutlineWidth / 2 + wrinkle;
                
                float outline = step(dist, outerEdge) - step(dist, innerEdge);
                
                half4 col = _Color;
                col.a *= outline;
                return col;
            }
            ENDHLSL
        }
    }
}
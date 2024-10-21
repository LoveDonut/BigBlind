Shader "Custom/DynamicNeonEnergyRing"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (0,1,1,1)
        _RingRadius ("Ring Radius", Range(0, 0.5)) = 0.4
        _RingWidth ("Ring Width", Range(0, 1)) = 0.05
        _GlowIntensity ("Glow Intensity", Range(0, 10)) = 2
        _NoiseScale ("Noise Scale", Float) = 10
        _NoiseStrength ("Noise Strength", Range(0, 1)) = 0.02
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
            Blend One One
            ZWrite Off
            Cull Off

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
                half4 _MainColor;
                float _RingRadius;
                float _RingWidth;
                float _GlowIntensity;
                float _NoiseScale;
                float _NoiseStrength;
                float _NoiseSpeed;
            CBUFFER_END

            // Simple noise function
            float noise(float2 uv) {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
            }

            // 2D noise based on Morgan McGuire @morgan3d
            // https://www.shadertoy.com/view/4dS3Wd
            float noise2D(float2 st) {
                float2 i = floor(st);
                float2 f = frac(st);

                float a = noise(i);
                float b = noise(i + float2(1.0, 0.0));
                float c = noise(i + float2(0.0, 1.0));
                float d = noise(i + float2(1.0, 1.0));

                float2 u = f * f * (3.0 - 2.0 * f);

                return lerp(a, b, u.x) +
                        (c - a)* u.y * (1.0 - u.x) +
                        (d - b) * u.x * u.y;
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv - 0.5;
                float dist = length(uv);
                
                // Dynamic noise effect
                float2 noiseUV = uv * _NoiseScale + _Time.y * _NoiseSpeed;
                float noiseValue = (noise2D(noiseUV) * 2 - 1) * _NoiseStrength;
                
                // Ring shape with noise
                float ring = smoothstep(_RingRadius - _RingWidth / 2, _RingRadius, dist + noiseValue) 
                           - smoothstep(_RingRadius, _RingRadius + _RingWidth / 2, dist + noiseValue);
                
                // Glow effect
                float glow = pow(ring, 2) * _GlowIntensity;
                
                // Final color
                half4 color = _MainColor * glow;
                color.a = ring;
                
                return color;
            }
            ENDHLSL
        }
    }
}
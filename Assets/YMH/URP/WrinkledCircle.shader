Shader "Custom/NeonEnergyRing"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (0,1,1,1)
        _RingRadius ("Ring Radius", Range(0, 0.5)) = 0.4
        _RingWidth ("Ring Width", Range(0, 0.1)) = 0.05
        _GlowIntensity ("Glow Intensity", Range(0, 10)) = 2
        _RotationSpeed ("Rotation Speed", Float) = 1
        _WaveFrequency ("Wave Frequency", Float) = 10
        _WaveAmplitude ("Wave Amplitude", Range(0, 0.1)) = 0.02
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
                float _RotationSpeed;
                float _WaveFrequency;
                float _WaveAmplitude;
            CBUFFER_END

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
                
                // Rotating effect
                float angle = atan2(uv.y, uv.x);
                float rotationOffset = _Time.y * _RotationSpeed;
                float wave = sin((angle + rotationOffset) * _WaveFrequency) * _WaveAmplitude;
                
                // Ring shape
                float ring = smoothstep(_RingRadius - _RingWidth / 2, _RingRadius, dist + wave) 
                           - smoothstep(_RingRadius, _RingRadius + _RingWidth / 2, dist + wave);
                
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
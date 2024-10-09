Shader "Custom/RainbowGlitter2D"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GlitterTex ("Glitter Texture", 2D) = "white" {}
        _GlitterSpeed ("Glitter Speed", Float) = 1.0
        _GlitterIntensity ("Glitter Intensity", Range(0, 1)) = 0.5
        _RainbowSpeed ("Rainbow Speed", Float) = 1.0
        _RainbowDensity ("Rainbow Density", Float) = 2.0
        _AlphaMultiplier ("Alpha Multiplier", Range(0, 1)) = 1.0
        _AlphaValue ("Alpha Value", Range(0, 1)) = 1.0  // 새로 추가된 알파값 조정 변수
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        ENDHLSL
        Pass
        {
            Name "Rainbow Glitter"
            Tags {"LightMode" = "Universal2D"}
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };
            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_GlitterTex);
            SAMPLER(sampler_GlitterTex);
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float _GlitterSpeed;
                float _GlitterIntensity;
                float _RainbowSpeed;
                float _RainbowDensity;
                float _AlphaMultiplier;
                float _AlphaValue;  // 새로 추가된 알파값 변수
            CBUFFER_END
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.color = IN.color;
                return OUT;
            }
            float3 RainbowColor(float t)
            {
                float3 a = float3(0.5, 0.5, 0.5);
                float3 b = float3(0.5, 0.5, 0.5);
                float3 c = float3(1.0, 1.0, 1.0);
                float3 d = float3(0.00, 0.33, 0.67);
                return a + b * cos(6.28318 * (c * t + d));
            }
            half4 frag(Varyings IN) : SV_Target
            {
                half4 mainColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                
                // Rainbow effect
                float rainbowTime = _Time.y * _RainbowSpeed;
                float3 rainbowColor = RainbowColor(IN.uv.x * _RainbowDensity + rainbowTime);
                
                // Glitter effect
                float2 glitterUV = IN.uv + float2(_Time.y * _GlitterSpeed, 0);
                half4 glitter = SAMPLE_TEXTURE2D(_GlitterTex, sampler_GlitterTex, glitterUV);
                
                // Combine effects
                half3 finalColor = lerp(mainColor.rgb * rainbowColor, glitter.rgb, glitter.a * _GlitterIntensity);
                
                // Apply vertex color and adjust alpha
                finalColor *= IN.color.rgb;
                float finalAlpha = mainColor.a * IN.color.a * _AlphaMultiplier * _AlphaValue;  // _AlphaValue 추가
                
                return half4(finalColor, finalAlpha);
            }
            ENDHLSL
        }
    }
}
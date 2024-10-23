Shader "Custom/NeonLine"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (1,1,1,1)
        _GlowIntensity ("Glow Intensity", Range(0,10)) = 2
        _GlowWidth ("Glow Width", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"
        }

        Blend One One // ���� �������� �۷ο� ȿ�� ��ȭ
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _MainColor;
                float _GlowIntensity;
                float _GlowWidth;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.color = IN.color;
                OUT.uv = IN.uv;
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                // UV�� v ��ǥ�� ������� �۷ο� ���
                float distFromCenter = abs(IN.uv.y - 0.5) * 2;
                float glow = 1 - smoothstep(0, _GlowWidth, distFromCenter);
                
                // �ھ�� �۷ο� ���� ���
                float4 color = _MainColor * _GlowIntensity;
                color.a *= glow;
                
                // ���� �������� �⺻ ���İ��� ����
                color *= IN.color.a;
                
                return color;
            }
            ENDHLSL
        }
    }
}
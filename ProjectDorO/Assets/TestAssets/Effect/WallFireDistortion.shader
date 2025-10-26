Shader "Custom/WallFireDistortion"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _NoiseTex("Noise Texture", 2D) = "white" {}
        _DistortionStrength("Distortion Strength", Range(0, 1)) = 0.05
        _Speed("Distortion Speed", Float) = 1.0
        _Color("Tint Color", Color) = (1, 0.5, 0.2, 1)
        _EmissionStrength("Emission Strength", Range(0, 5)) = 1.0
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 200

        Pass
        {
            Name "WallFireDistortionPass"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

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

            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float4 _MainTex_ST;
            float _DistortionStrength;
            float _Speed;
            float4 _Color;
            float _EmissionStrength;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;

                // �����̴� ������ UV
                float2 noiseUV = uv * 3.0 + float2(_Time.y * _Speed, _Time.y * _Speed * 0.5);
                float2 noise = tex2D(_NoiseTex, noiseUV).rg * 2.0 - 1.0;

                // �ְ� ����
                uv += noise * _DistortionStrength;

                // �ؽ�ó ���� ���ø�
                float4 baseColor = tex2D(_MainTex, uv) * _Color;

                // Emission �߰�
                float3 emission = baseColor.rgb * _EmissionStrength;

                // ���� ���� ���
                float4 finalColor = baseColor;
                finalColor.rgb += emission;

                return finalColor;
            }
            ENDHLSL
        }
    }
    FallBack "Unlit/Transparent"
}

Shader "Unlit/DepthToAlpha"
{
    Properties
    {
        _MainTex("textura principal 1", 2D) = "white" {}
        _MainDepth("Depth principal 1", 2D) = "white" {}
        //_Color("Color", color) = (1,1,1,1)
        _Numer("Multiplicador", float) = 1
        _NumerSum("Sumador", float) = 1
        _NumerMinus("One minus", float) = 1

    }

        SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
        }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

           
            sampler2D _MainTex;
            sampler2D _MainDepth;
            float4 _Color;
            float _Numer;
            float _NumerSum; 
            float _NumerMinus;
            //sampler2D _CameraDepthTexture;
           
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 color = tex2D(_MainTex, i.uv);

                float depth = (tex2D(_MainDepth,i.uv).r) * _Numer;

                return float4(color.r,color.g,color.b, depth);
            }

            ENDHLSL
        }
    }
}
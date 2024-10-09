Shader "Unlit/DepthToAlpha"
{
    Properties
    {
        _MainTex("textura principal 1", 2D) = "white" {}
        _MainDepth("Depth principal 1", 2D) = "white" {}
        //_Color("Color", color) = (1,1,1,1)
        _Numer("Multiplicador", float) = 1
        _Dist("Distancia", float) = 1
        _Potenciador("Potenciador", float) = 1
        [Toggle(_ZOOM)]_Zoom("Zoom activate", Float) = 0

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
            #pragma shader_feature_fragment _ZOOM

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

           
            sampler2D _MainTex;
            sampler2D _MainDepth;
            float4 _Color;
            float _Numer;
            float _Dist;
            float _Potenciador;
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

            void TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
            {
                Out = UV * Tiling + Offset;
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float depth;
                float2 uv = i.uv;
                
                #if _ZOOM

                    depth = ((tex2D(_MainDepth, uv).r));

                    depth = pow(depth, _Dist);

                    //depth = 1 - depth;

                    depth *= _Potenciador;

                    depth = saturate(depth);

                    TilingAndOffset_float(i.uv, depth, (depth * -0.5f) + 0.5f, uv);

                #endif

                depth = ((tex2D(_MainDepth, uv).r)) * _Numer;

                float4 color = tex2D(_MainTex, uv);
                //depth = pow(depth, _NumerSum);

                return float4(color.r,color.g,color.b, depth);
            }

            ENDHLSL
        }
    }
}
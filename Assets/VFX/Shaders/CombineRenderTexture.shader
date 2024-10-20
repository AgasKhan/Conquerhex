Shader "Custom/CombineRenderTexture"
{
    Properties
    {
        _Texture1("Primera Textura", 2D) = "white" {}
       
        _Texture2("Segunda Textura", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off
        Pass
        {
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            // The Blit.hlsl file provides the vertex shader (Vert),
            // input structure (Attributes) and output strucutre (Varyings)
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #pragma vertex Vert //Con V mayuscula anda
            #pragma fragment frag
            #pragma multi_compile_fwdbase

            sampler2D _Texture1;
            sampler2D _Texture2;

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

            float4 frag(v2f input) : SV_Target
            {
                float4 color1 = SAMPLER_TEXTURE2D(_Texture1, input.uv);

                float4 color2 = SAMPLER_TEXTURE2D(_Texture2, input.uv);
                
                if(color1.a>=color2.a)
                {
                    return color1;
                }
                else
                {
                    return color2;
                }
            }
            ENDHLSL
        }
    }
}
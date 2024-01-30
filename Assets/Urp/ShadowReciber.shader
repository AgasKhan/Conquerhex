Shader "URP Transparent Shadow Receiver"
{
    Properties
    {
        _ShadowColor("Shadow Color", Color) = (0.35, 0.4, 0.45, 1.0)
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent"
            "Queue" = "Transparent-1"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            Blend DstColor Zero, Zero One
            Cull Back
            ZTest LEqual
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile_fog
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPerMaterial);
            float4 _ShadowColor;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS               : SV_POSITION;
                float3 positionWS               : TEXCOORD0;
                float fogCoord : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertexInput.positionCS;
                output.positionWS = vertexInput.positionWS;
                output.fogCoord = ComputeFogFactor(vertexInput.positionCS.z);
                return output;
            }

            half CalculatePointLightShadow(float3 worldPos)
            {
                // Implementa la lógica de sombras para luces puntuales aquí
                return 1.0;
            }

            half CalculateSpotLightShadow(float3 worldPos)
            {
                // Implementa la lógica de sombras para luces de foco aquí
                return 1.0;
            }

            half3 CalculatePointOrSpotLightShadow(float3 worldPos)
            {
                half3 shadowColor = half3(1, 1, 1);

                            // Calcula sombras de luces puntuales
                #if defined(_POINT_LIGHT)
                    shadowColor *= CalculatePointLightShadow(worldPos);
                #endif

                // Calcula sombras de luces de foco
                #if defined(_SPOT_LIGHT)
                    shadowColor *= CalculateSpotLightShadow(worldPos);
                #endif

                return shadowColor;
            }


            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                half4 color = half4(1, 1, 1, 1);

                VertexPositionInputs vertexInput = (VertexPositionInputs)0;
                vertexInput.positionWS = input.positionWS;

                // Calcula sombras de luces puntuales y de foco
                //color.rgb = CalculatePointOrSpotLightShadow(input.positionWS);
        
                

                // Mezcla con el color del sombreado principal (si está habilitado)
                #if defined(_MAIN_LIGHT_SHADOWS) || defined(_MAIN_LIGHT_SHADOWS_CASCADE)
                    float4 shadowCoord = GetShadowCoord(vertexInput);
                    half shadowAttenutation = MainLightRealtimeShadow(shadowCoord);
                    color = lerp(half4(1, 1, 1, 1), _ShadowColor, (1.0 - shadowAttenutation) * _ShadowColor.a);
                    color.rgb = MixFogColor(color.rgb, half3(1, 1, 1), input.fogCoord);
                #endif

                return color;
            }



            ENDHLSL
        }
    }
}
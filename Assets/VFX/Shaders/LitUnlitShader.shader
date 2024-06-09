Shader "Custom/LitUnlitBlendShader"
{
    Properties
    {
        _BaseMap("BaseMap", 2D) = "white" {}
        _LitPercentage("Lit Percentage", Range(0, 1)) = 0.5
        _Color("Color", Color) = (1, 1, 1, 1)
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 200

            Pass
            {
                Tags { "LightMode" = "UniversalForward" }
                HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

                TEXTURE2D(_BaseMap);
                SAMPLER(sampler_BaseMap);
                float4 _BaseMap_ST;
                float _LitPercentage;
                float4 _Color;

                struct Attributes
                {
                    float4 positionOS : POSITION;
                    float2 uv : TEXCOORD0;
                    float3 normalOS : NORMAL;
                };

                struct Varyings
                {
                    float4 positionHCS : SV_POSITION;
                    float2 uv : TEXCOORD0;
                    float3 normalWS : TEXCOORD1;
                    float3 positionWS : TEXCOORD2;
                };

                Varyings vert(Attributes input)
                {
                    Varyings output;
                    output.positionHCS = TransformObjectToHClip(input.positionOS);
                    output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                    output.normalWS = normalize(TransformObjectToWorldNormal(input.normalOS));
                    output.positionWS = TransformObjectToWorld(input.positionOS).xyz;
                    return output;
                }

                half4 frag(Varyings input) : SV_Target
                {
                    // Sample the base texture
                    half4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv) * _Color;

                    // Calculate light direction and color manually
                    half3 lightDirection = normalize(_WorldSpaceLightPos0.xyz - input.positionWS);
                    half3 lightColor = _LightColor0.rgb;

                    // Calculate the lighting
                    half3 normal = normalize(input.normalWS);
                    half3 diffuse = lightColor * max(0, dot(normal, lightDirection));

                    // Calculate the final lit color
                    half4 litColor = half4(baseColor.rgb * diffuse, baseColor.a);
                    half4 unlitColor = baseColor;

                    // Interpolate between lit and unlit based on _LitPercentage
                    half4 finalColor = lerp(unlitColor, litColor, _LitPercentage);

                    return finalColor;
                }
                ENDHLSL
            }
        }
            FallBack "Diffuse"
}
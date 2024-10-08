Shader "Shader Graphs/RenderShader Effect"
{
    Properties
    {
        _OverrideColor("_OverrideColor", Float) = 2
        _Color("_Color", Color) = (0, 0.8865156, 1, 1)
        [ToggleUI]_DeActiveEffect("_DeActiveEffect", Float) = 0
        _Waves("_Waves", Vector) = (20, 0.4, 3.62, 3.85)
        [MainTexture][NoScaleOffset]_MainTex("MainTex", 2D) = "white" {}
        _LerpColor("LerpColor", Float) = 1
        _offsetFade("offsetFade", Vector) = (0, 0.06, 0, 0)
        [HideInInspector]_QueueOffset("_QueueOffset", Float) = 0
        [HideInInspector]_QueueControl("_QueueControl", Float) = -1
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
        SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
            "UniversalMaterialType" = "Unlit"
            "Queue" = "Geometry"
            "DisableBatching" = "False"
            "ShaderGraphShader" = "true"
            "ShaderGraphTargetId" = "UniversalUnlitSubTarget"
        }
        Pass
        {
            Name "Universal Forward"
            Tags
            {
                 "LightMode" = "Universal2DA"
            }

        // Render State
        Cull Back
        Blend One Zero
        ZTest Always
        ZWrite On

        // Debug
        // <None>

        // --------------------------------------------------
        // Pass

        HLSLPROGRAM

        // Pragmas
        #pragma target 2.0
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma instancing_options renderinglayer
        #pragma vertex vert
        #pragma fragment frag

        // Keywords
        #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma shader_feature _ _SAMPLE_GI
        #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
        // GraphKeywords: <None>

        // Defines

        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_COLOR
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_UNLIT
        #define _FOG_FRAGMENT 1


        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

        // Includes
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

        // --------------------------------------------------
        // Structs and Packing

        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 texCoord0;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 AbsoluteWorldSpacePosition;
             float2 NDCPosition;
             float2 PixelPosition;
             float4 uv0;
             float4 VertexColor;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
             float4 color : INTERP1;
             float3 positionWS : INTERP2;
             float3 normalWS : INTERP3;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

        PackedVaryings PackVaryings(Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.color.xyzw = input.color;
            output.positionWS.xyz = input.positionWS;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

        Varyings UnpackVaryings(PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            output.color = input.color.xyzw;
            output.positionWS = input.positionWS.xyz;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }


        // --------------------------------------------------
        // Graph

        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_TexelSize;
        float _OverrideColor;
        float4 _Color;
        float _DeActiveEffect;
        float4 _Waves;
        float _LerpColor;
        float2 _offsetFade;
        CBUFFER_END


            // Object and Global properties
            SAMPLER(SamplerState_Linear_Repeat);
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_GrabbedTexture);
            SAMPLER(sampler_GrabbedTexture);
            float4 _GrabbedTexture_TexelSize;

            // Graph Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Hashes.hlsl"

            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif

            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif

            // Graph Functions

            void Unity_Multiply_float_float(float A, float B, out float Out)
            {
                Out = A * B;
            }

            void Unity_Add_float(float A, float B, out float Out)
            {
                Out = A + B;
            }

            void Unity_Sine_float(float In, out float Out)
            {
                Out = sin(In);
            }

            void Unity_Divide_float(float A, float B, out float Out)
            {
                Out = A / B;
            }

            void Unity_Cosine_float(float In, out float Out)
            {
                Out = cos(In);
            }

            void Unity_Add_float2(float2 A, float2 B, out float2 Out)
            {
                Out = A + B;
            }

            void Unity_Saturate_float(float In, out float Out)
            {
                Out = saturate(In);
            }

            void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
            {
                Out = UV * Tiling + Offset;
            }

            float2 Unity_Voronoi_RandomVector_LegacySine_float(float2 UV, float offset)
            {
                Hash_LegacySine_2_2_float(UV, UV);
                return float2(sin(UV.y * offset), cos(UV.x * offset)) * 0.5 + 0.5;
            }

            void Unity_Voronoi_LegacySine_float(float2 UV, float AngleOffset, float CellDensity, out float Out, out float Cells)
            {
                float2 g = floor(UV * CellDensity);
                float2 f = frac(UV * CellDensity);
                float t = 8.0;
                float3 res = float3(8.0, 0.0, 0.0);
                for (int y = -1; y <= 1; y++)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        float2 lattice = float2(x, y);
                        float2 offset = Unity_Voronoi_RandomVector_LegacySine_float(lattice + g, AngleOffset);
                        float d = distance(lattice + offset, f);
                        if (d < res.x)
                        {
                            res = float3(d, offset.x, offset.y);
                            Out = res.x;
                            Cells = res.y;
                        }
                    }
                }
            }

            void Unity_Branch_float(float Predicate, float True, float False, out float Out)
            {
                Out = Predicate ? True : False;
            }

            void Unity_Lerp_float2(float2 A, float2 B, float2 T, out float2 Out)
            {
                Out = lerp(A, B, T);
            }

            void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
            {
                Out = lerp(A, B, T);
            }

            void Unity_Power_float(float A, float B, out float Out)
            {
                Out = pow(A, B);
            }

            void Unity_DDXY_0a363927fad749cf8b7c56f7d34d630c_float(float In, out float Out)
            {

                        #if defined(SHADER_STAGE_RAY_TRACING) && defined(RAYTRACING_SHADER_GRAPH_DEFAULT)
                        #error 'DDXY' node is not supported in ray tracing, please provide an alternate implementation, relying for instance on the 'Raytracing Quality' keyword
                        #endif
                Out = abs(ddx(In)) + abs(ddy(In));
            }

            void Unity_OneMinus_float(float In, out float Out)
            {
                Out = 1 - In;
            }

            // Custom interpolators pre vertex
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

            // Graph Vertex
            struct VertexDescription
            {
                float3 Position;
                float3 Normal;
                float3 Tangent;
            };

            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
            {
                VertexDescription description = (VertexDescription)0;
                description.Position = IN.ObjectSpacePosition;
                description.Normal = IN.ObjectSpaceNormal;
                description.Tangent = IN.ObjectSpaceTangent;
                return description;
            }

            // Custom interpolators, pre surface
            #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
            return output;
            }
            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif

            // Graph Pixel
            struct SurfaceDescription
            {
                float3 BaseColor;
            };

            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                UnityTexture2D _Property_12099d2392e649e286b6d12ed71a609a_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_GrabbedTexture);
                float4 _Property_0e43389aaf604c43b3c2d07ee9f1d591_Out_0_Vector4 = _Waves;
                float _Split_0a9a61cc6c674fddabda59fd63f4aecd_R_1_Float = _Property_0e43389aaf604c43b3c2d07ee9f1d591_Out_0_Vector4[0];
                float _Split_0a9a61cc6c674fddabda59fd63f4aecd_G_2_Float = _Property_0e43389aaf604c43b3c2d07ee9f1d591_Out_0_Vector4[1];
                float _Split_0a9a61cc6c674fddabda59fd63f4aecd_B_3_Float = _Property_0e43389aaf604c43b3c2d07ee9f1d591_Out_0_Vector4[2];
                float _Split_0a9a61cc6c674fddabda59fd63f4aecd_A_4_Float = _Property_0e43389aaf604c43b3c2d07ee9f1d591_Out_0_Vector4[3];
                float _Multiply_d4e5cf389c774989b27d43dd941f4ef9_Out_2_Float;
                Unity_Multiply_float_float(IN.TimeParameters.x, _Split_0a9a61cc6c674fddabda59fd63f4aecd_A_4_Float, _Multiply_d4e5cf389c774989b27d43dd941f4ef9_Out_2_Float);
                float _Add_9c7583068e514c7b8a50e1a7309aab45_Out_2_Float;
                Unity_Add_float(IN.TimeParameters.z, _Multiply_d4e5cf389c774989b27d43dd941f4ef9_Out_2_Float, _Add_9c7583068e514c7b8a50e1a7309aab45_Out_2_Float);
                float _Sine_70b442f3a67d4924a9b5aec4784df971_Out_1_Float;
                Unity_Sine_float(_Add_9c7583068e514c7b8a50e1a7309aab45_Out_2_Float, _Sine_70b442f3a67d4924a9b5aec4784df971_Out_1_Float);
                float _Divide_9c68520175614df3b11e4b72e27d0488_Out_2_Float;
                Unity_Divide_float(_Split_0a9a61cc6c674fddabda59fd63f4aecd_G_2_Float, float(100), _Divide_9c68520175614df3b11e4b72e27d0488_Out_2_Float);
                float _Multiply_e27f0ebd259a4f7895b924b454d41263_Out_2_Float;
                Unity_Multiply_float_float(_Sine_70b442f3a67d4924a9b5aec4784df971_Out_1_Float, _Divide_9c68520175614df3b11e4b72e27d0488_Out_2_Float, _Multiply_e27f0ebd259a4f7895b924b454d41263_Out_2_Float);
                float _Add_5ba55c2998454ec9804b692bde1cbadd_Out_2_Float;
                Unity_Add_float(_Multiply_d4e5cf389c774989b27d43dd941f4ef9_Out_2_Float, IN.TimeParameters.y, _Add_5ba55c2998454ec9804b692bde1cbadd_Out_2_Float);
                float _Cosine_299e4417054e4ea598f46785fce0148f_Out_1_Float;
                Unity_Cosine_float(_Add_5ba55c2998454ec9804b692bde1cbadd_Out_2_Float, _Cosine_299e4417054e4ea598f46785fce0148f_Out_1_Float);
                float _Multiply_786433c5798e4db7ad3ed616da6f2bdf_Out_2_Float;
                Unity_Multiply_float_float(_Cosine_299e4417054e4ea598f46785fce0148f_Out_1_Float, _Divide_9c68520175614df3b11e4b72e27d0488_Out_2_Float, _Multiply_786433c5798e4db7ad3ed616da6f2bdf_Out_2_Float);
                float2 _Vector2_340c0dd60c2d4a7c810a45132b04ea0d_Out_0_Vector2 = float2(_Multiply_e27f0ebd259a4f7895b924b454d41263_Out_2_Float, _Multiply_786433c5798e4db7ad3ed616da6f2bdf_Out_2_Float);
                float4 _ScreenPosition_6e7a3e6891194e06a01d93c268b093e2_Out_0_Vector4 = float4(IN.NDCPosition.xy, 0, 0);
                float2 _Add_fb5091e6ceb44f5b81fba17c4d07fd0a_Out_2_Vector2;
                Unity_Add_float2(_Vector2_340c0dd60c2d4a7c810a45132b04ea0d_Out_0_Vector2, (_ScreenPosition_6e7a3e6891194e06a01d93c268b093e2_Out_0_Vector4.xy), _Add_fb5091e6ceb44f5b81fba17c4d07fd0a_Out_2_Vector2);
                float _Split_bc17113d077f474bb43c82adf31fe12d_R_1_Float = IN.AbsoluteWorldSpacePosition[0];
                float _Split_bc17113d077f474bb43c82adf31fe12d_G_2_Float = IN.AbsoluteWorldSpacePosition[1];
                float _Split_bc17113d077f474bb43c82adf31fe12d_B_3_Float = IN.AbsoluteWorldSpacePosition[2];
                float _Split_bc17113d077f474bb43c82adf31fe12d_A_4_Float = 0;
                float2 _Property_4eac140b5fe44956bef5cb6b37d42f5a_Out_0_Vector2 = _offsetFade;
                float _Split_68e293d366444291b1d9aac5437bfa5c_R_1_Float = _Property_4eac140b5fe44956bef5cb6b37d42f5a_Out_0_Vector2[0];
                float _Split_68e293d366444291b1d9aac5437bfa5c_G_2_Float = _Property_4eac140b5fe44956bef5cb6b37d42f5a_Out_0_Vector2[1];
                float _Split_68e293d366444291b1d9aac5437bfa5c_B_3_Float = 0;
                float _Split_68e293d366444291b1d9aac5437bfa5c_A_4_Float = 0;
                float _Add_0147eec522604b45be1e8e2fa0413f71_Out_2_Float;
                Unity_Add_float(_Split_bc17113d077f474bb43c82adf31fe12d_G_2_Float, _Split_68e293d366444291b1d9aac5437bfa5c_R_1_Float, _Add_0147eec522604b45be1e8e2fa0413f71_Out_2_Float);
                float _Divide_25689ab008034fcc9ffedf797df18efe_Out_2_Float;
                Unity_Divide_float(_Add_0147eec522604b45be1e8e2fa0413f71_Out_2_Float, _Split_68e293d366444291b1d9aac5437bfa5c_G_2_Float, _Divide_25689ab008034fcc9ffedf797df18efe_Out_2_Float);
                float _Saturate_e4061d55b2a149f99478d3263bde45ba_Out_1_Float;
                Unity_Saturate_float(_Divide_25689ab008034fcc9ffedf797df18efe_Out_2_Float, _Saturate_e4061d55b2a149f99478d3263bde45ba_Out_1_Float);
                float _Property_f10053e85cc94fefbf8e444b3d6780a1_Out_0_Boolean = _DeActiveEffect;
                float _Divide_4dfa19c93c0d48fc92af818536d953dc_Out_2_Float;
                Unity_Divide_float(_Split_0a9a61cc6c674fddabda59fd63f4aecd_B_3_Float, float(100), _Divide_4dfa19c93c0d48fc92af818536d953dc_Out_2_Float);
                float _Multiply_db95f8431cff4a03bf03ddea74d2d7d0_Out_2_Float;
                Unity_Multiply_float_float(_Divide_4dfa19c93c0d48fc92af818536d953dc_Out_2_Float, IN.TimeParameters.x, _Multiply_db95f8431cff4a03bf03ddea74d2d7d0_Out_2_Float);
                float2 _Vector2_8498eb561e89467bb77b3182fb06fdaa_Out_0_Vector2 = float2(float(0), _Multiply_db95f8431cff4a03bf03ddea74d2d7d0_Out_2_Float);
                float2 _TilingAndOffset_974e343188494870af4255bb30688d7f_Out_3_Vector2;
                Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Vector2_8498eb561e89467bb77b3182fb06fdaa_Out_0_Vector2, _TilingAndOffset_974e343188494870af4255bb30688d7f_Out_3_Vector2);
                float _Voronoi_ff93b035d5a745f0b2f0f1667671079b_Out_3_Float;
                float _Voronoi_ff93b035d5a745f0b2f0f1667671079b_Cells_4_Float;
                Unity_Voronoi_LegacySine_float(_TilingAndOffset_974e343188494870af4255bb30688d7f_Out_3_Vector2, IN.TimeParameters.x, _Split_0a9a61cc6c674fddabda59fd63f4aecd_R_1_Float, _Voronoi_ff93b035d5a745f0b2f0f1667671079b_Out_3_Float, _Voronoi_ff93b035d5a745f0b2f0f1667671079b_Cells_4_Float);
                float _Branch_d60484dcf9c244e2892f1ca3c61ee83a_Out_3_Float;
                Unity_Branch_float(_Property_f10053e85cc94fefbf8e444b3d6780a1_Out_0_Boolean, float(1), _Voronoi_ff93b035d5a745f0b2f0f1667671079b_Cells_4_Float, _Branch_d60484dcf9c244e2892f1ca3c61ee83a_Out_3_Float);
                float _Add_9007cf9ba70846068e82d2ed33f61d09_Out_2_Float;
                Unity_Add_float(_Saturate_e4061d55b2a149f99478d3263bde45ba_Out_1_Float, _Branch_d60484dcf9c244e2892f1ca3c61ee83a_Out_3_Float, _Add_9007cf9ba70846068e82d2ed33f61d09_Out_2_Float);
                float _Saturate_016ca84577d046e8a8a1cb20de7253da_Out_1_Float;
                Unity_Saturate_float(_Add_9007cf9ba70846068e82d2ed33f61d09_Out_2_Float, _Saturate_016ca84577d046e8a8a1cb20de7253da_Out_1_Float);
                float2 _Lerp_51600998daeb40feac8153411c3a83ce_Out_3_Vector2;
                Unity_Lerp_float2(_Add_fb5091e6ceb44f5b81fba17c4d07fd0a_Out_2_Vector2, (_ScreenPosition_6e7a3e6891194e06a01d93c268b093e2_Out_0_Vector4.xy), (_Saturate_016ca84577d046e8a8a1cb20de7253da_Out_1_Float.xx), _Lerp_51600998daeb40feac8153411c3a83ce_Out_3_Vector2);
                float4 _SampleTexture2D_854e343b48884a6ca8eb8f7e9b24d280_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_12099d2392e649e286b6d12ed71a609a_Out_0_Texture2D.tex, _Property_12099d2392e649e286b6d12ed71a609a_Out_0_Texture2D.samplerstate, _Property_12099d2392e649e286b6d12ed71a609a_Out_0_Texture2D.GetTransformedUV(_Lerp_51600998daeb40feac8153411c3a83ce_Out_3_Vector2));
                float _SampleTexture2D_854e343b48884a6ca8eb8f7e9b24d280_R_4_Float = _SampleTexture2D_854e343b48884a6ca8eb8f7e9b24d280_RGBA_0_Vector4.r;
                float _SampleTexture2D_854e343b48884a6ca8eb8f7e9b24d280_G_5_Float = _SampleTexture2D_854e343b48884a6ca8eb8f7e9b24d280_RGBA_0_Vector4.g;
                float _SampleTexture2D_854e343b48884a6ca8eb8f7e9b24d280_B_6_Float = _SampleTexture2D_854e343b48884a6ca8eb8f7e9b24d280_RGBA_0_Vector4.b;
                float _SampleTexture2D_854e343b48884a6ca8eb8f7e9b24d280_A_7_Float = _SampleTexture2D_854e343b48884a6ca8eb8f7e9b24d280_RGBA_0_Vector4.a;
                float4 _Property_bf2e8901ab444031993ca902415355c2_Out_0_Vector4 = _Color;
                float _Property_201b7f3de12d4866af33d0e460d5b258_Out_0_Float = _LerpColor;
                float4 _Lerp_a715175cd4f84509a746af21ef49de6c_Out_3_Vector4;
                Unity_Lerp_float4(_Property_bf2e8901ab444031993ca902415355c2_Out_0_Vector4, IN.VertexColor, (_Property_201b7f3de12d4866af33d0e460d5b258_Out_0_Float.xxxx), _Lerp_a715175cd4f84509a746af21ef49de6c_Out_3_Vector4);
                float _Property_3b1e131023804e32ab832254be308715_Out_0_Float = _OverrideColor;
                float _Power_9300c14b2e354b90ad6468d62f03bfa5_Out_2_Float;
                Unity_Power_float(_Voronoi_ff93b035d5a745f0b2f0f1667671079b_Out_3_Float, _Property_3b1e131023804e32ab832254be308715_Out_0_Float, _Power_9300c14b2e354b90ad6468d62f03bfa5_Out_2_Float);
                float _DDXY_0a363927fad749cf8b7c56f7d34d630c_Out_1_Float;
                Unity_DDXY_0a363927fad749cf8b7c56f7d34d630c_float(_Voronoi_ff93b035d5a745f0b2f0f1667671079b_Cells_4_Float, _DDXY_0a363927fad749cf8b7c56f7d34d630c_Out_1_Float);
                float _Add_ebdcea94022748aebc23abfa81d117d0_Out_2_Float;
                Unity_Add_float(_Power_9300c14b2e354b90ad6468d62f03bfa5_Out_2_Float, _DDXY_0a363927fad749cf8b7c56f7d34d630c_Out_1_Float, _Add_ebdcea94022748aebc23abfa81d117d0_Out_2_Float);
                float _Saturate_fa7adf9f48a149b981a3b7e5506fb910_Out_1_Float;
                Unity_Saturate_float(_Add_ebdcea94022748aebc23abfa81d117d0_Out_2_Float, _Saturate_fa7adf9f48a149b981a3b7e5506fb910_Out_1_Float);
                float _OneMinus_5bc69c1f3d7349428ca986cc7153eaaf_Out_1_Float;
                Unity_OneMinus_float(_Saturate_e4061d55b2a149f99478d3263bde45ba_Out_1_Float, _OneMinus_5bc69c1f3d7349428ca986cc7153eaaf_Out_1_Float);
                float _Multiply_443bc8972331494f8176c341356372aa_Out_2_Float;
                Unity_Multiply_float_float(_Saturate_fa7adf9f48a149b981a3b7e5506fb910_Out_1_Float, _OneMinus_5bc69c1f3d7349428ca986cc7153eaaf_Out_1_Float, _Multiply_443bc8972331494f8176c341356372aa_Out_2_Float);
                float _Branch_ee1d13b0fbbb4fd28a8c2418c89269e1_Out_3_Float;
                Unity_Branch_float(_Property_f10053e85cc94fefbf8e444b3d6780a1_Out_0_Boolean, float(0), _Multiply_443bc8972331494f8176c341356372aa_Out_2_Float, _Branch_ee1d13b0fbbb4fd28a8c2418c89269e1_Out_3_Float);
                float4 _Lerp_7a10e1beef344d34a75bae4642cb3ed3_Out_3_Vector4;
                Unity_Lerp_float4(_SampleTexture2D_854e343b48884a6ca8eb8f7e9b24d280_RGBA_0_Vector4, _Lerp_a715175cd4f84509a746af21ef49de6c_Out_3_Vector4, (_Branch_ee1d13b0fbbb4fd28a8c2418c89269e1_Out_3_Float.xxxx), _Lerp_7a10e1beef344d34a75bae4642cb3ed3_Out_3_Vector4);
                surface.BaseColor = (_Lerp_7a10e1beef344d34a75bae4642cb3ed3_Out_3_Vector4.xyz);
                return surface;
            }

            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES Attributes
            #define VFX_SRP_VARYINGS Varyings
            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
            #endif
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
            {
                VertexDescriptionInputs output;
                ZERO_INITIALIZE(VertexDescriptionInputs, output);

                output.ObjectSpaceNormal = input.normalOS;
                output.ObjectSpaceTangent = input.tangentOS.xyz;
                output.ObjectSpacePosition = input.positionOS;

                return output;
            }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

            #ifdef HAVE_VFX_MODIFICATION
            #if VFX_USE_GRAPH_VALUES
                uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
                /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
            #endif
                /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

            #endif







                output.AbsoluteWorldSpacePosition = GetAbsolutePositionWS(input.positionWS);

                #if UNITY_UV_STARTS_AT_TOP
                output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x < 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
                #else
                output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x > 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
                #endif

                output.NDCPosition = output.PixelPosition.xy / _ScaledScreenParams.xy;
                output.NDCPosition.y = 1.0f - output.NDCPosition.y;

                output.uv0 = input.texCoord0;
                output.VertexColor = input.color;
                output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
            #else
            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
            #endif
            #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                    return output;
            }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"

            // --------------------------------------------------
            // Visual Effect Vertex Invocations
            #ifdef HAVE_VFX_MODIFICATION
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
            #endif

            ENDHLSL
            }
            Pass
            {
                Name "DepthOnly"
                Tags
                {
                    "LightMode" = "DepthOnly"
                }

                // Render State
                Cull Back
                ZTest LEqual
                ZWrite On
                ColorMask R

                // Debug
                // <None>

                // --------------------------------------------------
                // Pass

                HLSLPROGRAM

                // Pragmas
                #pragma target 2.0
                #pragma multi_compile_instancing
                #pragma vertex vert
                #pragma fragment frag

                // Keywords
                // PassKeywords: <None>
                // GraphKeywords: <None>

                // Defines

                #define ATTRIBUTES_NEED_NORMAL
                #define ATTRIBUTES_NEED_TANGENT
                #define FEATURES_GRAPH_VERTEX
                /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                #define SHADERPASS SHADERPASS_DEPTHONLY


                // custom interpolator pre-include
                /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                // Includes
                #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

                // --------------------------------------------------
                // Structs and Packing

                // custom interpolators pre packing
                /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                struct Attributes
                {
                     float3 positionOS : POSITION;
                     float3 normalOS : NORMAL;
                     float4 tangentOS : TANGENT;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                     float4 positionCS : SV_POSITION;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
                struct SurfaceDescriptionInputs
                {
                };
                struct VertexDescriptionInputs
                {
                     float3 ObjectSpaceNormal;
                     float3 ObjectSpaceTangent;
                     float3 ObjectSpacePosition;
                };
                struct PackedVaryings
                {
                     float4 positionCS : SV_POSITION;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };

                PackedVaryings PackVaryings(Varyings input)
                {
                    PackedVaryings output;
                    ZERO_INITIALIZE(PackedVaryings, output);
                    output.positionCS = input.positionCS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }

                Varyings UnpackVaryings(PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }


                // --------------------------------------------------
                // Graph

                // Graph Properties
                CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_TexelSize;
                float _OverrideColor;
                float4 _Color;
                float _DeActiveEffect;
                float4 _Waves;
                float _LerpColor;
                float2 _offsetFade;
                CBUFFER_END


                    // Object and Global properties
                    SAMPLER(SamplerState_Linear_Repeat);
                    TEXTURE2D(_MainTex);
                    SAMPLER(sampler_MainTex);
                    TEXTURE2D(_GrabbedTexture);
                    SAMPLER(sampler_GrabbedTexture);
                    float4 _GrabbedTexture_TexelSize;

                    // Graph Includes
                    // GraphIncludes: <None>

                    // -- Property used by ScenePickingPass
                    #ifdef SCENEPICKINGPASS
                    float4 _SelectionID;
                    #endif

                    // -- Properties used by SceneSelectionPass
                    #ifdef SCENESELECTIONPASS
                    int _ObjectId;
                    int _PassValue;
                    #endif

                    // Graph Functions
                    // GraphFunctions: <None>

                    // Custom interpolators pre vertex
                    /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                    // Graph Vertex
                    struct VertexDescription
                    {
                        float3 Position;
                        float3 Normal;
                        float3 Tangent;
                    };

                    VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                    {
                        VertexDescription description = (VertexDescription)0;
                        description.Position = IN.ObjectSpacePosition;
                        description.Normal = IN.ObjectSpaceNormal;
                        description.Tangent = IN.ObjectSpaceTangent;
                        return description;
                    }

                    // Custom interpolators, pre surface
                    #ifdef FEATURES_GRAPH_VERTEX
                    Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                    {
                    return output;
                    }
                    #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                    #endif

                    // Graph Pixel
                    struct SurfaceDescription
                    {
                    };

                    SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                    {
                        SurfaceDescription surface = (SurfaceDescription)0;
                        return surface;
                    }

                    // --------------------------------------------------
                    // Build Graph Inputs
                    #ifdef HAVE_VFX_MODIFICATION
                    #define VFX_SRP_ATTRIBUTES Attributes
                    #define VFX_SRP_VARYINGS Varyings
                    #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
                    #endif
                    VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                    {
                        VertexDescriptionInputs output;
                        ZERO_INITIALIZE(VertexDescriptionInputs, output);

                        output.ObjectSpaceNormal = input.normalOS;
                        output.ObjectSpaceTangent = input.tangentOS.xyz;
                        output.ObjectSpacePosition = input.positionOS;

                        return output;
                    }
                    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                    {
                        SurfaceDescriptionInputs output;
                        ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                    #ifdef HAVE_VFX_MODIFICATION
                    #if VFX_USE_GRAPH_VALUES
                        uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
                        /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
                    #endif
                        /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                    #endif








                        #if UNITY_UV_STARTS_AT_TOP
                        #else
                        #endif


                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                    #else
                    #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                    #endif
                    #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                            return output;
                    }

                    // --------------------------------------------------
                    // Main

                    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"

                    // --------------------------------------------------
                    // Visual Effect Vertex Invocations
                    #ifdef HAVE_VFX_MODIFICATION
                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
                    #endif

                    ENDHLSL
                    }
                    Pass
                    {
                        Name "DepthNormalsOnly"
                        Tags
                        {
                            "LightMode" = "DepthNormalsOnly"
                        }

                        // Render State
                        Cull Back
                        ZTest LEqual
                        ZWrite On

                        // Debug
                        // <None>

                        // --------------------------------------------------
                        // Pass

                        HLSLPROGRAM

                        // Pragmas
                        #pragma target 2.0
                        #pragma multi_compile_instancing
                        #pragma vertex vert
                        #pragma fragment frag

                        // Keywords
                        #pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT
                        // GraphKeywords: <None>

                        // Defines

                        #define ATTRIBUTES_NEED_NORMAL
                        #define ATTRIBUTES_NEED_TANGENT
                        #define VARYINGS_NEED_NORMAL_WS
                        #define FEATURES_GRAPH_VERTEX
                        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                        #define SHADERPASS SHADERPASS_DEPTHNORMALSONLY


                        // custom interpolator pre-include
                        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                        // Includes
                        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
                        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
                        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
                        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
                        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

                        // --------------------------------------------------
                        // Structs and Packing

                        // custom interpolators pre packing
                        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                        struct Attributes
                        {
                             float3 positionOS : POSITION;
                             float3 normalOS : NORMAL;
                             float4 tangentOS : TANGENT;
                            #if UNITY_ANY_INSTANCING_ENABLED
                             uint instanceID : INSTANCEID_SEMANTIC;
                            #endif
                        };
                        struct Varyings
                        {
                             float4 positionCS : SV_POSITION;
                             float3 normalWS;
                            #if UNITY_ANY_INSTANCING_ENABLED
                             uint instanceID : CUSTOM_INSTANCE_ID;
                            #endif
                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                            #endif
                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                            #endif
                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                            #endif
                        };
                        struct SurfaceDescriptionInputs
                        {
                        };
                        struct VertexDescriptionInputs
                        {
                             float3 ObjectSpaceNormal;
                             float3 ObjectSpaceTangent;
                             float3 ObjectSpacePosition;
                        };
                        struct PackedVaryings
                        {
                             float4 positionCS : SV_POSITION;
                             float3 normalWS : INTERP0;
                            #if UNITY_ANY_INSTANCING_ENABLED
                             uint instanceID : CUSTOM_INSTANCE_ID;
                            #endif
                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                            #endif
                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                            #endif
                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                            #endif
                        };

                        PackedVaryings PackVaryings(Varyings input)
                        {
                            PackedVaryings output;
                            ZERO_INITIALIZE(PackedVaryings, output);
                            output.positionCS = input.positionCS;
                            output.normalWS.xyz = input.normalWS;
                            #if UNITY_ANY_INSTANCING_ENABLED
                            output.instanceID = input.instanceID;
                            #endif
                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                            #endif
                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                            #endif
                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                            output.cullFace = input.cullFace;
                            #endif
                            return output;
                        }

                        Varyings UnpackVaryings(PackedVaryings input)
                        {
                            Varyings output;
                            output.positionCS = input.positionCS;
                            output.normalWS = input.normalWS.xyz;
                            #if UNITY_ANY_INSTANCING_ENABLED
                            output.instanceID = input.instanceID;
                            #endif
                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                            #endif
                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                            #endif
                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                            output.cullFace = input.cullFace;
                            #endif
                            return output;
                        }


                        // --------------------------------------------------
                        // Graph

                        // Graph Properties
                        CBUFFER_START(UnityPerMaterial)
                        float4 _MainTex_TexelSize;
                        float _OverrideColor;
                        float4 _Color;
                        float _DeActiveEffect;
                        float4 _Waves;
                        float _LerpColor;
                        float2 _offsetFade;
                        CBUFFER_END


                            // Object and Global properties
                            SAMPLER(SamplerState_Linear_Repeat);
                            TEXTURE2D(_MainTex);
                            SAMPLER(sampler_MainTex);
                            TEXTURE2D(_GrabbedTexture);
                            SAMPLER(sampler_GrabbedTexture);
                            float4 _GrabbedTexture_TexelSize;

                            // Graph Includes
                            // GraphIncludes: <None>

                            // -- Property used by ScenePickingPass
                            #ifdef SCENEPICKINGPASS
                            float4 _SelectionID;
                            #endif

                            // -- Properties used by SceneSelectionPass
                            #ifdef SCENESELECTIONPASS
                            int _ObjectId;
                            int _PassValue;
                            #endif

                            // Graph Functions
                            // GraphFunctions: <None>

                            // Custom interpolators pre vertex
                            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                            // Graph Vertex
                            struct VertexDescription
                            {
                                float3 Position;
                                float3 Normal;
                                float3 Tangent;
                            };

                            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                            {
                                VertexDescription description = (VertexDescription)0;
                                description.Position = IN.ObjectSpacePosition;
                                description.Normal = IN.ObjectSpaceNormal;
                                description.Tangent = IN.ObjectSpaceTangent;
                                return description;
                            }

                            // Custom interpolators, pre surface
                            #ifdef FEATURES_GRAPH_VERTEX
                            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                            {
                            return output;
                            }
                            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                            #endif

                            // Graph Pixel
                            struct SurfaceDescription
                            {
                            };

                            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                            {
                                SurfaceDescription surface = (SurfaceDescription)0;
                                return surface;
                            }

                            // --------------------------------------------------
                            // Build Graph Inputs
                            #ifdef HAVE_VFX_MODIFICATION
                            #define VFX_SRP_ATTRIBUTES Attributes
                            #define VFX_SRP_VARYINGS Varyings
                            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
                            #endif
                            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                            {
                                VertexDescriptionInputs output;
                                ZERO_INITIALIZE(VertexDescriptionInputs, output);

                                output.ObjectSpaceNormal = input.normalOS;
                                output.ObjectSpaceTangent = input.tangentOS.xyz;
                                output.ObjectSpacePosition = input.positionOS;

                                return output;
                            }
                            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                            {
                                SurfaceDescriptionInputs output;
                                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                            #ifdef HAVE_VFX_MODIFICATION
                            #if VFX_USE_GRAPH_VALUES
                                uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
                                /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
                            #endif
                                /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                            #endif








                                #if UNITY_UV_STARTS_AT_TOP
                                #else
                                #endif


                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                            #else
                            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                            #endif
                            #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                                    return output;
                            }

                            // --------------------------------------------------
                            // Main

                            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthNormalsOnlyPass.hlsl"

                            // --------------------------------------------------
                            // Visual Effect Vertex Invocations
                            #ifdef HAVE_VFX_MODIFICATION
                            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
                            #endif

                            ENDHLSL
                            }
                            Pass
                            {
                                Name "GBuffer"
                                Tags
                                {
                                    "LightMode" = "UniversalGBuffer"
                                }

                                // Render State
                                Cull Back
                                Blend One Zero
                                ZTest Always
                                ZWrite On

                                // Debug
                                // <None>

                                // --------------------------------------------------
                                // Pass

                                HLSLPROGRAM

                                // Pragmas
                                #pragma target 4.5
                                #pragma exclude_renderers gles gles3 glcore
                                #pragma multi_compile_instancing
                                #pragma multi_compile_fog
                                #pragma instancing_options renderinglayer
                                #pragma vertex vert
                                #pragma fragment frag

                                // Keywords
                                #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
                                #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
                                // GraphKeywords: <None>

                                // Defines

                                #define ATTRIBUTES_NEED_NORMAL
                                #define ATTRIBUTES_NEED_TANGENT
                                #define ATTRIBUTES_NEED_TEXCOORD0
                                #define ATTRIBUTES_NEED_COLOR
                                #define VARYINGS_NEED_POSITION_WS
                                #define VARYINGS_NEED_NORMAL_WS
                                #define VARYINGS_NEED_TEXCOORD0
                                #define VARYINGS_NEED_COLOR
                                #define FEATURES_GRAPH_VERTEX
                                /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                                #define SHADERPASS SHADERPASS_GBUFFER


                                // custom interpolator pre-include
                                /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                                // Includes
                                #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
                                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
                                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                                #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
                                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
                                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

                                // --------------------------------------------------
                                // Structs and Packing

                                // custom interpolators pre packing
                                /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                                struct Attributes
                                {
                                     float3 positionOS : POSITION;
                                     float3 normalOS : NORMAL;
                                     float4 tangentOS : TANGENT;
                                     float4 uv0 : TEXCOORD0;
                                     float4 color : COLOR;
                                    #if UNITY_ANY_INSTANCING_ENABLED
                                     uint instanceID : INSTANCEID_SEMANTIC;
                                    #endif
                                };
                                struct Varyings
                                {
                                     float4 positionCS : SV_POSITION;
                                     float3 positionWS;
                                     float3 normalWS;
                                     float4 texCoord0;
                                     float4 color;
                                    #if !defined(LIGHTMAP_ON)
                                     float3 sh;
                                    #endif
                                    #if UNITY_ANY_INSTANCING_ENABLED
                                     uint instanceID : CUSTOM_INSTANCE_ID;
                                    #endif
                                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                    #endif
                                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                    #endif
                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                    #endif
                                };
                                struct SurfaceDescriptionInputs
                                {
                                     float3 AbsoluteWorldSpacePosition;
                                     float2 NDCPosition;
                                     float2 PixelPosition;
                                     float4 uv0;
                                     float4 VertexColor;
                                     float3 TimeParameters;
                                };
                                struct VertexDescriptionInputs
                                {
                                     float3 ObjectSpaceNormal;
                                     float3 ObjectSpaceTangent;
                                     float3 ObjectSpacePosition;
                                };
                                struct PackedVaryings
                                {
                                     float4 positionCS : SV_POSITION;
                                    #if !defined(LIGHTMAP_ON)
                                     float3 sh : INTERP0;
                                    #endif
                                     float4 texCoord0 : INTERP1;
                                     float4 color : INTERP2;
                                     float3 positionWS : INTERP3;
                                     float3 normalWS : INTERP4;
                                    #if UNITY_ANY_INSTANCING_ENABLED
                                     uint instanceID : CUSTOM_INSTANCE_ID;
                                    #endif
                                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                    #endif
                                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                    #endif
                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                    #endif
                                };

                                PackedVaryings PackVaryings(Varyings input)
                                {
                                    PackedVaryings output;
                                    ZERO_INITIALIZE(PackedVaryings, output);
                                    output.positionCS = input.positionCS;
                                    #if !defined(LIGHTMAP_ON)
                                    output.sh = input.sh;
                                    #endif
                                    output.texCoord0.xyzw = input.texCoord0;
                                    output.color.xyzw = input.color;
                                    output.positionWS.xyz = input.positionWS;
                                    output.normalWS.xyz = input.normalWS;
                                    #if UNITY_ANY_INSTANCING_ENABLED
                                    output.instanceID = input.instanceID;
                                    #endif
                                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                    #endif
                                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                    #endif
                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                    output.cullFace = input.cullFace;
                                    #endif
                                    return output;
                                }

                                Varyings UnpackVaryings(PackedVaryings input)
                                {
                                    Varyings output;
                                    output.positionCS = input.positionCS;
                                    #if !defined(LIGHTMAP_ON)
                                    output.sh = input.sh;
                                    #endif
                                    output.texCoord0 = input.texCoord0.xyzw;
                                    output.color = input.color.xyzw;
                                    output.positionWS = input.positionWS.xyz;
                                    output.normalWS = input.normalWS.xyz;
                                    #if UNITY_ANY_INSTANCING_ENABLED
                                    output.instanceID = input.instanceID;
                                    #endif
                                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                    #endif
                                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                    #endif
                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                    output.cullFace = input.cullFace;
                                    #endif
                                    return output;
                                }


                                // --------------------------------------------------
                                // Graph

                                // Graph Properties
                                CBUFFER_START(UnityPerMaterial)
                                float4 _MainTex_TexelSize;
                                float _OverrideColor;
                                float4 _Color;
                                float _DeActiveEffect;
                                float4 _Waves;
                                float _LerpColor;
                                float2 _offsetFade;
                                CBUFFER_END


                                    // Object and Global properties
                                    SAMPLER(SamplerState_Linear_Repeat);
                                    TEXTURE2D(_MainTex);
                                    SAMPLER(sampler_MainTex);
                                    TEXTURE2D(_GrabbedTexture);
                                    SAMPLER(sampler_GrabbedTexture);
                                    float4 _GrabbedTexture_TexelSize;

                                    // Graph Includes
                                    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Hashes.hlsl"

                                    // -- Property used by ScenePickingPass
                                    #ifdef SCENEPICKINGPASS
                                    float4 _SelectionID;
                                    #endif

                                    // -- Properties used by SceneSelectionPass
                                    #ifdef SCENESELECTIONPASS
                                    int _ObjectId;
                                    int _PassValue;
                                    #endif

                                    // Graph Functions

                                    void Unity_Multiply_float_float(float A, float B, out float Out)
                                    {
                                        Out = A * B;
                                    }

                                    void Unity_Add_float(float A, float B, out float Out)
                                    {
                                        Out = A + B;
                                    }

                                    void Unity_Sine_float(float In, out float Out)
                                    {
                                        Out = sin(In);
                                    }

                                    void Unity_Divide_float(float A, float B, out float Out)
                                    {
                                        Out = A / B;
                                    }

                                    void Unity_Cosine_float(float In, out float Out)
                                    {
                                        Out = cos(In);
                                    }

                                    void Unity_Add_float2(float2 A, float2 B, out float2 Out)
                                    {
                                        Out = A + B;
                                    }

                                    void Unity_Saturate_float(float In, out float Out)
                                    {
                                        Out = saturate(In);
                                    }

                                    void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
                                    {
                                        Out = UV * Tiling + Offset;
                                    }

                                    float2 Unity_Voronoi_RandomVector_LegacySine_float(float2 UV, float offset)
                                    {
                                        Hash_LegacySine_2_2_float(UV, UV);
                                        return float2(sin(UV.y * offset), cos(UV.x * offset)) * 0.5 + 0.5;
                                    }

                                    void Unity_Voronoi_LegacySine_float(float2 UV, float AngleOffset, float CellDensity, out float Out, out float Cells)
                                    {
                                        float2 g = floor(UV * CellDensity);
                                        float2 f = frac(UV * CellDensity);
                                        float t = 8.0;
                                        float3 res = float3(8.0, 0.0, 0.0);
                                        for (int y = -1; y <= 1; y++)
                                        {
                                            for (int x = -1; x <= 1; x++)
                                            {
                                                float2 lattice = float2(x, y);
                                                float2 offset = Unity_Voronoi_RandomVector_LegacySine_float(lattice + g, AngleOffset);
                                                float d = distance(lattice + offset, f);
                                                if (d < res.x)
                                                {
                                                    res = float3(d, offset.x, offset.y);
                                                    Out = res.x;
                                                    Cells = res.y;
                                                }
                                            }
                                        }
                                    }

                                    void Unity_Branch_float(float Predicate, float True, float False, out float Out)
                                    {
                                        Out = Predicate ? True : False;
                                    }

                                    void Unity_Lerp_float2(float2 A, float2 B, float2 T, out float2 Out)
                                    {
                                        Out = lerp(A, B, T);
                                    }

                                    void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
                                    {
                                        Out = lerp(A, B, T);
                                    }

                                    void Unity_Power_float(float A, float B, out float Out)
                                    {
                                        Out = pow(A, B);
                                    }

                                    void Unity_DDXY_0a363927fad749cf8b7c56f7d34d630c_float(float In, out float Out)
                                    {

                                                #if defined(SHADER_STAGE_RAY_TRACING) && defined(RAYTRACING_SHADER_GRAPH_DEFAULT)
                                                #error 'DDXY' node is not supported in ray tracing, please provide an alternate implementation, relying for instance on the 'Raytracing Quality' keyword
                                                #endif
                                        Out = abs(ddx(In)) + abs(ddy(In));
                                    }

                                    void Unity_OneMinus_float(float In, out float Out)
                                    {
                                        Out = 1 - In;
                                    }

                                    // Custom interpolators pre vertex
                                    /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                                    // Graph Vertex
                                    struct VertexDescription
                                    {
                                        float3 Position;
                                        float3 Normal;
                                        float3 Tangent;
                                    };

                                    VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                                    {
                                        VertexDescription description = (VertexDescription)0;
                                        description.Position = IN.ObjectSpacePosition;
                                        description.Normal = IN.ObjectSpaceNormal;
                                        description.Tangent = IN.ObjectSpaceTangent;
                                        return description;
                                    }

                                    // Custom interpolators, pre surface
                                    #ifdef FEATURES_GRAPH_VERTEX
                                    Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                                    {
                                    return output;
                                    }
                                    #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                                    #endif

                                    // Graph Pixel
                                    struct SurfaceDescription
                                    {
                                        float3 BaseColor;
                                    };

                                    SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                                    {
                                        SurfaceDescription surface = (SurfaceDescription)0;
                                        UnityTexture2D _Property_12099d2392e649e286b6d12ed71a609a_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_GrabbedTexture);
                                        float4 _Property_0e43389aaf604c43b3c2d07ee9f1d591_Out_0_Vector4 = _Waves;
                                        float _Split_0a9a61cc6c674fddabda59fd63f4aecd_R_1_Float = _Property_0e43389aaf604c43b3c2d07ee9f1d591_Out_0_Vector4[0];
                                        float _Split_0a9a61cc6c674fddabda59fd63f4aecd_G_2_Float = _Property_0e43389aaf604c43b3c2d07ee9f1d591_Out_0_Vector4[1];
                                        float _Split_0a9a61cc6c674fddabda59fd63f4aecd_B_3_Float = _Property_0e43389aaf604c43b3c2d07ee9f1d591_Out_0_Vector4[2];
                                        float _Split_0a9a61cc6c674fddabda59fd63f4aecd_A_4_Float = _Property_0e43389aaf604c43b3c2d07ee9f1d591_Out_0_Vector4[3];
                                        float _Multiply_d4e5cf389c774989b27d43dd941f4ef9_Out_2_Float;
                                        Unity_Multiply_float_float(IN.TimeParameters.x, _Split_0a9a61cc6c674fddabda59fd63f4aecd_A_4_Float, _Multiply_d4e5cf389c774989b27d43dd941f4ef9_Out_2_Float);
                                        float _Add_9c7583068e514c7b8a50e1a7309aab45_Out_2_Float;
                                        Unity_Add_float(IN.TimeParameters.z, _Multiply_d4e5cf389c774989b27d43dd941f4ef9_Out_2_Float, _Add_9c7583068e514c7b8a50e1a7309aab45_Out_2_Float);
                                        float _Sine_70b442f3a67d4924a9b5aec4784df971_Out_1_Float;
                                        Unity_Sine_float(_Add_9c7583068e514c7b8a50e1a7309aab45_Out_2_Float, _Sine_70b442f3a67d4924a9b5aec4784df971_Out_1_Float);
                                        float _Divide_9c68520175614df3b11e4b72e27d0488_Out_2_Float;
                                        Unity_Divide_float(_Split_0a9a61cc6c674fddabda59fd63f4aecd_G_2_Float, float(100), _Divide_9c68520175614df3b11e4b72e27d0488_Out_2_Float);
                                        float _Multiply_e27f0ebd259a4f7895b924b454d41263_Out_2_Float;
                                        Unity_Multiply_float_float(_Sine_70b442f3a67d4924a9b5aec4784df971_Out_1_Float, _Divide_9c68520175614df3b11e4b72e27d0488_Out_2_Float, _Multiply_e27f0ebd259a4f7895b924b454d41263_Out_2_Float);
                                        float _Add_5ba55c2998454ec9804b692bde1cbadd_Out_2_Float;
                                        Unity_Add_float(_Multiply_d4e5cf389c774989b27d43dd941f4ef9_Out_2_Float, IN.TimeParameters.y, _Add_5ba55c2998454ec9804b692bde1cbadd_Out_2_Float);
                                        float _Cosine_299e4417054e4ea598f46785fce0148f_Out_1_Float;
                                        Unity_Cosine_float(_Add_5ba55c2998454ec9804b692bde1cbadd_Out_2_Float, _Cosine_299e4417054e4ea598f46785fce0148f_Out_1_Float);
                                        float _Multiply_786433c5798e4db7ad3ed616da6f2bdf_Out_2_Float;
                                        Unity_Multiply_float_float(_Cosine_299e4417054e4ea598f46785fce0148f_Out_1_Float, _Divide_9c68520175614df3b11e4b72e27d0488_Out_2_Float, _Multiply_786433c5798e4db7ad3ed616da6f2bdf_Out_2_Float);
                                        float2 _Vector2_340c0dd60c2d4a7c810a45132b04ea0d_Out_0_Vector2 = float2(_Multiply_e27f0ebd259a4f7895b924b454d41263_Out_2_Float, _Multiply_786433c5798e4db7ad3ed616da6f2bdf_Out_2_Float);
                                        float4 _ScreenPosition_6e7a3e6891194e06a01d93c268b093e2_Out_0_Vector4 = float4(IN.NDCPosition.xy, 0, 0);
                                        float2 _Add_fb5091e6ceb44f5b81fba17c4d07fd0a_Out_2_Vector2;
                                        Unity_Add_float2(_Vector2_340c0dd60c2d4a7c810a45132b04ea0d_Out_0_Vector2, (_ScreenPosition_6e7a3e6891194e06a01d93c268b093e2_Out_0_Vector4.xy), _Add_fb5091e6ceb44f5b81fba17c4d07fd0a_Out_2_Vector2);
                                        float _Split_bc17113d077f474bb43c82adf31fe12d_R_1_Float = IN.AbsoluteWorldSpacePosition[0];
                                        float _Split_bc17113d077f474bb43c82adf31fe12d_G_2_Float = IN.AbsoluteWorldSpacePosition[1];
                                        float _Split_bc17113d077f474bb43c82adf31fe12d_B_3_Float = IN.AbsoluteWorldSpacePosition[2];
                                        float _Split_bc17113d077f474bb43c82adf31fe12d_A_4_Float = 0;
                                        float2 _Property_4eac140b5fe44956bef5cb6b37d42f5a_Out_0_Vector2 = _offsetFade;
                                        float _Split_68e293d366444291b1d9aac5437bfa5c_R_1_Float = _Property_4eac140b5fe44956bef5cb6b37d42f5a_Out_0_Vector2[0];
                                        float _Split_68e293d366444291b1d9aac5437bfa5c_G_2_Float = _Property_4eac140b5fe44956bef5cb6b37d42f5a_Out_0_Vector2[1];
                                        float _Split_68e293d366444291b1d9aac5437bfa5c_B_3_Float = 0;
                                        float _Split_68e293d366444291b1d9aac5437bfa5c_A_4_Float = 0;
                                        float _Add_0147eec522604b45be1e8e2fa0413f71_Out_2_Float;
                                        Unity_Add_float(_Split_bc17113d077f474bb43c82adf31fe12d_G_2_Float, _Split_68e293d366444291b1d9aac5437bfa5c_R_1_Float, _Add_0147eec522604b45be1e8e2fa0413f71_Out_2_Float);
                                        float _Divide_25689ab008034fcc9ffedf797df18efe_Out_2_Float;
                                        Unity_Divide_float(_Add_0147eec522604b45be1e8e2fa0413f71_Out_2_Float, _Split_68e293d366444291b1d9aac5437bfa5c_G_2_Float, _Divide_25689ab008034fcc9ffedf797df18efe_Out_2_Float);
                                        float _Saturate_e4061d55b2a149f99478d3263bde45ba_Out_1_Float;
                                        Unity_Saturate_float(_Divide_25689ab008034fcc9ffedf797df18efe_Out_2_Float, _Saturate_e4061d55b2a149f99478d3263bde45ba_Out_1_Float);
                                        float _Property_f10053e85cc94fefbf8e444b3d6780a1_Out_0_Boolean = _DeActiveEffect;
                                        float _Divide_4dfa19c93c0d48fc92af818536d953dc_Out_2_Float;
                                        Unity_Divide_float(_Split_0a9a61cc6c674fddabda59fd63f4aecd_B_3_Float, float(100), _Divide_4dfa19c93c0d48fc92af818536d953dc_Out_2_Float);
                                        float _Multiply_db95f8431cff4a03bf03ddea74d2d7d0_Out_2_Float;
                                        Unity_Multiply_float_float(_Divide_4dfa19c93c0d48fc92af818536d953dc_Out_2_Float, IN.TimeParameters.x, _Multiply_db95f8431cff4a03bf03ddea74d2d7d0_Out_2_Float);
                                        float2 _Vector2_8498eb561e89467bb77b3182fb06fdaa_Out_0_Vector2 = float2(float(0), _Multiply_db95f8431cff4a03bf03ddea74d2d7d0_Out_2_Float);
                                        float2 _TilingAndOffset_974e343188494870af4255bb30688d7f_Out_3_Vector2;
                                        Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Vector2_8498eb561e89467bb77b3182fb06fdaa_Out_0_Vector2, _TilingAndOffset_974e343188494870af4255bb30688d7f_Out_3_Vector2);
                                        float _Voronoi_ff93b035d5a745f0b2f0f1667671079b_Out_3_Float;
                                        float _Voronoi_ff93b035d5a745f0b2f0f1667671079b_Cells_4_Float;
                                        Unity_Voronoi_LegacySine_float(_TilingAndOffset_974e343188494870af4255bb30688d7f_Out_3_Vector2, IN.TimeParameters.x, _Split_0a9a61cc6c674fddabda59fd63f4aecd_R_1_Float, _Voronoi_ff93b035d5a745f0b2f0f1667671079b_Out_3_Float, _Voronoi_ff93b035d5a745f0b2f0f1667671079b_Cells_4_Float);
                                        float _Branch_d60484dcf9c244e2892f1ca3c61ee83a_Out_3_Float;
                                        Unity_Branch_float(_Property_f10053e85cc94fefbf8e444b3d6780a1_Out_0_Boolean, float(1), _Voronoi_ff93b035d5a745f0b2f0f1667671079b_Cells_4_Float, _Branch_d60484dcf9c244e2892f1ca3c61ee83a_Out_3_Float);
                                        float _Add_9007cf9ba70846068e82d2ed33f61d09_Out_2_Float;
                                        Unity_Add_float(_Saturate_e4061d55b2a149f99478d3263bde45ba_Out_1_Float, _Branch_d60484dcf9c244e2892f1ca3c61ee83a_Out_3_Float, _Add_9007cf9ba70846068e82d2ed33f61d09_Out_2_Float);
                                        float _Saturate_016ca84577d046e8a8a1cb20de7253da_Out_1_Float;
                                        Unity_Saturate_float(_Add_9007cf9ba70846068e82d2ed33f61d09_Out_2_Float, _Saturate_016ca84577d046e8a8a1cb20de7253da_Out_1_Float);
                                        float2 _Lerp_51600998daeb40feac8153411c3a83ce_Out_3_Vector2;
                                        Unity_Lerp_float2(_Add_fb5091e6ceb44f5b81fba17c4d07fd0a_Out_2_Vector2, (_ScreenPosition_6e7a3e6891194e06a01d93c268b093e2_Out_0_Vector4.xy), (_Saturate_016ca84577d046e8a8a1cb20de7253da_Out_1_Float.xx), _Lerp_51600998daeb40feac8153411c3a83ce_Out_3_Vector2);
                                        float4 _SampleTexture2D_854e343b48884a6ca8eb8f7e9b24d280_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_12099d2392e649e286b6d12ed71a609a_Out_0_Texture2D.tex, _Property_12099d2392e649e286b6d12ed71a609a_Out_0_Texture2D.samplerstate, _Property_12099d2392e649e286b6d12ed71a609a_Out_0_Texture2D.GetTransformedUV(_Lerp_51600998daeb40feac8153411c3a83ce_Out_3_Vector2));
                                        float _SampleTexture2D_854e343b48884a6ca8eb8f7e9b24d280_R_4_Float = _SampleTexture2D_854e343b48884a6ca8eb8f7e9b24d280_RGBA_0_Vector4.r;
                                        float _SampleTexture2D_854e343b48884a6ca8eb8f7e9b24d280_G_5_Float = _SampleTexture2D_854e343b48884a6ca8eb8f7e9b24d280_RGBA_0_Vector4.g;
                                        float _SampleTexture2D_854e343b48884a6ca8eb8f7e9b24d280_B_6_Float = _SampleTexture2D_854e343b48884a6ca8eb8f7e9b24d280_RGBA_0_Vector4.b;
                                        float _SampleTexture2D_854e343b48884a6ca8eb8f7e9b24d280_A_7_Float = _SampleTexture2D_854e343b48884a6ca8eb8f7e9b24d280_RGBA_0_Vector4.a;
                                        float4 _Property_bf2e8901ab444031993ca902415355c2_Out_0_Vector4 = _Color;
                                        float _Property_201b7f3de12d4866af33d0e460d5b258_Out_0_Float = _LerpColor;
                                        float4 _Lerp_a715175cd4f84509a746af21ef49de6c_Out_3_Vector4;
                                        Unity_Lerp_float4(_Property_bf2e8901ab444031993ca902415355c2_Out_0_Vector4, IN.VertexColor, (_Property_201b7f3de12d4866af33d0e460d5b258_Out_0_Float.xxxx), _Lerp_a715175cd4f84509a746af21ef49de6c_Out_3_Vector4);
                                        float _Property_3b1e131023804e32ab832254be308715_Out_0_Float = _OverrideColor;
                                        float _Power_9300c14b2e354b90ad6468d62f03bfa5_Out_2_Float;
                                        Unity_Power_float(_Voronoi_ff93b035d5a745f0b2f0f1667671079b_Out_3_Float, _Property_3b1e131023804e32ab832254be308715_Out_0_Float, _Power_9300c14b2e354b90ad6468d62f03bfa5_Out_2_Float);
                                        float _DDXY_0a363927fad749cf8b7c56f7d34d630c_Out_1_Float;
                                        Unity_DDXY_0a363927fad749cf8b7c56f7d34d630c_float(_Voronoi_ff93b035d5a745f0b2f0f1667671079b_Cells_4_Float, _DDXY_0a363927fad749cf8b7c56f7d34d630c_Out_1_Float);
                                        float _Add_ebdcea94022748aebc23abfa81d117d0_Out_2_Float;
                                        Unity_Add_float(_Power_9300c14b2e354b90ad6468d62f03bfa5_Out_2_Float, _DDXY_0a363927fad749cf8b7c56f7d34d630c_Out_1_Float, _Add_ebdcea94022748aebc23abfa81d117d0_Out_2_Float);
                                        float _Saturate_fa7adf9f48a149b981a3b7e5506fb910_Out_1_Float;
                                        Unity_Saturate_float(_Add_ebdcea94022748aebc23abfa81d117d0_Out_2_Float, _Saturate_fa7adf9f48a149b981a3b7e5506fb910_Out_1_Float);
                                        float _OneMinus_5bc69c1f3d7349428ca986cc7153eaaf_Out_1_Float;
                                        Unity_OneMinus_float(_Saturate_e4061d55b2a149f99478d3263bde45ba_Out_1_Float, _OneMinus_5bc69c1f3d7349428ca986cc7153eaaf_Out_1_Float);
                                        float _Multiply_443bc8972331494f8176c341356372aa_Out_2_Float;
                                        Unity_Multiply_float_float(_Saturate_fa7adf9f48a149b981a3b7e5506fb910_Out_1_Float, _OneMinus_5bc69c1f3d7349428ca986cc7153eaaf_Out_1_Float, _Multiply_443bc8972331494f8176c341356372aa_Out_2_Float);
                                        float _Branch_ee1d13b0fbbb4fd28a8c2418c89269e1_Out_3_Float;
                                        Unity_Branch_float(_Property_f10053e85cc94fefbf8e444b3d6780a1_Out_0_Boolean, float(0), _Multiply_443bc8972331494f8176c341356372aa_Out_2_Float, _Branch_ee1d13b0fbbb4fd28a8c2418c89269e1_Out_3_Float);
                                        float4 _Lerp_7a10e1beef344d34a75bae4642cb3ed3_Out_3_Vector4;
                                        Unity_Lerp_float4(_SampleTexture2D_854e343b48884a6ca8eb8f7e9b24d280_RGBA_0_Vector4, _Lerp_a715175cd4f84509a746af21ef49de6c_Out_3_Vector4, (_Branch_ee1d13b0fbbb4fd28a8c2418c89269e1_Out_3_Float.xxxx), _Lerp_7a10e1beef344d34a75bae4642cb3ed3_Out_3_Vector4);
                                        surface.BaseColor = (_Lerp_7a10e1beef344d34a75bae4642cb3ed3_Out_3_Vector4.xyz);
                                        return surface;
                                    }

                                    // --------------------------------------------------
                                    // Build Graph Inputs
                                    #ifdef HAVE_VFX_MODIFICATION
                                    #define VFX_SRP_ATTRIBUTES Attributes
                                    #define VFX_SRP_VARYINGS Varyings
                                    #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
                                    #endif
                                    VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                                    {
                                        VertexDescriptionInputs output;
                                        ZERO_INITIALIZE(VertexDescriptionInputs, output);

                                        output.ObjectSpaceNormal = input.normalOS;
                                        output.ObjectSpaceTangent = input.tangentOS.xyz;
                                        output.ObjectSpacePosition = input.positionOS;

                                        return output;
                                    }
                                    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                                    {
                                        SurfaceDescriptionInputs output;
                                        ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                                    #ifdef HAVE_VFX_MODIFICATION
                                    #if VFX_USE_GRAPH_VALUES
                                        uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
                                        /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
                                    #endif
                                        /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                                    #endif







                                        output.AbsoluteWorldSpacePosition = GetAbsolutePositionWS(input.positionWS);

                                        #if UNITY_UV_STARTS_AT_TOP
                                        output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x < 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
                                        #else
                                        output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x > 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
                                        #endif

                                        output.NDCPosition = output.PixelPosition.xy / _ScaledScreenParams.xy;
                                        output.NDCPosition.y = 1.0f - output.NDCPosition.y;

                                        output.uv0 = input.texCoord0;
                                        output.VertexColor = input.color;
                                        output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                    #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                                    #else
                                    #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                                    #endif
                                    #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                                            return output;
                                    }

                                    // --------------------------------------------------
                                    // Main

                                    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                                    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitGBufferPass.hlsl"

                                    // --------------------------------------------------
                                    // Visual Effect Vertex Invocations
                                    #ifdef HAVE_VFX_MODIFICATION
                                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
                                    #endif

                                    ENDHLSL
                                    }
                                    Pass
                                    {
                                        Name "SceneSelectionPass"
                                        Tags
                                        {
                                            "LightMode" = "SceneSelectionPass"
                                        }

                                        // Render State
                                        Cull Off

                                        // Debug
                                        // <None>

                                        // --------------------------------------------------
                                        // Pass

                                        HLSLPROGRAM

                                        // Pragmas
                                        #pragma target 2.0
                                        #pragma vertex vert
                                        #pragma fragment frag

                                        // Keywords
                                        // PassKeywords: <None>
                                        // GraphKeywords: <None>

                                        // Defines

                                        #define ATTRIBUTES_NEED_NORMAL
                                        #define ATTRIBUTES_NEED_TANGENT
                                        #define FEATURES_GRAPH_VERTEX
                                        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                                        #define SHADERPASS SHADERPASS_DEPTHONLY
                                        #define SCENESELECTIONPASS 1
                                        #define ALPHA_CLIP_THRESHOLD 1


                                        // custom interpolator pre-include
                                        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                                        // Includes
                                        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                                        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
                                        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                                        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
                                        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
                                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                                        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
                                        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

                                        // --------------------------------------------------
                                        // Structs and Packing

                                        // custom interpolators pre packing
                                        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                                        struct Attributes
                                        {
                                             float3 positionOS : POSITION;
                                             float3 normalOS : NORMAL;
                                             float4 tangentOS : TANGENT;
                                            #if UNITY_ANY_INSTANCING_ENABLED
                                             uint instanceID : INSTANCEID_SEMANTIC;
                                            #endif
                                        };
                                        struct Varyings
                                        {
                                             float4 positionCS : SV_POSITION;
                                            #if UNITY_ANY_INSTANCING_ENABLED
                                             uint instanceID : CUSTOM_INSTANCE_ID;
                                            #endif
                                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                            #endif
                                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                            #endif
                                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                            #endif
                                        };
                                        struct SurfaceDescriptionInputs
                                        {
                                        };
                                        struct VertexDescriptionInputs
                                        {
                                             float3 ObjectSpaceNormal;
                                             float3 ObjectSpaceTangent;
                                             float3 ObjectSpacePosition;
                                        };
                                        struct PackedVaryings
                                        {
                                             float4 positionCS : SV_POSITION;
                                            #if UNITY_ANY_INSTANCING_ENABLED
                                             uint instanceID : CUSTOM_INSTANCE_ID;
                                            #endif
                                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                            #endif
                                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                            #endif
                                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                            #endif
                                        };

                                        PackedVaryings PackVaryings(Varyings input)
                                        {
                                            PackedVaryings output;
                                            ZERO_INITIALIZE(PackedVaryings, output);
                                            output.positionCS = input.positionCS;
                                            #if UNITY_ANY_INSTANCING_ENABLED
                                            output.instanceID = input.instanceID;
                                            #endif
                                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                            #endif
                                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                            #endif
                                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                            output.cullFace = input.cullFace;
                                            #endif
                                            return output;
                                        }

                                        Varyings UnpackVaryings(PackedVaryings input)
                                        {
                                            Varyings output;
                                            output.positionCS = input.positionCS;
                                            #if UNITY_ANY_INSTANCING_ENABLED
                                            output.instanceID = input.instanceID;
                                            #endif
                                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                            #endif
                                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                            #endif
                                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                            output.cullFace = input.cullFace;
                                            #endif
                                            return output;
                                        }


                                        // --------------------------------------------------
                                        // Graph

                                        // Graph Properties
                                        CBUFFER_START(UnityPerMaterial)
                                        float4 _MainTex_TexelSize;
                                        float _OverrideColor;
                                        float4 _Color;
                                        float _DeActiveEffect;
                                        float4 _Waves;
                                        float _LerpColor;
                                        float2 _offsetFade;
                                        CBUFFER_END


                                            // Object and Global properties
                                            SAMPLER(SamplerState_Linear_Repeat);
                                            TEXTURE2D(_MainTex);
                                            SAMPLER(sampler_MainTex);
                                            TEXTURE2D(_GrabbedTexture);
                                            SAMPLER(sampler_GrabbedTexture);
                                            float4 _GrabbedTexture_TexelSize;

                                            // Graph Includes
                                            // GraphIncludes: <None>

                                            // -- Property used by ScenePickingPass
                                            #ifdef SCENEPICKINGPASS
                                            float4 _SelectionID;
                                            #endif

                                            // -- Properties used by SceneSelectionPass
                                            #ifdef SCENESELECTIONPASS
                                            int _ObjectId;
                                            int _PassValue;
                                            #endif

                                            // Graph Functions
                                            // GraphFunctions: <None>

                                            // Custom interpolators pre vertex
                                            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                                            // Graph Vertex
                                            struct VertexDescription
                                            {
                                                float3 Position;
                                                float3 Normal;
                                                float3 Tangent;
                                            };

                                            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                                            {
                                                VertexDescription description = (VertexDescription)0;
                                                description.Position = IN.ObjectSpacePosition;
                                                description.Normal = IN.ObjectSpaceNormal;
                                                description.Tangent = IN.ObjectSpaceTangent;
                                                return description;
                                            }

                                            // Custom interpolators, pre surface
                                            #ifdef FEATURES_GRAPH_VERTEX
                                            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                                            {
                                            return output;
                                            }
                                            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                                            #endif

                                            // Graph Pixel
                                            struct SurfaceDescription
                                            {
                                            };

                                            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                                            {
                                                SurfaceDescription surface = (SurfaceDescription)0;
                                                return surface;
                                            }

                                            // --------------------------------------------------
                                            // Build Graph Inputs
                                            #ifdef HAVE_VFX_MODIFICATION
                                            #define VFX_SRP_ATTRIBUTES Attributes
                                            #define VFX_SRP_VARYINGS Varyings
                                            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
                                            #endif
                                            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                                            {
                                                VertexDescriptionInputs output;
                                                ZERO_INITIALIZE(VertexDescriptionInputs, output);

                                                output.ObjectSpaceNormal = input.normalOS;
                                                output.ObjectSpaceTangent = input.tangentOS.xyz;
                                                output.ObjectSpacePosition = input.positionOS;

                                                return output;
                                            }
                                            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                                            {
                                                SurfaceDescriptionInputs output;
                                                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                                            #ifdef HAVE_VFX_MODIFICATION
                                            #if VFX_USE_GRAPH_VALUES
                                                uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
                                                /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
                                            #endif
                                                /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                                            #endif








                                                #if UNITY_UV_STARTS_AT_TOP
                                                #else
                                                #endif


                                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                                            #else
                                            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                                            #endif
                                            #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                                                    return output;
                                            }

                                            // --------------------------------------------------
                                            // Main

                                            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                                            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"

                                            // --------------------------------------------------
                                            // Visual Effect Vertex Invocations
                                            #ifdef HAVE_VFX_MODIFICATION
                                            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
                                            #endif

                                            ENDHLSL
                                            }
                                            Pass
                                            {
                                                Name "ScenePickingPass"
                                                Tags
                                                {
                                                    "LightMode" = "Picking"
                                                }

                                                // Render State
                                                Cull Back

                                                // Debug
                                                // <None>

                                                // --------------------------------------------------
                                                // Pass

                                                HLSLPROGRAM

                                                // Pragmas
                                                #pragma target 2.0
                                                #pragma vertex vert
                                                #pragma fragment frag

                                                // Keywords
                                                // PassKeywords: <None>
                                                // GraphKeywords: <None>

                                                // Defines

                                                #define ATTRIBUTES_NEED_NORMAL
                                                #define ATTRIBUTES_NEED_TANGENT
                                                #define FEATURES_GRAPH_VERTEX
                                                /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                                                #define SHADERPASS SHADERPASS_DEPTHONLY
                                                #define SCENEPICKINGPASS 1
                                                #define ALPHA_CLIP_THRESHOLD 1


                                                // custom interpolator pre-include
                                                /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                                                // Includes
                                                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                                                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
                                                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                                                #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
                                                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
                                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                                                #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
                                                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

                                                // --------------------------------------------------
                                                // Structs and Packing

                                                // custom interpolators pre packing
                                                /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                                                struct Attributes
                                                {
                                                     float3 positionOS : POSITION;
                                                     float3 normalOS : NORMAL;
                                                     float4 tangentOS : TANGENT;
                                                    #if UNITY_ANY_INSTANCING_ENABLED
                                                     uint instanceID : INSTANCEID_SEMANTIC;
                                                    #endif
                                                };
                                                struct Varyings
                                                {
                                                     float4 positionCS : SV_POSITION;
                                                    #if UNITY_ANY_INSTANCING_ENABLED
                                                     uint instanceID : CUSTOM_INSTANCE_ID;
                                                    #endif
                                                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                                    #endif
                                                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                                    #endif
                                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                                    #endif
                                                };
                                                struct SurfaceDescriptionInputs
                                                {
                                                };
                                                struct VertexDescriptionInputs
                                                {
                                                     float3 ObjectSpaceNormal;
                                                     float3 ObjectSpaceTangent;
                                                     float3 ObjectSpacePosition;
                                                };
                                                struct PackedVaryings
                                                {
                                                     float4 positionCS : SV_POSITION;
                                                    #if UNITY_ANY_INSTANCING_ENABLED
                                                     uint instanceID : CUSTOM_INSTANCE_ID;
                                                    #endif
                                                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                                    #endif
                                                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                                    #endif
                                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                                    #endif
                                                };

                                                PackedVaryings PackVaryings(Varyings input)
                                                {
                                                    PackedVaryings output;
                                                    ZERO_INITIALIZE(PackedVaryings, output);
                                                    output.positionCS = input.positionCS;
                                                    #if UNITY_ANY_INSTANCING_ENABLED
                                                    output.instanceID = input.instanceID;
                                                    #endif
                                                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                                    #endif
                                                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                                    #endif
                                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                    output.cullFace = input.cullFace;
                                                    #endif
                                                    return output;
                                                }

                                                Varyings UnpackVaryings(PackedVaryings input)
                                                {
                                                    Varyings output;
                                                    output.positionCS = input.positionCS;
                                                    #if UNITY_ANY_INSTANCING_ENABLED
                                                    output.instanceID = input.instanceID;
                                                    #endif
                                                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                                    #endif
                                                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                                    #endif
                                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                    output.cullFace = input.cullFace;
                                                    #endif
                                                    return output;
                                                }


                                                // --------------------------------------------------
                                                // Graph

                                                // Graph Properties
                                                CBUFFER_START(UnityPerMaterial)
                                                float4 _MainTex_TexelSize;
                                                float _OverrideColor;
                                                float4 _Color;
                                                float _DeActiveEffect;
                                                float4 _Waves;
                                                float _LerpColor;
                                                float2 _offsetFade;
                                                CBUFFER_END


                                                    // Object and Global properties
                                                    SAMPLER(SamplerState_Linear_Repeat);
                                                    TEXTURE2D(_MainTex);
                                                    SAMPLER(sampler_MainTex);
                                                    TEXTURE2D(_GrabbedTexture);
                                                    SAMPLER(sampler_GrabbedTexture);
                                                    float4 _GrabbedTexture_TexelSize;

                                                    // Graph Includes
                                                    // GraphIncludes: <None>

                                                    // -- Property used by ScenePickingPass
                                                    #ifdef SCENEPICKINGPASS
                                                    float4 _SelectionID;
                                                    #endif

                                                    // -- Properties used by SceneSelectionPass
                                                    #ifdef SCENESELECTIONPASS
                                                    int _ObjectId;
                                                    int _PassValue;
                                                    #endif

                                                    // Graph Functions
                                                    // GraphFunctions: <None>

                                                    // Custom interpolators pre vertex
                                                    /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                                                    // Graph Vertex
                                                    struct VertexDescription
                                                    {
                                                        float3 Position;
                                                        float3 Normal;
                                                        float3 Tangent;
                                                    };

                                                    VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                                                    {
                                                        VertexDescription description = (VertexDescription)0;
                                                        description.Position = IN.ObjectSpacePosition;
                                                        description.Normal = IN.ObjectSpaceNormal;
                                                        description.Tangent = IN.ObjectSpaceTangent;
                                                        return description;
                                                    }

                                                    // Custom interpolators, pre surface
                                                    #ifdef FEATURES_GRAPH_VERTEX
                                                    Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                                                    {
                                                    return output;
                                                    }
                                                    #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                                                    #endif

                                                    // Graph Pixel
                                                    struct SurfaceDescription
                                                    {
                                                    };

                                                    SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                                                    {
                                                        SurfaceDescription surface = (SurfaceDescription)0;
                                                        return surface;
                                                    }

                                                    // --------------------------------------------------
                                                    // Build Graph Inputs
                                                    #ifdef HAVE_VFX_MODIFICATION
                                                    #define VFX_SRP_ATTRIBUTES Attributes
                                                    #define VFX_SRP_VARYINGS Varyings
                                                    #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
                                                    #endif
                                                    VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                                                    {
                                                        VertexDescriptionInputs output;
                                                        ZERO_INITIALIZE(VertexDescriptionInputs, output);

                                                        output.ObjectSpaceNormal = input.normalOS;
                                                        output.ObjectSpaceTangent = input.tangentOS.xyz;
                                                        output.ObjectSpacePosition = input.positionOS;

                                                        return output;
                                                    }
                                                    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                                                    {
                                                        SurfaceDescriptionInputs output;
                                                        ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                                                    #ifdef HAVE_VFX_MODIFICATION
                                                    #if VFX_USE_GRAPH_VALUES
                                                        uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
                                                        /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
                                                    #endif
                                                        /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                                                    #endif








                                                        #if UNITY_UV_STARTS_AT_TOP
                                                        #else
                                                        #endif


                                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                    #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                                                    #else
                                                    #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                                                    #endif
                                                    #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                                                            return output;
                                                    }

                                                    // --------------------------------------------------
                                                    // Main

                                                    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                                                    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"

                                                    // --------------------------------------------------
                                                    // Visual Effect Vertex Invocations
                                                    #ifdef HAVE_VFX_MODIFICATION
                                                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
                                                    #endif

                                                    ENDHLSL
                                                    }
    }
        CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
                                                        CustomEditorForRenderPipeline "UnityEditor.ShaderGraphUnlitGUI" "UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset"
                                                        FallBack "Hidden/Shader Graph/FallbackError"
}
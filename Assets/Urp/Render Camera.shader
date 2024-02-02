Shader "Custom/MultiRenderTexture" 
{
    Properties
    {
        _MainTex0("Render Texture Main", 2D) = "white" {}
        _position0("camara activa Main", FLOAT) = 1
        _MainTex1("Render Texture 1", 2D) = "white" {}
        _position1("camara activa 1", FLOAT) = 1
        _MainTex2("Render Texture 2", 2D) = "white" {}
        _position2("camara activa 2", FLOAT) = 1
        _MainTex3("Render Texture 3", 2D) = "white" {}
        _position3("camara activa 3", FLOAT) = 1
        _MainTex4("Render Texture 4", 2D) = "white" {}
        _position4("camara activa 4", FLOAT) = 1
        _MainTex5("Render Texture 5", 2D) = "white" {}
        _position5("camara activa 5", FLOAT) = 1
        _MainTex6("Render Texture 6", 2D) = "white" {}
        _position6("camara activa 6", FLOAT) = 1



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

                sampler2D _Texs[7];
                sampler2D _MainTex1;
                sampler2D _MainTex2;
                sampler2D _MainTex3;
                sampler2D _MainTex4;
                sampler2D _MainTex5;
                sampler2D _MainTex6;
                sampler2D _MainTex0;

                
                float _pos[7];
                float _position1;
                float _position2;
                float _position3;
                float _position4;
                float _position5;
                float _position6;
                float _position0;
                
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
                    float4 color = float4(1, 1, 1, 1);

                    _Texs[0] = _MainTex0;
                    _Texs[1] = _MainTex1;
                    _Texs[2] = _MainTex2;
                    _Texs[3] = _MainTex3;
                    _Texs[4] = _MainTex4;
                    _Texs[5] = _MainTex5;
                    _Texs[6] = _MainTex6;

                    _pos[0] = _position0;
                    _pos[1] = _position1;
                    _pos[2] = _position2;
                    _pos[3] = _position3;
                    _pos[4] = _position4;
                    _pos[5] = _position5;
                    _pos[6] = _position6;

                    float depthMax = 0;

                    
                    for (int index =0; index < 7; index++)
                    {
                        if (_pos[index] == 1)
                        {
                            float4 colorAux = tex2D(_Texs[index], i.uv);

                            if (depthMax < colorAux.a)
                            {
                                depthMax = colorAux.a;
                                colorAux.a = 1;
                                color = colorAux;
                            }
                        }
                    }
                    
                    return color;
                    

                    //float a = tex2D(_Texs[6], i.uv).w;

                    //return tex2D(_Texs[6], i.uv);
                }

                ENDHLSL
            }
        }
}
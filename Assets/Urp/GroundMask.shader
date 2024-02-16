Shader "Unlit/GroundMask"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        ColorMask 0
        ZWrite Off

        Stencil
        {
            ref 1
            comp always
            pass replace
        }

        Pass
        {

        }
    }
}

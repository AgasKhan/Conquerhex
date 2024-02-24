Shader "Unlit/UniversalMask"
{
    Properties
    {
        _StencilMask ("Stencil mask write", int) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        ColorMask 0
        ZWrite Off

        Stencil
        {
            ref [_StencilMask]
            comp always
            pass replace
        }

        Pass
        {

        }
    }
}

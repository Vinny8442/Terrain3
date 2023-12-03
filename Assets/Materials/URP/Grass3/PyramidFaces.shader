Shader "Custom/PyramidFaces"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PyramidHeight ("Pyramid height", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RendererPipeline" = "UniversalPipeline" }

        Pass
        {
            Name "ForwardLit"
            Tags {"LightMode" = "UniversalForward"}
            Cull Back
            
            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0
            #pragma require geometry

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFTr

            #pragma vertex Vertex
            #pragma geometry Geometry
            #pragma fragment Fragment

            // #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
            #include "PyramidFaces.hlsl"

            ENDHLSL
        }
    }
}

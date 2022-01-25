// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "MyShader/MyCustomShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1, 1, 1, 1)
        _Smoothness("Smoothness", Range(0, 99)) = 0
        [Gamma] _Metallic("Metallic", Range(0, 1)) = 0
        _SpecularTint ("Specular", Color) = (0.5, 0.5, 0.5)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
			Tags {
				"LightMode" = "ForwardBase"
			}
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex VertexSub
            #pragma fragment FragmentSub
            
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"
            #include "UnityShaderVariables.cginc"
        
            #include "MyCustomShaderLighting.cginc"
            ENDCG
        }

        Pass
        {
			Tags {
				"LightMode" = "ForwardAdd"
			}
			
			Blend One One
			ZWrite Off
			
            CGPROGRAM
            #pragma target 3.0
            #pragma multi_compile DIRECTIONAL POINT SPOT 
            #pragma multi_compile _ VERTEXLIGHT_ON
            #pragma vertex VertexSub
            #pragma fragment FragmentSub
            
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"
            #include "UnityShaderVariables.cginc"
        
            //#define POINT
            #include "MyCustomShaderLighting.cginc"
            ENDCG
        }
        
        /*Pass {
			Tags {
				"LightMode" = "ForwardAdd"
			}

			Blend One One
			ZWrite Off

			CGPROGRAM

			#pragma target 3.0

			#pragma vertex VertextSub
            #pragma fragment FragmentSub
            
            // make fog work
            //#pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityLightingCommon.cginc"
            #include "UnityShaderVariables.cginc"
        
            #include "MyCustomShaderLighting.cginc"

			ENDCG
		}*/
    }
}

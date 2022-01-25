// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

#include "AutoLight.cginc"
#include "UnityPBSLighting.cginc"

float4 _Color;
sampler2D _MainTex;
float _Smoothness;
float _Metallic;
float3 _SpecularTint;
float4 _MainTex_ST;

struct VertexSubInput
{
    float4 position:POSITION;
    float2 uv:TEXCOORD0;
    float3 normal:NORMAL;
};

struct VertSubResult
{
    float2 uv:TEXCOORD0;
    float3 normal:NORMAL;
    float3 worldPosition:TEXCOORD2;
    
    #if defined(VERTEXLIGHT_ON)
    float3 vertexLightColor :TEXCOORD3; 
    #endif
    
    float4 position:POSITION;
};

float4 Abs(in float4 value)
{
    return float4(value.x < 0 ? -value.x : value.x, value.y < 0 ? -value.y : value.y, value.z < 0 ? -value.z : value.z,
                  value.a < 0 ? -value.a : value.a);
}

float Quant(float f, float colorQuantity)
{
    int i = (int)(f * colorQuantity);
    return (float)i / colorQuantity;
}

float3 Quant(float3 v, float colorQuantity)
{
    return float3(Quant(v.x, colorQuantity), Quant(v.y, colorQuantity), Quant(v.z, colorQuantity));
}

float4 Quant(float4 v, float colorQuantity)
{
    return float4(Quant(v.x, colorQuantity), Quant(v.y, colorQuantity), Quant(v.z, colorQuantity),
                  Quant(v.w, colorQuantity));
}

void ComputeVertexLightColor (inout VertSubResult i) {
    #if defined(VERTEXLIGHT_ON)
    float3 lightPos = float3(
        unity_4LightPosX0.x, unity_4LightPosY0.x, unity_4LightPosZ0.x
    );
    float3 lightVec = lightPos - i.worldPosition;
    float3 lightDir = normalize(lightVec);
    float ndotl = DotClamped(i.normal, lightDir);
    float attenuation = 1 / (1 + dot(lightVec, lightVec) * unity_4LightAtten0.x);
    i.vertexLightColor = unity_LightColor[0].rgb * ndotl * attenuation;
    #endif
}

UnityIndirect CreateIndirect(VertSubResult input)
{
    UnityIndirect result;

    result.diffuse = half3(0, 0, 0);
    result.specular = half3(0, 0, 0);
    #if defined(VERTEXLIGHT_ON)
    result.specular = input.vertexLightColor;//half3(0, 1, 1);
    #endif

    return result;
}

UnityLight CreateLight(VertSubResult input)
{
    UnityLight light;
    #if defined(POINT) || defined(SPOT)
        float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz - input.worldPosition);
    #else
        float3 lightDirection = _WorldSpaceLightPos0.xyz;
    #endif
    UNITY_LIGHT_ATTENUATION(attenuation, 0, input.worldPosition);
    /*float3 lightVec = _WorldSpaceLightPos0.xyz - input.worldPosition;
    attenuation = 1 / (1+dot(lightVec, lightVec));/**/
    light.color = _LightColor0.rgb * attenuation;
    light.dir = lightDirection;
    light.ndotl = DotClamped(input.normal, light.dir);
    return light;
}

VertSubResult VertexSub(VertexSubInput input)
{
    VertSubResult result;
    result.uv = TRANSFORM_TEX(input.uv, _MainTex);
    result.position = UnityObjectToClipPos(input.position);
    result.normal = UnityObjectToWorldNormal(input.normal);
    result.worldPosition = mul(unity_ObjectToWorld, input.position);
    ComputeVertexLightColor(result);
    return result;
}

float4 FragmentSub(in VertSubResult input):SV_TARGET
{
    input.normal = normalize(input.normal);
    // input.normal = Quant(input.normal, 20);
    float3 albedoColor = float3(1, 1, 1);
    tex2D(_MainTex, input.uv).rgb;

    // float3 lightDir = _WorldSpaceLightPos0.xyz;
    // float3 lightIntensity = DotClamped(lightDir, input.normal);
    // float3 lightColor = _LightColor0.rgb;

    float3 viewDirection = normalize(_WorldSpaceCameraPos - input.worldPosition);
    // float3 reflectionDirection = reflect(-lightDir, input.normal);
    // float3 reflectionIntensity = pow(DotClamped(reflectionDirection, viewDirection), _Smoothness);// * _SpecularTint;
    //albedoColor *= (1 - reflectionIntensity);
    //return float4( lightIntensity * (lightColor * albedoColor) + reflectionIntensity, 1);

    float oneMinusReflectivity; // = 1 - _Metallic;
    float3 specularTint;

    UnityLight lightData = CreateLight(input);

    UnityIndirect indirectLightData = CreateIndirect(input);

    albedoColor = DiffuseAndSpecularFromMetallic(albedoColor, _Metallic, specularTint, oneMinusReflectivity);
    return UNITY_BRDF_PBS(albedoColor, specularTint, oneMinusReflectivity, _Smoothness / 100, input.normal,
                          viewDirection, lightData, indirectLightData);
    //return Quant(finalColor, _ColorQuant);
}

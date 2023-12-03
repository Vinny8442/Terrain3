#ifndef PYRAMIDFACES_INCLUDED
#define PYRAMIDFACES_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "NMGGeometryHelpers.hlsl"

struct Attributes
{
    float4 positionOS    :POSITION;
    float3 normalOS      :NORMAL;
    float4 tangentOS     :TANGENT;
    float2 uv            :TEXCOORD0;
};


struct VertexOutput
{
    float3 positionWS : TEXCOORD0;
    float2 uv : TEXCOORD1;
};


struct GeometryOutput
{
    float3 positionWS : TEXCOORD0;
    float3 normalWS : TEXCOORD1;
    float2 uv : TEXCOORD2;

    float4 positionCS : SV_POSITION;
};


TEXTURE2D(_MainTex);
SAMPLER(sampler_MainTex);
float4 _MainTex_ST;

float _PyramidHeight;

VertexOutput Vertex(Attributes input)
{
    VertexOutput output = (VertexOutput)0;
    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
    output.positionWS = vertexInput.positionWS;

    output.uv = TRANSFORM_TEX(input.uv, _MainTex);
    return output;
}

GeometryOutput SetupVertex(float3 positionWS, float3 normal, float2 uv)
{
    GeometryOutput result = (GeometryOutput)0;
    result.positionWS = positionWS;
    result.normalWS = normal;
    result.uv = uv;
    result.positionCS = TransformWorldToHClip(positionWS);
    return result;
}

void SetupAndOutputTriangle(inout TriangleStream<GeometryOutput> stream, VertexOutput a, VertexOutput b, VertexOutput c)
{
    stream.RestartStrip();
    float3 normal = GetNormalFromTriangle(a.positionWS, b.positionWS, c.positionWS);
    stream.Append(SetupVertex(a.positionWS, normal, a.uv));
    stream.Append(SetupVertex(b.positionWS, normal, b.uv));
    stream.Append(SetupVertex(c.positionWS, normal, c.uv));
}


[maxvertexcount(9)]
void Geometry(triangle VertexOutput inputs[3], inout TriangleStream<GeometryOutput> outputStream)
{
    VertexOutput center = (VertexOutput)0;
    const float3 normal = GetNormalFromTriangle(inputs[0].positionWS, inputs[1].positionWS, inputs[2].positionWS);
    center.positionWS = GetTriangleCenter(inputs[0].positionWS, inputs[1].positionWS, inputs[2].positionWS + normal * _PyramidHeight);
    center.uv = GetTriangleCenter(inputs[0].uv, inputs[1].uv, inputs[2].uv);

    SetupAndOutputTriangle(outputStream, inputs[0], inputs[1], center);
    SetupAndOutputTriangle(outputStream, inputs[1], inputs[2], center);
    SetupAndOutputTriangle(outputStream, inputs[2], inputs[0], center);
}


float4 Fragment(GeometryOutput input):SV_Target{
    InputData inputData = (InputData)0;
    inputData.positionWS = input.positionWS;
    
    inputData.normalWS = NormalizeNormalPerPixel(input.normalWS);
    inputData.viewDirectionWS = GetViewDirectionFromPosition(input.positionWS);
    inputData.shadowCoord = CalculateShadowCoord(input.positionWS, input.positionCS);
    inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);
    inputData.shadowMask = SAMPLE_SHADOWMASK(input.staticLightmapUV);
    
    SurfaceData surfaceData = (SurfaceData)0;
    surfaceData.albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv).rgb;

    return UniversalFragmentBlinnPhong(inputData, surfaceData);
}

#endif
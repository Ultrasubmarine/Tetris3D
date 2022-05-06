// MIT License

// Copyright (c) 2020 NedMakesGames

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#ifndef BACKFACEOUTLINES_INCLUDED
#define BACKFACEOUTLINES_INCLUDED

// Include helper functions from URP
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

// Data from the meshes
struct Attributes {
    float4 positionOS       : POSITION; // Position in object space
    float3 normalOS         : NORMAL; // Normal vector in object space
#ifdef USE_PRECALCULATED_OUTLINE_NORMALS
    float3 smoothNormalOS   : TEXCOORD1; // Calculated "smooth" normals to extrude along in object space
#endif
};

// Output from the vertex function and input to the fragment function
struct VertexOutput {
    float4 positionCS   : SV_POSITION; // Position in clip space
};

// Properties
float _Thickness;
float4 _Color;
float _DepthOffset;

VertexOutput Vertex(Attributes input) {
    VertexOutput output = (VertexOutput)0;

    float3 normalOS;
#ifdef USE_PRECALCULATED_OUTLINE_NORMALS
    normalOS = input.smoothNormalOS;
#else
    normalOS = input.normalOS;
#endif

    // Extrude the object space position along a normal vector
    float3 posOS = input.positionOS.xyz + normalOS * _Thickness;
    // Convert this position to world and clip space
    output.positionCS = GetVertexPositionInputs(posOS).positionCS;

    float depthOffset = _DepthOffset;
    // If depth is reversed on this platform, reverse the offset
#ifdef UNITY_REVERSED_Z
    depthOffset = -depthOffset;
#endif
    output.positionCS.z += depthOffset;

    return output;
}

float4 Fragment(VertexOutput input) : SV_Target {
    return _Color;
}

#endif
{\rtf1\ansi\ansicpg1252\cocoartf2822
\cocoatextscaling0\cocoaplatform0{\fonttbl\f0\fswiss\fcharset0 Helvetica;}
{\colortbl;\red255\green255\blue255;}
{\*\expandedcolortbl;;}
\paperw11900\paperh16840\margl1440\margr1440\vieww11520\viewh8400\viewkind0
\pard\tx720\tx1440\tx2160\tx2880\tx3600\tx4320\tx5040\tx5760\tx6480\tx7200\tx7920\tx8640\pardirnatural\partightenfactor0

\f0\fs24 \cf0 Shader "Custom/UnlitWater"\
\{\
    Properties\
    \{\
        _BaseColor ("Base Color", Color) = (0.18,0.47,1,0.6)\
        _ShallowColor ("Shallow Color", Color) = (0.4,0.7,0.3,0.6)\
        _DeepColor ("Deep Color", Color) = (0.1,0.3,0.1,0.8)\
        _Transparency ("Transparency", Range(0,1)) = 0.6\
        _NormalMap ("Normal Map", 2D) = "bump" \{\}\
        _NormalStrength ("Normal Strength", Range(0,1)) = 0.2\
        _WaveSpeed ("Wave Speed", Range(0,2)) = 0.3\
        _WaveTiling ("Wave Tiling", Range(0,10)) = 2\
    \}\
\
    SubShader\
    \{\
        Tags \{ "RenderType"="Transparent" "Queue"="Transparent" \}\
        LOD 100\
        Cull Off\
        ZWrite Off\
        Blend SrcAlpha OneMinusSrcAlpha\
\
        Pass\
        \{\
            HLSLPROGRAM\
            #pragma vertex vert\
            #pragma fragment frag\
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"\
\
            struct Attributes\
            \{\
                float4 positionOS : POSITION;\
                float2 uv : TEXCOORD0;\
            \};\
\
            struct Varyings\
            \{\
                float4 positionCS : SV_POSITION;\
                float2 uv : TEXCOORD0;\
            \};\
\
            sampler2D _NormalMap;\
            float4 _BaseColor;\
            float4 _ShallowColor;\
            float4 _DeepColor;\
            float _Transparency;\
            float _NormalStrength;\
            float _WaveSpeed;\
            float _WaveTiling;\
            float _Time;\
\
            Varyings vert(Attributes IN)\
            \{\
                Varyings OUT;\
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);\
                OUT.uv = IN.uv * _WaveTiling + float2(_Time * _WaveSpeed, _Time * _WaveSpeed);\
                return OUT;\
            \}\
\
            float4 frag(Varyings IN) : SV_Target\
            \{\
                // Sample normal map\
                float3 normalSample = UnpackNormal(tex2D(_NormalMap, IN.uv));\
                normalSample.xy *= _NormalStrength;\
\
                // Fake depth-based color (Y axis approximation)\
                float depthFactor = saturate(IN.positionCS.y * 1.5);\
                float4 depthColor = lerp(_ShallowColor, _DeepColor, depthFactor);\
\
                // Mix with base color\
                float4 color = lerp(_BaseColor, depthColor, 0.5);\
\
                // Fake lighting using normals\
                color.rgb += normalSample.xy * 0.1;\
\
                // Apply transparency\
                color.a = _Transparency;\
\
                return color;\
            \}\
            ENDHLSL\
        \}\
    \}\
\}\
\
\
}
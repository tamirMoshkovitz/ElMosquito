Shader "Custom/MM_master_material_01_HDRP"
{
    Properties
    {
        _Albedo("Albedo", 2D) = "white" {}
        _AlbedoColorTint("Albedo Color Tint", Color) = (1,1,1,1)
        [Toggle]_AlbedoMap("Use Albedo Map", Float) = 1
        _AlbedoColorVertexTint("Albedo Color Vertex Tint", Color) = (1,1,1,1)
        [Toggle]_UseVertexPaintedTint("Use Vertex Painted Tint", Float) = 0
        [Toggle]_AmbientOcclusion("Use Ambient Occlusion", Float) = 1
        [Toggle]_CustomEmissiveTexture("Use Custom Emissive Texture", Float) = 0
        _EmissiveColorMulti("Emissive Color Multiplier", Color) = (1,1,1,1)
        [Toggle]_EmissiveOn("Enable Emission", Float) = 0
        _Emissive("Emissive Map", 2D) = "white" {}
        _RMA("RMA Map (R=Roughness, G=Metallic, A=AO)", 2D) = "white" {}
        _RMATextureCoord("RMA Texture Coord", Range(0,1)) = 0
        [Toggle]_MetalnessMap("Use Metalness Map", Float) = 1
        _MetalnessValue("Metalness Value", Range(0,1)) = 0
        _MetalnessValueMulti("Metalness Value Multiplier", Range(0,1)) = 1
        _Normal("Normal Map", 2D) = "bump" {}
        [Toggle]_NormalMap("Use Normal Map", Float) = 1
        _NormalMulti("Normal Map Intensity", Vector) = (1,1,1,1)
        [Toggle]_NoOpacity("No Opacity", Float) = 1
        [Toggle]_OpacityinAlbedoAlpha("Opacity in Albedo Alpha", Float) = 0
        _OpacityMap("Opacity Map", 2D) = "white" {}
        [Toggle]_MicroRoughnessDetail("Micro Roughness Detail", Float) = 1
        _MicroRoughnessMulti("Micro Roughness Multiplier", Range(0,10)) = 5
        _MicroRoughnessPower("Micro Roughness Power", Range(0,5)) = 1
        _RoughnessmicroDetail("Roughness Micro Detail Map", 2D) = "white" {}
        _RoughnessMulti("Roughness Multiplier", Range(0,2)) = 1
        _RougnhnessPower("Roughness Power", Range(0,2)) = 0.1
        _TextureCoordMicro("Texture Coord Micro", Float) = 1
        _OpacityMap_ST("Opacity Map UV Transform", Vector) = (1,1,0,0)
        _Albedo_ST("Albedo UV Transform", Vector) = (1,1,0,0)
        _Normal_ST("Normal UV Transform", Vector) = (1,1,0,0)
        _Emissive_ST("Emissive UV Transform", Vector) = (1,1,0,0)
    }

    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"

    TEXTURE2D(_Albedo);
    SAMPLER(sampler_Albedo);
    TEXTURE2D(_Normal);
    SAMPLER(sampler_Normal);
    TEXTURE2D(_Emissive);
    SAMPLER(sampler_Emissive);
    TEXTURE2D(_RMA);
    SAMPLER(sampler_RMA);
    TEXTURE2D(_OpacityMap);
    SAMPLER(sampler_OpacityMap);
    TEXTURE2D(_RoughnessmicroDetail);
    SAMPLER(sampler_RoughnessmicroDetail);

    float4 _AlbedoColorTint;
    float4 _AlbedoColorVertexTint;
    float _AlbedoMap;
    float _UseVertexPaintedTint;
    float _AmbientOcclusion;
    float _CustomEmissiveTexture;
    float4 _EmissiveColorMulti;
    float _EmissiveOn;
    float _RMATextureCoord;
    float _MetalnessMap;
    float _MetalnessValue;
    float _MetalnessValueMulti;
    float _MicroRoughnessDetail;
    float _RoughnessMulti;
    float _RougnhnessPower;
    // float _MicroRoughnessMulti;
    // float _MicroRoughnessPower;
    float _TextureCoordMicro;
    float _NoOpacity;
    float _OpacityinAlbedoAlpha;
    float _NormalMap;
    float4 _NormalMulti;

    float4 _Albedo_ST;
    float4 _Normal_ST;
    float4 _Emissive_ST;
    float4 _OpacityMap_ST;

    UNITY_INSTANCING_BUFFER_START(MM_master_material_01)
        UNITY_DEFINE_INSTANCED_PROP(float, _MicroRoughnessMulti)
        UNITY_DEFINE_INSTANCED_PROP(float, _MicroRoughnessPower)
    UNITY_INSTANCING_BUFFER_END(MM_master_material_01)

    struct Attributes
    {
        float4 positionOS   : POSITION;
        float3 normalOS     : NORMAL;
        float4 tangentOS    : TANGENT;
        float2 uv           : TEXCOORD0;
        float4 color        : COLOR;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct Varyings
    {
        float4 positionCS   : SV_POSITION;
        float3 normalWS     : NORMAL;
        float3 tangentWS    : TANGENT;
        float3 bitangentWS  : BINORMAL;
        float2 uvAlbedo     : TEXCOORD0;
        float2 uvNormal     : TEXCOORD1;
        float2 uvEmissive   : TEXCOORD2;
        float2 uvRMA        : TEXCOORD3;
        float2 uvOpacity    : TEXCOORD4;
        float2 uvRoughnessMicro : TEXCOORD5;
        float4 color        : COLOR;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    Varyings Vert(Attributes IN)
    {
        Varyings OUT;
        UNITY_SETUP_INSTANCE_ID(IN);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

        float3 worldPos = TransformObjectToWorld(IN.positionOS.xyz);
        OUT.positionCS = TransformWorldToHClip(worldPos);

        float3 normalWS = TransformObjectToWorldNormal(IN.normalOS);
        float3 tangentWS = TransformObjectToWorldDir(IN.tangentOS.xyz);
        float tangentSign = IN.tangentOS.w;
        float3 bitangentWS = cross(normalWS, tangentWS) * tangentSign;

        OUT.normalWS = normalWS;
        OUT.tangentWS = tangentWS;
        OUT.bitangentWS = bitangentWS;

        OUT.uvAlbedo = IN.uv * _Albedo_ST.xy + _Albedo_ST.zw;
        OUT.uvNormal = IN.uv * _Normal_ST.xy + _Normal_ST.zw;
        OUT.uvEmissive = IN.uv * _Emissive_ST.xy + _Emissive_ST.zw;
        OUT.uvRMA = IN.uv * _RMATextureCoord;
        OUT.uvOpacity = IN.uv * _OpacityMap_ST.xy + _OpacityMap_ST.zw;
        OUT.uvRoughnessMicro = IN.uv * _TextureCoordMicro;

        OUT.color = IN.color;

        return OUT;
    }

    float4 Fragment(Varyings IN) : SV_Target
    {
        UNITY_SETUP_INSTANCE_ID(IN);

        float3 baseColor = float3(1,1,1);
        if (_AlbedoMap > 0.5)
            baseColor = SAMPLE_TEXTURE2D(_Albedo, sampler_Albedo, IN.uvAlbedo).rgb;

        float4 albedoTint = lerp(_AlbedoColorTint, _AlbedoColorVertexTint, _UseVertexPaintedTint > 0.5 ? IN.color.g : 0);
        baseColor *= albedoTint.rgb;

        float3 normalWS = IN.normalWS;
        if (_NormalMap > 0.5)
        {
            float3 normalSample = SAMPLE_TEXTURE2D(_Normal, sampler_Normal, IN.uvNormal).xyz * 2 - 1;
            normalSample.xy *= _NormalMulti.xy;
            normalWS = normalize(normalSample.x * IN.tangentWS + normalSample.y * IN.bitangentWS + normalSample.z * IN.normalWS);
        }

        float4 rmaSample = SAMPLE_TEXTURE2D(_RMA, sampler_RMA, IN.uvRMA);
        float ao = _AmbientOcclusion > 0.5 ? rmaSample.a : 1.0;
        float metallic = (_MetalnessMap > 0.5) ? rmaSample.g * _MetalnessValueMulti : _MetalnessValue * _MetalnessValueMulti;
        float smoothness = 1.0 - (rmaSample.r * _RoughnessMulti);

        float3 emissive = float3(0,0,0);
        if (_EmissiveOn > 0.5)
        {
            emissive = _CustomEmissiveTexture > 0.5 ? SAMPLE_TEXTURE2D(_Emissive, sampler_Emissive, IN.uvEmissive).rgb : float3(1,1,1);
            emissive *= _EmissiveColorMulti.rgb;
        }

        // Simple output combining albedo and emissive
        float3 colorOut = baseColor + emissive;

        return float4(colorOut, 1.0);
    }
    ENDHLSL

    SubShader
    {
        Tags { "RenderPipeline" = "HDRenderPipeline" }
        Pass
        {
            Name "Forward"
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Fragment
            ENDHLSL
        }
    }
}
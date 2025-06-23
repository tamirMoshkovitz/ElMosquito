Shader "Custom/HDRP_Glass_ChromAberration"
{
    Properties
    {
        _GlassColor("Glass Color", Color) = (1,1,1,0)
        _ChromaticAberration("Chromatic Aberration", Range(0, 0.3)) = 0.1
        _RoughnessValue("Roughness Value", Range(0, 1)) = 0.5
        _Opacity("Opacity", Range(0, 1)) = 0.5
        _Reflectivity("Reflectivity", Range(0, 1)) = 1
        _Refraction("Refraction", Range(0, 1.5)) = 1.3
        _EmissiveMulti("Emissive Multi", Range(0, 0.05)) = 0.01
        _SwirlsMulti("Swirls Multi", Range(0, 20)) = 2
        _RoughnessMap("Roughness Map", 2D) = "white" {}
        _SwirlsMap("Swirls Map", 2D) = "white" {}
        _Cubemap("Texture", 2D) = "white" {}
    }
    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"

    // Uniforms
    float4 _GlassColor;
    float _ChromaticAberration;
    float _RoughnessValue;
    float _Opacity;
    float _Reflectivity;
    float _Refraction;
    float _EmissiveMulti;
    float _SwirlsMulti;
    TEXTURE2D(_RoughnessMap);
    SAMPLER(sampler_RoughnessMap);
    TEXTURE2D(_SwirlsMap);
    SAMPLER(sampler_SwirlsMap);
    TEXTURECUBE(_Cubemap);
    SAMPLER(sampler_Cubemap);

    ENDHLSL

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200

        Pass
        {
            Name "Forward"
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Back
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float2 uv : TEXCOORD2;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                // Object to World transforms:
                output.positionWS = TransformObjectToWorld(input.positionOS);
                output.normalWS = normalize(TransformObjectToWorldNormal(input.normalOS));
                output.positionHCS = TransformWorldToHClip(output.positionWS);
                output.uv = input.uv;
                return output;
            }

            float4 Frag(Varyings input) : SV_Target
            {
                // Sample roughness and swirl masks
                float roughnessSample = SAMPLE_TEXTURE2D(_RoughnessMap, sampler_RoughnessMap, input.uv).g;
                float roughness = saturate(_RoughnessValue * roughnessSample);

                float swirls = (1.0 - SAMPLE_TEXTURE2D(_SwirlsMap, sampler_SwirlsMap, input.uv).r) * _SwirlsMulti;
                float3 emissive = swirls * _EmissiveMulti;

                // View direction
                float3 viewDir = normalize(_WorldSpaceCameraPos - input.positionWS);

                // Reflection from cubemap
                float3 reflectDir = reflect(-viewDir, input.normalWS);
                float3 reflection = SAMPLE_TEXTURECUBE(_Cubemap, sampler_Cubemap, reflectDir).rgb;

                // Simple refraction approximation (no grab texture in this simplified example)
                float3 refractionColor = _GlassColor.rgb;

                // Mix colors
                float3 color = lerp(_GlassColor.rgb, reflection * 0.2 + float3(0.2, 0.2, 0.12), _Reflectivity);
                color = lerp(color, refractionColor, _Opacity);

                // Add emissive
                color += emissive;

                return float4(color, _Opacity);
            }
            ENDHLSL
        }
    }
    FallBack "HDRP/Unlit"
}
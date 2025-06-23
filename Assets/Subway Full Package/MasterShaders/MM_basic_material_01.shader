Shader "HDRP/MM_basic_material_01_HDRP"
{
    Properties
    {
        _ALB("Albedo", 2D) = "white" {}
        _NRM("Normal", 2D) = "bump" {}
        _RMA("RMA (Roughness, Metallic, AO)", 2D) = "black" {}
        _Cutoff("Alpha Cutoff", Range(0,1)) = 0.5
        _RoughnessMultiply("Roughness Multiply", Float) = 1.0
    }

    HLSLINCLUDE
    #pragma target 4.5
    #pragma vertex Vert
    #pragma fragment Frag

    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Builtin/BuiltinData.hlsl"

    TEXTURE2D(_ALB);       SAMPLER(sampler_ALB);
    TEXTURE2D(_NRM);       SAMPLER(sampler_NRM);
    TEXTURE2D(_RMA);       SAMPLER(sampler_RMA);

    float4 _ALB_ST;
    float4 _NRM_ST;
    float4 _RMA_ST;
    float _Cutoff;
    float _RoughnessMultiply;

    struct Attributes
    {
        float3 positionOS   : POSITION;
        float3 normalOS     : NORMAL;
        float4 tangentOS    : TANGENT;
        float2 uv           : TEXCOORD0;
    };

    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float3 positionWS : TEXCOORD0;
        float2 uv         : TEXCOORD1;
        float3 normalWS   : TEXCOORD2;
        float3 tangentWS  : TEXCOORD3;
        float3 bitangentWS: TEXCOORD4;
    };

    Varyings Vert(Attributes IN)
    {
        Varyings OUT;
        float3 positionWS = TransformObjectToWorld(IN.positionOS);
        OUT.positionWS = positionWS;
        OUT.positionCS = TransformWorldToHClip(positionWS);
        OUT.uv = IN.uv;

        float3 normalWS = TransformObjectToWorldNormal(IN.normalOS);
        float3 tangentWS = TransformObjectToWorldDir(IN.tangentOS.xyz);
        float3 bitangentWS = cross(normalWS, tangentWS) * IN.tangentOS.w;

        OUT.normalWS = normalWS;
        OUT.tangentWS = tangentWS;
        OUT.bitangentWS = bitangentWS;

        return OUT;
    }

    float4 Frag(Varyings IN) : SV_Target
    {
        float2 uv = IN.uv;

        // Sample textures
        float4 albedoSample = SAMPLE_TEXTURE2D(_ALB, sampler_ALB, TRANSFORM_TEX(uv, _ALB));
        float3 albedo = albedoSample.rgb;
        float alpha = albedoSample.a;
        clip(alpha - _Cutoff); // Alpha clip

        float3 tangentNormal = UnpackNormal(SAMPLE_TEXTURE2D(_NRM, sampler_NRM, TRANSFORM_TEX(uv, _NRM)));
        float3x3 tangentToWorld = float3x3(IN.tangentWS, IN.bitangentWS, IN.normalWS);
        float3 normalWS = normalize(mul(tangentNormal, tangentToWorld));

        float4 rma = SAMPLE_TEXTURE2D(_RMA, sampler_RMA, TRANSFORM_TEX(uv, _RMA));
        float roughness = saturate(rma.r * _RoughnessMultiply);
        float metallic = rma.g;
        float ao = rma.b;

        // Output a simple lit-like result (not HDRP-lit! just visualizing)
        float3 lightDir = normalize(float3(0.3, 0.8, 0.5)); // Arbitrary light direction
        float3 viewDir = normalize(_WorldSpaceCameraPos - IN.positionWS);
        float3 halfDir = normalize(lightDir + viewDir);

        float NdotL = saturate(dot(normalWS, lightDir));
        float NdotH = saturate(dot(normalWS, halfDir));

        float3 diffuse = albedo * NdotL;
        float3 specular = pow(NdotH, 32) * metallic;

        float3 color = (diffuse + specular) * ao;

        return float4(color, alpha);
    }
    ENDHLSL

    SubShader
    {
        Tags{ "RenderPipeline"="HDRenderPipeline" }
        Pass
        {
            Name "ForwardUnlit"
            Tags{ "LightMode" = "ForwardOnly" }

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            ENDHLSL
        }
    }

    FallBack Off
}
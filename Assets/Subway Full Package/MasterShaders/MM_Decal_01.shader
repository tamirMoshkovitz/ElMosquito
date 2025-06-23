Shader "HDRP/MM_Decal_01_HDRP"
{
    Properties
    {
        _AlbedoTexture("Albedo Texture", 2D) = "white" {}
        _Microdetail("Microdetail", 2D) = "white" {}
        _ColorTint("Color Tint", Color) = (1,1,1,1)
        _ScratchesOpacityPower("Scratches Opacity Power", Range(0,1)) = 1
        _TilingU("Tiling U", Float) = 1
        _TilingV("Tiling V", Float) = 1
        _RoughMultiply("Rough Multiply", Range(0,1)) = 0.35
        _Opacity("Opacity Blend", Float) = 0
        _OpacityMulti("Opacity Multiplier", Range(0,2)) = 1
        _UseAlbedoRGBforAlpha("Use Albedo RGB for Alpha?", Float) = 1
        _Cutoff("Alpha Clip Threshold", Range(0,1)) = 0.5
    }

    HLSLINCLUDE

    #pragma target 4.5
    #pragma vertex vert
    #pragma fragment frag

    // Include HDRP and core libraries
    // #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/ShaderPass/ShaderPass.cs.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Builtin/BuiltinData.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"

    TEXTURE2D(_AlbedoTexture); SAMPLER(sampler_AlbedoTexture);
    TEXTURE2D(_Microdetail);   SAMPLER(sampler_Microdetail);

    float4 _AlbedoTexture_ST;
    float _ScratchesOpacityPower;
    float _TilingU, _TilingV;
    float4 _ColorTint;
    float _RoughMultiply;
    float _Opacity;
    float _OpacityMulti;
    float _UseAlbedoRGBforAlpha;
    float _Cutoff;

    struct Attributes
    {
        float3 positionOS : POSITION;
        float2 uv         : TEXCOORD0;
    };

    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float2 uv         : TEXCOORD0;
    };

    Varyings vert(Attributes IN)
    {
        Varyings OUT;

        float3 positionWS = TransformObjectToWorld(IN.positionOS);
        OUT.positionCS = TransformWorldToHClip(positionWS);
        OUT.uv = IN.uv;

        return OUT;
    }

    float4 frag(Varyings IN) : SV_Target
    {
        float2 uv = IN.uv;

        float2 microUV = float2(_TilingU * uv.x * 0.5, _TilingV * uv.y * 0.5);
        float detailG = SAMPLE_TEXTURE2D(_Microdetail, sampler_Microdetail, microUV).g;

        float2 albedoUV = TRANSFORM_TEX(uv, _AlbedoTexture);
        float4 alb = SAMPLE_TEXTURE2D(_AlbedoTexture, sampler_AlbedoTexture, albedoUV);

        float scratchOpacity = 1.0 - pow(detailG, _ScratchesOpacityPower);
        float4 scratchMask = float4(scratchOpacity, scratchOpacity, scratchOpacity, 1.0);

        float4 blend1 = lerp(float4(1,1,1,1), (_UseAlbedoRGBforAlpha > 0.5) ? alb : alb * detailG, _Opacity);
        float alpha = saturate((blend1.r - scratchMask.r) * _OpacityMulti);

        clip(alpha - _Cutoff);

        float3 color = (detailG * alb.rgb) * _ColorTint.rgb;

        return float4(color, alpha);
    }

    ENDHLSL

    SubShader
    {
        Tags { "RenderPipeline"="HDRenderPipeline" }
        Pass
        {
            Name "HDRP_Decal"
            Tags { "LightMode"="ForwardOnly" "Queue"="Transparent" }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Back
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDHLSL
        }
    }
    FallBack Off
}
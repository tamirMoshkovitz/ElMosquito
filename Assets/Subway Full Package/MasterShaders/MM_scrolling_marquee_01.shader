Shader "Custom/MM_scrolling_marquee_01_HDRP"
{
    Properties
    {
        _scrollingtexture("Scrolling Texture", 2D) = "white" {}
        _Tile_mask("Tile Mask", 2D) = "white" {}
        _Mainmask("Main Mask", 2D) = "white" {}
        [HDR]_ColorEmissive("Color Emissive", Color) = (1,1,1,1)
    }

    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"

    TEXTURE2D(_scrollingtexture);
    SAMPLER(sampler_scrollingtexture);
    TEXTURE2D(_Tile_mask);
    SAMPLER(sampler_Tile_mask);
    TEXTURE2D(_Mainmask);
    SAMPLER(sampler_Mainmask);

    float4 _ColorEmissive;
    float4 _scrollingtexture_ST;

    struct Attributes
    {
        float3 positionOS : POSITION;
        float2 uv         : TEXCOORD0;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float2 uv         : TEXCOORD0;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    Varyings Vert(Attributes input)
    {
        Varyings output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

        float3 worldPos = TransformObjectToWorld(input.positionOS);
        output.positionCS = TransformWorldToHClip(worldPos);
        output.uv = input.uv * _scrollingtexture_ST.xy + _scrollingtexture_ST.zw;

        return output;
    }

    float3 SampleScrollingEmission(float2 uv, float time)
    {
        float2 tileUV = uv * float2(5.0, 0.5) + float2(0.0, 0.5);
        float4 mainMask = SAMPLE_TEXTURE2D(_Mainmask, sampler_Mainmask, uv);
        float4 tileMask = SAMPLE_TEXTURE2D(_Tile_mask, sampler_Tile_mask, tileUV);

        // Emission base color (assume linear color space)
        float4 baseColor = float4(0.1950, 0.9368, 0.0768, 1.0);

        float2 panner = time * float2(0.45, 0.0) + (uv * baseColor.rg);
        float4 scrollingTex = SAMPLE_TEXTURE2D(_scrollingtexture, sampler_scrollingtexture, panner);

        float3 emission = mainMask.r * (
            (tileMask.g * _ColorEmissive.rgb) * 0.01
            + (0.01 * _ColorEmissive.rgb)
            + (tileMask.g * scrollingTex.rgb * _ColorEmissive.rgb)
        );

        return emission;
    }

    float4 Fragment(Varyings input) : SV_Target
    {
        UNITY_SETUP_INSTANCE_ID(input);

        // Base albedo color (fixed, non-textured)
        float3 baseColor = float3(0.0198, 0.0106, 0.0045);

        // Smoothness from main mask texture
        float mainMaskBlue = SAMPLE_TEXTURE2D(_Mainmask, sampler_Mainmask, input.uv).b;
        float smoothness = saturate(1.0 - mainMaskBlue * 2.0);

        // Metallic hardcoded 0
        float metallic = 0.0;

        // Alpha fixed opaque
        float alpha = 1.0;

        // Emissive color calculation with scrolling
        float3 emissiveColor = SampleScrollingEmission(input.uv, _Time.y);

        // Simple lighting: just return emission + albedo modulated by smoothness (fake lit)
        // For actual lighting, you'd integrate HDRP lighting functions here.
        float3 finalColor = baseColor + emissiveColor;

        return float4(finalColor, alpha);
    }
    ENDHLSL

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" "IsEmissive"="true" }

        Pass
        {
            Name "Forward"
            Tags { "LightMode" = "Forward" }
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Fragment
            #pragma target 4.5

            ENDHLSL
        }
    }

    FallBack "Hidden/HDRenderPipeline/Unlit"
}
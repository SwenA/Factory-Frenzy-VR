// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/AdditiveVR"
{
    Properties
    {
        _TintColor("Tint Color", Color) = (0.5, 0.5, 0.5, 0.5)
        _MainTex("Particle Texture Left Eye", 2D) = "white" {}
        _MainTexR("Particle Texture Right Eye", 2D) = "white" {}
    }

        CGINCLUDE

#include "UnityCG.cginc"

        sampler2D _MainTex;
    float4 _MainTex_ST;
    sampler2D _MainTexR;
    float4 _MainTexR_ST;
    fixed4 _TintColor;

    struct appdata_t
    {
        float4 position : POSITION;
        float4 texcoord : TEXCOORD0;
        fixed4 color : COLOR;
    };

    struct v2f
    {
        float4 position : SV_POSITION;
        float2 texcoord : TEXCOORD0;
        fixed4 color : COLOR;
        UNITY_FOG_COORDS(1)
    };

    v2f vert(appdata_t v)
    {
        v2f o;
        o.position = UnityObjectToClipPos(v.position);
        o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
        o.color = v.color;
        UNITY_TRANSFER_FOG(o, o.vertex);
        return o;
    }

    fixed4 frag(v2f i) : SV_Target
    {
        fixed4 col;
        if (unity_StereoEyeIndex == 0)
        {
            col = 2.0f * i.color * _TintColor * tex2D(_MainTex, UnityStereoScreenSpaceUVAdjust(i.texcoord, _MainTex_ST));
        }
        else 
        {
            col = 2.0f * i.color * _TintColor * tex2D(_MainTexR, UnityStereoScreenSpaceUVAdjust(i.texcoord, _MainTexR_ST));
        }
        UNITY_APPLY_FOG_COLOR(i.fogCoord, col, (fixed4)0);
        return col;
    }

        ENDCG

        SubShader
    {
        Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

            Blend SrcAlpha One
            Cull Off Lighting Off ZWrite Off Fog{ Mode Off }

            Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_particles
            #pragma multi_compile_fog
            ENDCG
        }
    }
}
Shader "Custom/DoubleSidedTransparent"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}     // La texture d'origine
        _ReplaceColor ("Replace Color", Color) = (1,0,0,1) // Couleur de remplacement (rouge par défaut)
        _Threshold ("Threshold", Range(0,1)) = 0.5 // Seuil de détection des pixels rouges
        _GlobalOpacity ("Global Opacity", Range(0,1)) = 1.0 // Opacité globale de la texture
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue"="Transparent" }
        LOD 200

        Blend SrcAlpha OneMinusSrcAlpha // Activer la transparence
        Cull Off // Désactiver le culling pour rendre les deux côtés

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _ReplaceColor;  // Couleur de remplacement
            float _Threshold;      // Seuil pour détecter la quantité de rouge
            float _GlobalOpacity;  // Opacité globale

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Lire la couleur du pixel de la texture, y compris l'alpha (transparence)
                fixed4 texColor = tex2D(_MainTex, i.uv);

                // Si le pixel est entièrement transparent, on le conserve tel quel
                if (texColor.a == 0)
                {
                    return texColor;
                }

                // Condition pour détecter un pixel rouge (seuil pour tolérance)
                if (texColor.r > texColor.g + _Threshold && texColor.r > texColor.b + _Threshold)
                {
                    // Remplace le pixel rouge par la couleur définie, tout en conservant l'alpha (transparence)
                    return float4(_ReplaceColor.rgb, texColor.a * _GlobalOpacity);
                }
                else
                {
                    // Sinon, conserve la couleur et applique l'opacité globale
                    return float4(texColor.rgb, texColor.a * _GlobalOpacity);
                }
            }
            ENDCG
        }
    }
    FallBack "Transparent/Cutout/VertexLit"
}
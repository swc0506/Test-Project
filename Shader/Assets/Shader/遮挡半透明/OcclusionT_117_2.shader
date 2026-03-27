Shader "Unlit/OcclusionT_117_2"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        // 菲涅尔参数 垂直角度反射率
        _FresnelScale ("Fresnel Scale", Range(0, 2)) = 1.0
        _FresnelPower ("Fresnel Power", Range(0, 5)) = 5.0
        _Color ("Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }

        Pass
        {
            ZTest Greater
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha // 混合后的颜色 = 源颜色 * 源透明度 + 目标颜色 * （1 - 源透明度）
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 viewDir : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
            };
            
            float _FresnelScale;
            float _FresnelPower;
            fixed4 _Color;

            v2f vert(appdata_base v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //视角方向 法线
                o.viewDir = normalize(WorldSpaceViewDir(v.vertex));
                o.worldNormal = normalize(UnityObjectToWorldNormal(v.normal));
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // 菲涅尔公式 
                fixed alpha = pow(1 - dot(normalize(i.viewDir), normalize(i.worldNormal)), _FresnelPower) * (1 - _FresnelScale) + _FresnelScale;
                return fixed4(_Color.rgb, alpha);
            }
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed _Alpha;

            v2f vert(appdata_base v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
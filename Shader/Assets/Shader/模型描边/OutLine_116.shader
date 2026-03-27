Shader "Unlit/OutLine_116"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutLineColor ("OutLine Color", Color) = (1,1,1,1)
        _OutLineWidth ("OutLine Width", Float) = 0.1
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Transparent" }
        

        Pass
        {
            // 取消深度写入 第二个Pass 才有深度写入
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            float4 _OutLineColor;
            float _OutLineWidth;

            v2f vert(appdata_base v)
            {
                v2f o;
                //偏移顶点位置
                v.vertex.xyz += normalize(v.normal) * _OutLineWidth;

                // 顶点坐标转换到裁剪空间
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return _OutLineColor;
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
            
            v2f vert(appdata_base v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv);
            }
            
            ENDCG
        }
    }

Fallback "Diffuse"
}
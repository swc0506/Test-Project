Shader "Unlit/MovingLight_115"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        //光叠加颜色
        _Color ("Color", Color) = (1,1,1,1)
        //光的速度
        _Speed ("Speed", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent"  "Queue"="Transparent"}
        Blend One One //透明混合模式
        Cull Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Speed;

            v2f vert (appdata_base v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //根据时间偏移uv坐标
                i.uv = float2(i.uv.x + _Time.y * _Speed, i.uv.y);
                return tex2D(_MainTex, i.uv) * _Color;
            }
            ENDCG
        }
    }
}

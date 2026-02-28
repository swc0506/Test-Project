Shader "Unlit/Shader_PT"
{
    Properties
    {
        // 棋盘格参数
        _TileCount("TileCount", Float) = 8
        // 棋盘格颜色
        _Color1("Color1", Color) = (1,1,1,1)
        _Color2("Color2", Color) = (0,0,0,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float _TileCount;
            float4 _Color1;
            float4 _Color2;
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata_base v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv * _TileCount;// 将uv坐标乘以_TileCount，得到一个新的uv坐标
                float2 pos = floor(uv);// 对新的uv坐标取整，得到一个新的坐标
                float value = (pos.x + pos.y) % 2;// 对新的坐标取余，得到一个0到1之间的小数
                
                return lerp(_Color1, _Color2, value);
            }
            ENDCG
        }
    }
}

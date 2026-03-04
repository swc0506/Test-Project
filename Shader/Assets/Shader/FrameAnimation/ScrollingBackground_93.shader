Shader "Unlit/ScrollingBackground_93"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _scrollSpeedU ("Scroll SpeedU", float) = 0.5
        _scrollSpeedV ("Scroll SpeedV", float) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="transparent" "Queue"="Transparent" "IgnoreProjector"="True" }
        
        Pass
        {
            Zwrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            
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
            float _scrollSpeedU;
            float _scrollSpeedV;

            v2f vert (appdata_base v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //利用_Time.y来控制滚动速度
                //frac函数可以将一个浮点数的小数部分提取出来
                //将滚动后的UV坐标映射到[0,1]的范围内
                float2 scrollUV = frac(i.uv + float2(_scrollSpeedU * _Time.y, _scrollSpeedV * _Time.y));
                return tex2D(_MainTex, scrollUV);
            }
            ENDCG
        }
    }
}

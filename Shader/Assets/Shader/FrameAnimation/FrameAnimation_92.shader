Shader "Unlit/FrameAnimation_92"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        //图集行列
        _Row ("Row", Int) = 8
        _Col ("Col", Int) = 8
        //切换时间
        _Speed ("Speed", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="transparent" "Queue"="Transparent" "IgnoreProjector"="True"}
        
        Pass
        {
            ZWrite Off
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
            int _Row;
            int _Col;
            float _Speed;

            v2f vert (appdata_base v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //得到当前帧 利用时间变量计算
                float frameIndex = floor(_Time.y * _Speed) % (_Row * _Col);
                //计算起始位置
                float2 frameUV = float2(frameIndex % _Col / _Col, 1 - (floor(frameIndex / _Row) + 1) / _Row);
                //得到uv缩放比例 从大图映射到小图
                float2 uvScale = float2(1.0 / _Col, 1.0 / _Row);
                
                //计算过最终uv采样坐标信息
                float2 uv = i.uv * uvScale + frameUV;
                return tex2D(_MainTex, uv);
            }
            ENDCG
        }
    }
}

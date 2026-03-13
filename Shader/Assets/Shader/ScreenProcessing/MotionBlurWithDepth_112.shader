Shader "Unlit/MotionBlurWithDepth_112"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurSize("Blur Size", Float) = 0.5
    }
    SubShader
    {
        Pass
        {
            ZTest Always
            Cull Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 uv_depth : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _Maintex_TexelSize;
            fixed _BlurSize;
            sampler2D _CameraDepthTexture;
            float4x4 _ClipToWorldMatrix; //裁剪空间到世界空间的变换矩阵
            float4x4 _FrontWorldToClipMatrix; //上一帧 世界空间到裁剪空间的变换矩阵

            v2f vert(appdata_base v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.uv_depth = v.texcoord;
                //多平台时建议进行翻转判断
                #if UNITY_UV_STARTS_AT_TOP
                    if (_Maintex_TexelSize.y < 0)
                        o.uv_depth.y = 1 - o.uv_depth.y;
                #endif

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                //得到裁剪空间下的两个点
                //获得深度值
                float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv_depth);
                //得到裁剪空间下一个组合
                float4 nowClipPos = float4(i.uv.x * 2 - 1, i.uv.y * 2 - 1, depth * 2 - 1, 1);
                //用裁剪空间到世界空间的变换矩阵 得到 世界空间下的
                float4 worldPos = mul(_ClipToWorldMatrix, nowClipPos);
                //齐次除法
                worldPos /= worldPos.w;

                //第二个点 得到上一帧 对应的裁剪空间下的点
                float4 oldClipPos = mul(_FrontWorldToClipMatrix, worldPos);
                oldClipPos /= oldClipPos.w;

                //得到运动方向
                float2 moveDir = (nowClipPos.xy - oldClipPos.xy) / 2;

                //进行模糊处理
                float2 uv = i.uv;
                float4 color = float4(0, 0, 0, 0);
                for (int j = 0; j < 3; j++)
                {
                    //第一次采样累加的是当前像素所在位置
                    color += tex2D(_MainTex, uv);
                    uv += moveDir * _BlurSize;
                }
                color /= 3;
                return fixed4(color.rgb, 1);
            }
            ENDCG
        }
    }
    Fallback Off
}
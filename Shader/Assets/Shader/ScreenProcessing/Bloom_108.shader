Shader "Unlit/Bloom_108"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Bloom("Bloom", 2D) = ""{}// 高光
        _Luminance("Luminance", Float) = 0.5// 亮度阈值

        _BlurSize("BlurSize", Float) = 1.0// 模糊大小
    }
    SubShader
    {
        CGINCLUDE
        #include "UnityCG.cginc"

        struct v2f
        {
            float2 uv : TEXCOORD0;
            float4 vertex : SV_POSITION;
        };

        sampler2D _MainTex;
        sampler2D _Bloom;
        float _Luminance;
        float _BlurSize;
        half4 _MainTex_TexelSize;

        fixed luminance(fixed4 color)
        {
            return dot(color.rgb, fixed3(0.2125, 0.7154, 0.0721));
        }
        ENDCG

        Tags
        {
            "RenderType"="Opaque"
        }

        ZTest Always
        Cull Off
        ZWrite Off

        //提取Pass
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            v2f vert(appdata_base v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord.xy;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                //采样源纹理颜色
                fixed4 color = tex2D(_MainTex, i.uv);
                //得到亮度贡献值
                fixed luminanceValue = clamp(luminance(color) - _Luminance, 0, 1);
                //返回颜色*亮度贡献值
                return color * luminanceValue;
            }
            ENDCG
        }

        //复用高斯模糊的2个Pass
        UsePass "Unlit/GaussianBlur_106/GAUSSIAN_BLUR_HORIZONTAL"
        UsePass "Unlit/GaussianBlur_106/GAUSSIAN_BLUR_VERTICAL"

        //合并Pass
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct v2fBloom
            {
                //xy 用于主纹理进行采样 zw 用于高亮模糊后的纹理进行采样
                half4 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2fBloom vert(appdata_base v)
            {
                v2fBloom o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv.xy = v.texcoord.xy; // 主纹理
                o.uv.zw = v.texcoord.xy; // 高光模糊纹理

                //用宏去判断uv坐标是否被翻转 使用RenderTexture写入到Shader的纹理变量时Unity可能会对其进行Y轴翻转
                #if UNITY_UV_STARTS_AT_TOP
                if (_MainTex_TexelSize.y < 0) // 如果y轴被翻转 则需要对高光模糊纹理的y坐标进行翻转
                {
                    o.uv.w = 1 - o.uv.w;
                }

                #endif
                return o;
            }

            fixed4 frag(v2fBloom i) : SV_Target
            {
                return tex2D(_MainTex, i.uv.xy) + tex2D(_Bloom, i.uv.zw); // 主纹理 + 高光模糊纹理
            }
            ENDCG
        }
    }

    Fallback Off
}
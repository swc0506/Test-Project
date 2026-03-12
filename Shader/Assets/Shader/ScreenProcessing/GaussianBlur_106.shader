Shader "Unlit/GaussianBlur_106"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurSpread ("Blur Spread", float) = 1.0
    }
    SubShader
    {
        //用于包裹共用代码 在之后的多个Pass当中都可以使用
        CGINCLUDE
        #include "UnityCG.cginc"

        sampler2D _MainTex;
        half4 _MainTex_TexelSize;
        float _BlurSpread; //纹理偏移单位

        struct v2f
        {
            //5个像素的uv坐标偏移
            half2 uv[5] : TEXCOORD0;
            float4 vertex : SV_POSITION;
        };

        //水平方向的 顶点着色器函数
        v2f vertBlurHorizontal(appdata_base v)
        {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);

            o.uv[0] = v.texcoord;
            o.uv[1] = v.texcoord + half2(_MainTex_TexelSize.x, 0.0) * _BlurSpread;
            o.uv[2] = v.texcoord - half2(_MainTex_TexelSize.x, 0.0) * _BlurSpread;
            o.uv[3] = v.texcoord + half2(2.0 * _MainTex_TexelSize.x, 0.0) * _BlurSpread;
            o.uv[4] = v.texcoord - half2(2.0 * _MainTex_TexelSize.x, 0.0) * _BlurSpread;
            return o;
        }

        //垂直方向的 顶点着色器函数
        v2f vertBlurVertical(appdata_base v)
        {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);

            o.uv[0] = v.texcoord;
            o.uv[1] = v.texcoord + half2(0.0, _MainTex_TexelSize.y) * _BlurSpread;
            o.uv[2] = v.texcoord - half2(0.0, _MainTex_TexelSize.y) * _BlurSpread;
            o.uv[3] = v.texcoord + half2(0.0, 2.0 * _MainTex_TexelSize.y) * _BlurSpread;
            o.uv[4] = v.texcoord - half2(0.0, 2.0 * _MainTex_TexelSize.y) * _BlurSpread;
            return o;
        }

        //片元着色器函数 通用
        fixed4 fragBlur(v2f i) : SV_Target
        {
            //卷积运算
            //卷积核
            float weight[3] = {0.4026, 0.2442, 0.0545};
            //先计算当前像素点
            fixed3 sum = tex2D(_MainTex, i.uv[0]).rgb * weight[0];

            sum += tex2D(_MainTex, i.uv[1]).rgb * weight[1];
            sum += tex2D(_MainTex, i.uv[2]).rgb * weight[1];
            sum += tex2D(_MainTex, i.uv[3]).rgb * weight[2];
            sum += tex2D(_MainTex, i.uv[4]).rgb * weight[2];

            return fixed4(sum, 1.0);
        }
        ENDCG

        Tags
        {
            "RenderType"="Opaque"
        }

        ZTest Always
        Cull Off
        ZWrite Off

        Pass
        {
            Name "GAUSSIAN_BLUR_HORIZONTAL"
            CGPROGRAM
            #pragma vertex vertBlurHorizontal
            #pragma fragment fragBlur
            ENDCG
        }

        Pass
        {
            Name "GAUSSIAN_BLUR_VERTICAL"
            CGPROGRAM
            #pragma vertex vertBlurVertical
            #pragma fragment fragBlur
            ENDCG
        }
    }
}
Shader "Unlit/MotionBlur_109"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurAmount ("Blur Amount", Float) = 0.5//模糊程度
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
        float _BlurAmount;

        v2f vert(appdata_base v)
        {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.uv = v.texcoord;
            return o;
        }

        fixed4 frag(v2f i) : SV_Target
        {
            fixed4 col = tex2D(_MainTex, i.uv);
            return col;
        }
        ENDCG

        ZTest Always
        ZWrite Off
        Cull Off
        
        Pass//RGB通道分离
        {
            Blend SrcAlpha OneMinusSrcAlpha//(源颜色 * _BlurAmount) + (目标颜色 * (1 - _BlurAmount))
            ColorMask RGB
            
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment fragRGB
            
            fixed4 fragRGB(v2f i) : SV_Target
            {
                return fixed4(tex2D(_MainTex, i.uv).rgb, _BlurAmount);
            }
            
            ENDCG
        }

        Pass//A通道分离
        {
            Blend One Zero//(源颜色 * 1) + (目标颜色 * 0)
            ColorMask A//只改变颜色缓冲区中的A通道
            
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment fragA
            
            fixed4 fragA(v2f i) : SV_Target
            {
                return fixed4(tex2D(_MainTex, i.uv));
            }
            
            ENDCG
        }
    }
}
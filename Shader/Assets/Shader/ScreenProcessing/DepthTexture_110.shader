Shader "Unlit/DepthTexture_110"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION; // 裁剪空间下的顶点坐标
            };

            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;

            v2f vert (appdata_base v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target// SV_Target 是固定写法，表示输出到渲染目标
            {
                //非线性的 裁剪空间下的深度值
                float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
                fixed linearDepth = Linear01Depth(depth);//线性深度值
                return fixed4(linearDepth, linearDepth, linearDepth, 1);
            }
            ENDCG
        }
    }
}

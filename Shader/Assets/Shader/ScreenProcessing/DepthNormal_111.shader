Shader "Unlit/DepthNormal_111"
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
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _CameraDepthNormalsTexture;
            
            v2f vert (appdata_img v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 获取深度和法线 裁剪空间下
                float4 depthNormal = tex2D(_CameraDepthNormalsTexture, i.uv);
                fixed depth;
                fixed3 normals;
                DecodeDepthNormal(depthNormal, depth, normals);
                // 法线的-1到1之间的区间 变换到 0到1之间
                normals = normals * 0.5 + 0.5;
                
                return fixed4(normals, 1.0);
            }
            ENDCG
        }
    }
}

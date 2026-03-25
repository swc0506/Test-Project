Shader "Unlit/EdgeWithDepthNormalsTexture_114"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _EdgeOnly ("Edge Only", Float) = 0.0 //0显示场景 1只显示边缘
        _EdgeColor ("Edge Color", Color) = (0, 0, 0, 1) //边缘颜色
        _EdgeGroundColor ("Edge Ground Color", Color) = (1, 1, 1, 1) //边缘背景颜色
        _SampleDistance ("Sample Distance", Float) = 1.0 //采样偏移程度
        _SensitivityDepth ("Sensitivity Depth", Float) = 1.0 //深度灵敏度
        _SensitivityNormal ("Sensitivity Normal", Float) = 1.0 //法线灵敏度
    }
    SubShader
    {
        //屏幕后处理
        ZTest Always
        Cull Off
        ZWrite Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct v2f
            {
                // 5个uv，分别对应中心点，4个边缘点 中心 左上 右下 右上 左下
                half2 uv[5] : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            half4 _MainTex_TexelSize;
            sampler2D _CameraDepthNormalsTexture;
            fixed4 _EdgeColor;
            fixed4 _EdgeGroundColor;
            float _EdgeOnly;
            float _SampleDistance;
            float _SensitivityDepth;
            float _SensitivityNormal;

            //用于比较两个点的深度和法线纹理中采样得到的信息 用来判断是否边缘
            //返回1 表示同一平面 0 表示不同平面
            half CheckSame(half4 depthNormal1, half4 depthNormal2)
            {
                // 得到深度值
                float depth1 = DecodeFloatRG(depthNormal1.zw);
                float depth2 = DecodeFloatRG(depthNormal2.zw);

                // 得到法线值
                float2 normal1 = depthNormal1.xy;
                float2 normal2 = depthNormal2.xy;
                
                //法线差异计算
                float2 normalDiff = abs(normal1 - normal2) * _SensitivityNormal;
                int isSameNormal = (normalDiff.x + normalDiff.y) < 0.1;// 法线差异小于0.1 认为是同一个法线
                //深度差异计算
                float depthDiff = abs(depth1 - depth2) * _SensitivityDepth;
                int isSameDepth = depthDiff < 0.1 * depth1;// 深度差异小于阈值 认为是同一个深度
                
                return isSameDepth * isSameNormal ? 1 : 0;
            }
            
            v2f vert(appdata_base v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                half2 uv = v.texcoord;
                o.uv[0] = uv;// 中心点
                o.uv[1] = uv + _MainTex_TexelSize.xy * half2(-1, 1) * _SampleDistance;// 左上点
                o.uv[2] = uv + _MainTex_TexelSize.xy * half2(1, -1) * _SampleDistance;// 右下点
                o.uv[3] = uv + _MainTex_TexelSize.xy * half2(1, 1) * _SampleDistance;// 右上点
                o.uv[4] = uv + _MainTex_TexelSize.xy * half2(-1, -1) * _SampleDistance;// 左下点
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                //获取四个点的深度和法线信息
                half4 TL = tex2D(_CameraDepthNormalsTexture, i.uv[1]);
                half4 BR = tex2D(_CameraDepthNormalsTexture, i.uv[2]);
                half4 TR = tex2D(_CameraDepthNormalsTexture, i.uv[3]);
                half4 BL = tex2D(_CameraDepthNormalsTexture, i.uv[4]);
                
                half edgeLerp = 1;
                edgeLerp *= CheckSame(TL, BR);
                edgeLerp *= CheckSame(TR, BL);
                
                //通过插值进行颜色变化
                fixed4 withEdgeColor = lerp(_EdgeColor, tex2D(_MainTex, i.uv[0]), edgeLerp);
                fixed4 onlyEdgeColor = lerp(_EdgeColor, _EdgeGroundColor, edgeLerp);
                
                return lerp(withEdgeColor, onlyEdgeColor, _EdgeOnly);
            }
            ENDCG
        }
    }
}
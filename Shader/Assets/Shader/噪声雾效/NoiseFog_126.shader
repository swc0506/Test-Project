Shader "Unlit/NoiseFog_126"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FogColor ("Fog Color", Color) = (1, 1, 1, 1)
        _FogDensity ("Fog Density", Float) = 1
        _FogStart ("Fog Start", Float) = 0
        _FogEnd ("Fog End", Float) = 10

        _Noise("Noise", 2D) = ""{}
        // 噪声系数控制偏移范围
        _NoiseAmount("Noise Amount", Float) = 0.1
        _FogXSpeed("Fog X Speed", Float) = 0.1
        _FogYSpeed("Fog Y Speed", Float) = 0.1
    }
    SubShader
    {
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
                float2 uv : TEXCOORD0;
                float2 uv_depth : TEXCOORD1;
                float4 ray:TEXCOORD2; //指向四个角的方向向量 传递到片元时会自动进行插值
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            half4 _MainTex_TexelSize; //纹素 用来判断翻转
            sampler2D _CameraDepthTexture;
            float4 _FogColor;
            float _FogDensity;
            float _FogStart;
            float _FogEnd;
            float4x4 _RayMatrix; //0左下 1右下 2右上 3左上
            
            sampler2D _Noise;
            float _NoiseAmount;
            float _FogXSpeed;
            float _FogYSpeed;

            v2f vert(appdata_base v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.uv_depth = v.texcoord;
                //顶点着色器函数 每一个顶点都会执行一次
                //对于屏幕后处理来说 会执行四次
                //通过uv坐标判断当前顶点位置
                int index = 0;
                if (v.texcoord.x < 0.5 && v.texcoord.y < 0.5)
                {
                    index = 0;
                }
                else if (v.texcoord.x > 0.5 && v.texcoord.y < 0.5)
                {
                    index = 1;
                }
                else if (v.texcoord.x > 0.5 && v.texcoord.y > 0.5)
                {
                    index = 2;
                }
                else
                {
                    index = 3;
                }

                //判断纹理是否翻转
                #if UNITY_UV_STARTS_AT_TOP
                if (_MainTex_TexelSize.y < 0)
                {
                    o.uv_depth.y = 1 - o.uv_depth.y;
                    index = 3 - index;
                }
                #endif

                o.ray = _RayMatrix[index];
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                //观察深度值 z分量
                float linerDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv_depth));
                //世界空间下 像素的坐标
                float3 worldPos = _WorldSpaceCameraPos + linerDepth * i.ray;
                
                //噪声纹理采样以及偏移
                float2 speed = _Time.y * float2(_FogXSpeed, _FogYSpeed);
                //噪声纹理中采样 采样出来的结果是0-1 我们需要将其转为 -0.5-0.5 然后还可以通过自定义系数控制这个正负范围
                float noise = (tex2D(_Noise, i.uv + speed).r - 0.5) * _NoiseAmount;

                //雾相关计算
                //混合因子
                float f = (_FogEnd - worldPos.y) / (_FogEnd - _FogStart);
                f = saturate(f * _FogDensity * (1 + noise)); //saturate 饱和函数 将值限制在0-1之间

                fixed3 color = lerp(tex2D(_MainTex, i.uv).rgb, _FogColor.rgb, f);

                return fixed4(color, 1);
            }
            ENDCG
        }
    }
}
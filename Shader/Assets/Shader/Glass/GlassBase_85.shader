Shader "Unlit/GlassBase_85"
{
    Properties
    {
        //主纹理
        _MainTex("MainTex", 2D) = ""{}
        //立方体纹理
        _Cube("Cube", Cube) = ""{}
        // 折射程度 0 表示完全反射 1 表示完全折射
        _RefractAmount("RefractAmount", Range(0, 1)) = 1
    }
    SubShader
    {
        //滞后渲染
        Tags
        {
            "RenderType"="Opaque" "Queue"="transparent"
        }

        // 抓取屏幕图像
        GrabPass {}

        Pass
        {
            Tags
            {
                "LightMode"="ForwardBase"
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            samplerCUBE _Cube;
            float _RefractAmount;
            //默认抓取纹理
            sampler2D _GrabTexture;

            struct v2f
            {
                float4 pos : SV_POSITION; //裁剪空间下的顶点坐标
                float4 screenPos : TEXCOORD0; //屏幕空间下的顶点坐标（顶点相对于屏幕的位置）
                float2 uv : TEXCOORD1; //在颜色纹理中采样的UV坐标
                float3 worldReflect : TEXCOORD2; //世界空间下的反射向量
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex); //顶点坐标转换到裁剪空间
                o.screenPos = ComputeScreenPos(o.pos); //计算屏幕空间下的顶点坐标
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex); //计算在颜色纹理中采样的UV坐标
                float3 worldNormal = UnityObjectToWorldNormal(v.normal); //法线转换到世界空间
                fixed3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz; //顶点坐标转换到世界空间
                fixed3 worldViewDir = UnityWorldSpaceViewDir(worldPos);
                //视线方向转换到世界空间 内部是用_WorldSpaceCameraPos - worldPos
                o.worldReflect = reflect(-worldViewDir, worldNormal); //计算反射向量
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 reflColor = texCUBE(_Cube, i.worldReflect) * tex2D(_MainTex, i.uv); //反射光线采样颜色纹理

                //折射光线采样抓取纹理
                //进行采样偏移 避免采样到当前像素
                fixed2 offset = 1 - _RefractAmount;
                fixed2 screenUV = (i.screenPos.xy - offset / 10) / i.screenPos.w; //将屏幕空间下的顶点坐标转换到[0,1]范围
                fixed4 grabColor = tex2D(_GrabTexture, screenUV); //从抓取纹理中采样颜色

                float4 c = reflColor * (1 - _RefractAmount) + grabColor * _RefractAmount;

                return c;
            }
            ENDCG
        }
    }
}
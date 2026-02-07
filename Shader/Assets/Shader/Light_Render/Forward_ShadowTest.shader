Shader "Unlit/Forward_ShadowTest"
{
    Properties
    {
       //主要就是将单张纹理Shader和布林方光照模型逐片元Shader进行一个结合
       _MainTex("MainTex", 2D) = ""{}
       //漫反射颜色、高光反射颜色、光泽度
       _Color("MainColor", Color) = (1,1,1,1)
       _SpecularColor("SpecularColor", Color) = (1,1,1,1)
       _SpecularNum("SpecularNum", Range(0,20)) = 15
       //透明度测试用的阈值
       _Cutoff("Cutoff", Range(0,1)) = 0
    }
    SubShader
    {
        //设置渲染队列 决定对象在何时渲染
        Tags{"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
        Pass
        {
            Tags{ "LightMode" = "ForwardBase" }
            //关闭剔除 让正面和背面都去进行渲染
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            //纹理贴图对应的映射成员
            sampler2D _MainTex;
            float4 _MainTex_ST;
            //漫反射颜色、高光反射颜色、光泽度
            fixed4 _Color;
            fixed4 _SpecularColor;
            float _SpecularNum;
            //透明度测试用的阈值
            fixed _Cutoff;

            struct v2f
            {
                //裁剪空间下的顶点坐标
                float4 pos:SV_POSITION;
                //UV坐标
                float2 uv:TEXCOORD0;
                //世界空间下的法线
                float3 wNormal:NORMAL;
                //世界空间下的顶点坐标
                float3 wPos:TEXCOORD1;
                //阴影坐标宏 下一次使用的TEXCOORD索引
                SHADOW_COORDS(2)
            };

            v2f vert (appdata_base v)
            {
               v2f data;
               //把模型空间下的顶点转换到裁剪空间下
               data.pos = UnityObjectToClipPos(v.vertex);
               //uv坐标计算
               data.uv = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
               //世界空间下的法线
               data.wNormal = UnityObjectToWorldNormal(v.normal);
               //世界空间下的顶点坐标
               data.wPos = mul(unity_ObjectToWorld, v.vertex);

               //坐标转换宏
               TRANSFER_SHADOW(data)

               return data;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //颜色纹理的颜色信息
                fixed4 texColor = tex2D(_MainTex, i.uv);

                //判断贴图的 颜色信息中的 透明通道 有没有小于阈值 
                //如果小于了 就直接丢弃
                clip(texColor.a - _Cutoff);
                //相当于
                //if(texColor.a - _Cutoff < 0)
                //if(texColor.a < _Cutoff)
                //    discard;

                //新知识点：纹理颜色需要和漫反射材质颜色叠加（乘法） 共同决定最终的颜色
                fixed3 albedo = texColor.rgb * _Color.rgb;
                //光的方向（指向光源方向）
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float3 wNormal = normalize(i.wNormal);
                //兰伯特漫反射颜色 = 光的颜色 * 漫反射材质的颜色 * max(0, dot(世界坐标系下的法线, 光的方向))
                //新知识点：兰伯特光照模型计算时，漫反射材质颜色使用 1 中的叠加颜色计算
                fixed3 lambertColor = _LightColor0.rgb * albedo.rgb * max(0, dot(wNormal, lightDir));
                
                // 视角方向
                //float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.wPos);
                float3 viewDir = normalize(UnityWorldSpaceViewDir(i.wPos));
                //半角向量 = 视角方向 + 光的方向
                float3 halfA = normalize(viewDir + lightDir);
                //高光反射的颜色 = 光的颜色 * 高光反射材质的颜色 * pow(max(0, dot(世界坐标系下的法线, 半角向量)), 光泽度)
                fixed3 specularColor = _LightColor0.rgb * _SpecularColor * pow( max(0, dot(wNormal, halfA)), _SpecularNum);

                //计算光照衰减和阴影衰减的宏
                UNITY_LIGHT_ATTENUATION(atten, i, i.wPos)

                //布林方光照颜色 = 环境光颜色 + 兰伯特漫反射颜色 + 高光反射的颜色
                //新知识点：最终使用的环境光叠加时，环境光变量UNITY_LIGHTMODEL_AMBIENT需要和 1 中颜色进行乘法叠加
                //         为了避免最终的渲染效果偏灰
                fixed3 color = UNITY_LIGHTMODEL_AMBIENT.rgb * albedo + (lambertColor + specularColor)*atten;

                return fixed4(color.rgb, 1);
            }
            ENDCG
        }
    }
    Fallback "Transparent/Cutout/VertexLit"//必须要有_Cutoff 和 _Color属性
}

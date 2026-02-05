Shader "Unlit/Forward_Attend"
{
    Properties
    {
        _MainColor("MainColor", Color) = (1,1,1,1)
        //高光反射颜色  光泽度
        _SpecularColor("SpecularColor", Color) = (1,1,1,1)
        _SpecularNum("SpecularNum", Range(0, 20)) = 1
    }
    SubShader
    {
        //Bass Pass 基础渲染通道
        Pass
        {
            Tags
            {
                "LightMode"="ForwardBase"
            }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            //用于帮助我们编译所有变体 并且保证衰减相关光照变量能够正确赋值到对应的内置变量中
            #pragma multi_compile_fwdbase

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            //材质漫反射颜色
            fixed4 _MainColor;
            fixed4 _SpecularColor;
            float _SpecularNum;

            //顶点着色器返回出去的内容
            struct v2f
            {
                //裁剪空间下的顶点位置
                float4 pos:SV_POSITION;
                //世界空间下的法线位置
                float3 wNormal:NORMAL;
                //世界空间下的 顶点坐标 
                float3 wPos:TEXCOORD0;
                //阴影坐标宏 主要用于存储阴影纹理坐标
                SHADOW_COORDS(2)
            };

            //得到兰伯特光照模型计算的颜色 （逐片元）
            fixed3 getLambertFColor(in float3 wNormal)
            {
                //得到光源单位向量
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                //计算除了兰伯特光照的漫反射颜色
                fixed3 color = _LightColor0.rgb * _MainColor.rgb * max(0, dot(wNormal, lightDir));

                return color;
            }

            //得到Blinn Phong式高光反射模型计算的颜色（逐片元）
            fixed3 getSpecularColor(in float3 wPos, in float3 wNormal)
            {
                //1.视角单位向量
                //float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - wPos );
                float3 viewDir = normalize(UnityWorldSpaceViewDir(wPos));

                //2.光的反射单位向量
                //光的方向
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);

                //半角方向向量
                float3 halfA = normalize(viewDir + lightDir);

                //color = 光源颜色 * 材质高光反射颜色 * pow( max(0, dot(视角单位向量, 光的反射单位向量)), 光泽度 )
                fixed3 color = _LightColor0.rgb * _SpecularColor.rgb * pow(max(0, dot(wNormal, halfA)), _SpecularNum);

                return color;
            }

            v2f vert(appdata_base v)
            {
                v2f v2fData;
                //转换模型空间下的顶点到裁剪空间中
                v2fData.pos = UnityObjectToClipPos(v.vertex);
                //转换模型空间下的法线到世界空间下
                v2fData.wNormal = UnityObjectToWorldNormal(v.normal);
                //顶点转到世界空间
                v2fData.wPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                
                //计算阴影映射纹理坐标 他会在内部去进行计算 将其存入v2f中的SHADOW_COORDS中
                TRANSFER_SHADOW(v2fData);

                return v2fData;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                //计算兰伯特光照颜色
                fixed3 lambertColor = getLambertFColor(normalize(i.wNormal));
                //计算BlinnPhong式高光反射颜色
                fixed3 specularColor = getSpecularColor(i.wPos, normalize(i.wNormal));

                //得到阴影衰减值
                //fixed3 shaow = SHADOW_ATTENUATION(i);
                
                //衰减值
                //fixed atten = 1;
                
                //利用灯光衰减和阴影衰减计算宏进行统一衰减值的计算
                UNITY_LIGHT_ATTENUATION(atten, i, i.wPos)
                
                //物体表面光照颜色 = 环境光颜色 + 兰伯特光照模型所得颜色 + Phong式高光反射光照模型所得颜色
                //衰减值 会和 漫反射颜色 + 高光反射颜色 后 再进行乘法运算
                fixed3 blinnPhongColor = UNITY_LIGHTMODEL_AMBIENT.rgb + (lambertColor + specularColor) * atten;

                return fixed4(blinnPhongColor.rgb, 1);
            }
            ENDCG
        }

        //Additional Pass 附加渲染通道
        Pass
        {
            Tags
            {
                "LightMode"="ForwardAdd"
            }
            //线性减淡的效果 进行 光照颜色混合
            Blend One One

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            //用于帮助我们编译所有变体 并且保证衰减相关光照变量能够正确赋值到对应的内置变量中
            //#pragma multi_compile_fwdadd
            //生成多个包括支持和不支持阴影的Shader变体 从而为额外的逐像素光源计算阴影，并传递给Shader
            #pragma multi_compile_fwdadd_fullshadows

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"
            //材质漫反射颜色
            fixed4 _MainColor;
            fixed4 _SpecularColor;
            float _SpecularNum;

            //顶点着色器返回出去的内容
            struct v2f
            {
                //裁剪空间下的顶点位置
                float4 pos:SV_POSITION;
                //世界空间下的法线位置
                float3 wNormal:NORMAL;
                //世界空间下的 顶点坐标 
                float3 wPos:TEXCOORD0;
                
                //阴影坐标宏 主要用于存储阴影纹理坐标
                SHADOW_COORDS(2)
            };

            v2f vert(appdata_base v)
            {
                v2f v2fData;
                //转换模型空间下的顶点到裁剪空间中
                v2fData.pos = UnityObjectToClipPos(v.vertex);
                //转换模型空间下的法线到世界空间下
                v2fData.wNormal = UnityObjectToWorldNormal(v.normal);
                //顶点转到世界空间
                v2fData.wPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                
                //计算阴影映射纹理坐标 他会在内部去进行计算 将其存入v2f中的SHADOW_COORDS中
                TRANSFER_SHADOW(v2fData);

                return v2fData;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                //兰伯特漫反射
                fixed3 worldNormal = normalize(i.wNormal);
                //平行光 光的方向 其实就是它的位置
                #if defined(_DIRECTIONAL_LIGHT)
                fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
                #else //点光源和聚光灯 光的方向 是 光的位置 - 顶点位置
                fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz - i.wPos);
                #endif
                // 漫反射颜色 = 光颜色 * 属性中颜色 * max(0, dot(世界坐标系下的法线, 世界坐标系下的光方向));
                fixed3 diffuse = _LightColor0.rgb * _MainColor.rgb * max(0, dot(worldNormal, worldLightDir));

                //BlinnPhong高光反射
                //视角方向
                fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.wPos.xyz);
                //半角方向向量
                fixed3 halfDir = normalize(worldLightDir + viewDir);
                // 高光颜色 = 光颜色 * 属性中的高光颜色 * pow(max(0, dot(世界坐标系法线, 世界坐标系半角向量)), 光泽度);
                fixed3 specular = _LightColor0.rgb * _SpecularColor.rgb * pow(
                    max(0, dot(worldNormal, halfDir)), _SpecularNum);

                //衰减值
                // #ifdef USING_DIRECTIONAL_LIGHT
                // fixed atten = 1;
                // #else
                // #if defined(POINT)
                // //将世界坐标系下顶点转到光源空间下
                // float3 lightCoord = mul(unity_WorldToLight, float4(i.wPos, 1)).xyz;
                // //利用这个坐标得到距离的平方 然后再再光源纹理中隐射得到衰减值
                // fixed atten = tex2D(_LightTexture0, dot(lightCoord, lightCoord).xx).UNITY_ATTEN_CHANNEL;
                // #elif defined(SPOT)
                // //将世界坐标系下顶点转到光源空间下 聚光灯需要用w参与后续计算
                // float4 lightCoord = mul(unity_WorldToLight, float4(i.wPos, 1));
                // fixed atten = (lightCoord.z > 0) * //判断在聚光灯前面吗
                //     tex2D(_LightTexture0, lightCoord.xy / lightCoord.w + 0.5).w * //映射到大图中进行采样
                //     tex2D(_LightTextureB0, dot(lightCoord, lightCoord).xx).UNITY_ATTEN_CHANNEL; //距离的平方采样
                // #else
                // fixed atten = 1;
                // #endif
                // #endif
                //在附加渲染通道中不需要在加上环境光颜色了 因为它只需要计算一次 在基础渲染通道中已经计算了
                
                //利用灯光衰减和阴影衰减计算宏进行统一衰减值的计算
                UNITY_LIGHT_ATTENUATION(atten, i, i.wPos)
                return fixed4((diffuse + specular) * atten, 1);
            }
            ENDCG
        }
    }
    FallBack "Specular"
}

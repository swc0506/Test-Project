Shader "Unlit/Dissolve_124"
{
    Properties
    {
        _MainColor("MainColor", Color) = (1,1,1,1)
        _MainTex("MainTex", 2D) = ""{}
        _BumpMap("BumpMap", 2D) = ""{}
        _BumpScale("BumpScale", Range(0,1)) = 1
        _SpecularColor("SpecularColor", Color) = (1,1,1,1)
        _SpecularNum("SpecularNum", Range(0,20)) = 18

        //噪声纹理
        _Noise("Noise", 2D) = ""{}
        //渐变纹理
        _Gradient("Gradient", 2D) = ""{}
        //消融进度
        _Dissolve("Dissolve", Range(0,1)) = 0
        //边缘范围
        _EdgeRange("EdgeRange", Range(0,1)) = 0
    }
    SubShader
    {
        Pass
        {
            Tags
            {
                "LightMode"="ForwardBase"
            }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            struct v2f
            {
                float4 pos:SV_POSITION;
                //float2 uvTex:TEXCOORD0;
                //float2 uvBump:TEXCOORD1;
                //我们可以单独的声明两个float2的成员用于记录 颜色和法线纹理的uv坐标
                //也可以直接声明一个float4的成员 xy用于记录颜色纹理的uv，zw用于记录法线纹理的uv
                float4 uv:TEXCOORD0;
                //噪声纹理的uv
                float2 uvNosie:TEXCOORD1;
                //光的方向 相对于切线空间下的
                float3 lightDir:TEXCOORD2;
                //视角的方向 相对于切线空间下的
                float3 viewDir:TEXCOORD3;
                float3 worldPos:TEXCOORD4;
                SHADOW_COORDS(5)
            };

            float4 _MainColor; //漫反射颜色
            sampler2D _MainTex; //颜色纹理
            float4 _MainTex_ST; //颜色纹理的缩放和平移
            sampler2D _BumpMap; //法线纹理
            float4 _BumpMap_ST; //法线纹理的缩放和平移
            float _BumpScale; //凹凸程度
            float4 _SpecularColor; //高光颜色
            fixed _SpecularNum; //光泽度

            sampler2D _Noise; //噪声纹理
            float4 _Noise_ST; //噪声纹理的缩放和平移
            sampler2D _Gradient; //渐变纹理
            float _Dissolve; //消融进度
            float _EdgeRange; //边缘范围

            v2f vert(appdata_full v)
            {
                v2f data;
                //把模型空间下的顶点转到裁剪空间下
                data.pos = UnityObjectToClipPos(v.vertex);
                //计算纹理的缩放偏移
                data.uv.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                data.uv.zw = v.texcoord.xy * _BumpMap_ST.xy + _BumpMap_ST.zw;

                //噪声纹理的uv
                data.uvNosie = v.texcoord.xy * _Noise_ST.xy + _Noise_ST.zw;

                //在顶点着色器当中 得到 模型空间到切线空间的 转换矩阵
                //切线、副切线、法线
                //计算副切线 计算叉乘结果后 垂直与切线和法线的向量有两条 通过乘以 切线当中的w，就可以确定是哪一条
                float3 binormal = cross(v.normal, v.tangent.xyz) * v.tangent.w;
                //转换矩阵
                float3x3 rotation = float3x3(v.tangent.xyz,
                                             binormal,
                                             v.normal);

                //rotation = transpose(rotation);//转置

                //模型空间下的光的方向
                //data.lightDir = ObjSpaceLightDir(v.vertex);
                //乘以模型空间到切线空间的转换矩阵 就可以得到切线空间下的 光的方向了
                data.lightDir = mul(rotation, ObjSpaceLightDir(v.vertex));

                //模型空间下的视角的方向
                //data.viewDir = ObjSpaceViewDir(v.vertex);
                data.viewDir = mul(rotation, ObjSpaceViewDir(v.vertex));

                data.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                TRANSFER_SHADOW(data);

                return data;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                //剔除-消融
                fixed3 noiseColor = tex2D(_Noise, i.uvNosie.xy).rgb;
                clip(_Dissolve == 1 ? -1 : noiseColor.r - _Dissolve);

                //通过纹理采样函数 取出法线纹理贴图当中的数据
                float4 packedNormal = tex2D(_BumpMap, i.uv.zw);
                //将我们取出来的法线数据 进行逆运算并且可能会进行解压缩的运算，最终得到切线空间下的法线数据
                float3 tangentNormal = UnpackNormal(packedNormal);
                //乘以凹凸程度的系数
                tangentNormal.xy *= _BumpScale;
                tangentNormal.z = sqrt(1.0 - saturate(dot(tangentNormal.xy, tangentNormal.xy)));

                //接下来就来处理 带颜色纹理的 布林方光照模型计算

                //颜色纹理和漫反射颜色的 叠加
                fixed3 albedo = tex2D(_MainTex, i.uv.xy) * _MainColor.rgb;
                //兰伯特
                fixed3 lambertColor = _LightColor0.rgb * albedo.rgb * max(0, dot(tangentNormal, normalize(i.lightDir)));
                //半角向量
                float3 halfA = normalize(normalize(i.viewDir) + normalize(i.lightDir));
                //高光反射
                fixed3 specularColor = _LightColor0.rgb * _SpecularColor.rgb * pow(
                    max(0, dot(tangentNormal, halfA)), _SpecularNum);

                //强度计算
                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos)

                //布林方
                fixed3 color = UNITY_LIGHTMODEL_AMBIENT.rgb * albedo + lambertColor * atten + specularColor;

                //渐变颜色采样
                fixed value = 1 - smoothstep(0, _EdgeRange, noiseColor.r - _Dissolve);
                fixed3 gradientColor = tex2D(_Gradient, fixed2(value, 0.5)).rgb;
                //最终颜色
                fixed3 finalColor = lerp(color, gradientColor, value * step(0.000001, _Dissolve));

                return fixed4(finalColor.rgb, 1);
            }
            ENDCG
        }

        //该注释主要用于进行阴影投影 主要是用来计算阴影映射纹理的
        Pass
        {
            Tags
            {
                "LightMode" = "ShadowCaster"
            }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            //  该编译指令时告诉Unity编译器生成多个着色器变体
            //  用于支持不同类型的阴影（SM，SSSM等等）
            //  可以确保着色器能够在所有可能的阴影投射模式下正确渲染
            #pragma multi_compile_shadowcaster
            //  其中包含了关键的阴影计算相关的宏
            #include "UnityCG.cginc"
            
            sampler2D _Noise;
            float4 _Noise_ST;
            float _Dissolve;

            struct v2f
            {
                //顶点到片元着色器阴影投射结构体数据宏
                //这个宏定义了一些标准的成员变量
                //这些变量用于在阴影投射路径中传递顶点数据到片元着色器
                //我们主要在结构体中使用
                V2F_SHADOW_CASTER;
                //噪声纹理的uv
                float2 uvNosie:TEXCOORD1;
            };

            v2f vert(appdata_base v)
            {
                v2f data;
                //转移阴影投射器法线偏移宏
                //用于在顶点着色器中计算和传递阴影投射所需的变量
                //主要做了
                //2-2-1.将对象空间的顶点位置转换为裁剪空间的位置
                //2-2-2.考虑法线偏移，以减轻阴影失真问题，尤其是在处理自阴影时
                //2-2-3.传递顶点的投影空间位置，用于后续的阴影计算
                //我们主要在顶点着色器中使用
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(data);
                //噪声纹理的uv
                data.uvNosie = v.texcoord.xy * _Noise_ST.xy + _Noise_ST.zw;
                return data;
            }

            float4 frag(v2f i):SV_Target
            {
                //剔除-消融
                fixed3 noiseColor = tex2D(_Noise, i.uvNosie.xy).rgb;
                clip(_Dissolve == 1 ? -1 : noiseColor.r - _Dissolve);
                
                //阴影投射片元宏
                //将深度值写入到阴影映射纹理中
                //我们主要在片元着色器中使用
                SHADOW_CASTER_FRAGMENT(i);
            }
            ENDCG
        }
    }

    Fallback "Diffuse"
}
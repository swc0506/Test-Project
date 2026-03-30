Shader "Unlit/Toon_120"
{
    Properties
    {
        _MainColor("MainColor", Color) = (1,1,1,1)
        _MainTex("MainTex", 2D) = ""{}
        _BumpMap("BumpMap", 2D) = ""{}
        _BumpScale("BumpScale", Range(0,1)) = 1
        _RampTex("RampTex", 2D) = ""{}
        _SpecularColor("SpecularColor", Color) = (1,1,1,1)
        _SpecularNum("SpecularNum", Range(0,1)) = 0.5
        _OutLineColor("OutLineColor", Color) = (0,0,0,1)
        _OutLineWidth("OutLineWidth", Range(0,0.01)) = 0.005
    }
    SubShader
    {
        Pass
        {
            Name "OUTLINE"
            Cull Front
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            
            struct v2f
            {
                float4 pos:SV_POSITION;
            };
            
            // 轮廓颜色
            float4 _OutLineColor;
            // 轮廓宽度
            float _OutLineWidth;
            
            v2f vert (appdata_base v)
            {
                v2f data;
                v.vertex.xyz += normalize(v.normal) * _OutLineWidth;
                data.pos = UnityObjectToClipPos(v.vertex);
                return data;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                return _OutLineColor;
            }
            
            ENDCG
        }
        
        
        Pass
        {
            Tags { "LightMode"="ForwardBase" }
            Cull Back
            
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
                //光的方向 相对于切线空间下的
                float3 lightDir:TEXCOORD1;
                //视角的方向 相对于切线空间下的
                float3 viewDir:TEXCOORD2;
                float3 worldPos:TEXCOORD3;
                SHADOW_COORDS(4)
            };

            float4 _MainColor;//漫反射颜色
            sampler2D _MainTex;//颜色纹理
            float4 _MainTex_ST;//颜色纹理的缩放和平移
            sampler2D _RampTex;//渐变纹理
            float4 _RamText_ST;
            sampler2D _BumpMap;//法线纹理
            float4 _BumpMap_ST;//法线纹理的缩放和平移
            float _BumpScale;//凹凸程度
            float4 _SpecularColor;//高光颜色
            fixed _SpecularNum;//光泽度

            v2f vert (appdata_full v)
            {
                v2f data;
                //把模型空间下的顶点转到裁剪空间下
                data.pos = UnityObjectToClipPos(v.vertex);
                //计算纹理的缩放偏移
                data.uv.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                data.uv.zw = v.texcoord.xy * _BumpMap_ST.xy + _BumpMap_ST.zw;

                //在顶点着色器当中 得到 模型空间到切线空间的 转换矩阵
                //切线、副切线、法线
                //计算副切线 计算叉乘结果后 垂直与切线和法线的向量有两条 通过乘以 切线当中的w，就可以确定是哪一条
                float3 binormal = cross(v.normal, v.tangent.xyz) * v.tangent.w;
                //转换矩阵
                float3x3 rotation = float3x3( v.tangent.xyz,
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
                TRANSFER_SHADOW(data)
                
                return data;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //通过纹理采样函数 取出法线纹理贴图当中的数据
                float4 packedNormal = tex2D(_BumpMap, i.uv.zw);
                //将我们取出来的法线数据 进行逆运算并且可能会进行解压缩的运算，最终得到切线空间下的法线数据
                float3 tangentNormal = UnpackNormal(packedNormal);
                //乘以凹凸程度的系数
                tangentNormal.xy *= _BumpScale;
                tangentNormal.z = sqrt(1.0 - saturate(dot(tangentNormal.xy, tangentNormal.xy)));// z轴方向 就是 切线空间下的法线方向 的 模长 

                //接下来就来处理 带颜色纹理的 布林方光照模型计算

                //颜色纹理和漫反射颜色的 叠加
                fixed3 albedo = tex2D(_MainTex, i.uv.xy) * _MainColor.rgb;
                //半兰伯特 加渐变纹理相关计算方式
                fixed halfLambert = dot(normalize(tangentNormal), normalize(i.lightDir)) * 0.5 + 0.5;
                
                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos)
                halfLambert *= atten;
                
                fixed3 diffuseColor = _LightColor0.rgb * albedo.rgb * tex2D(_RampTex, fixed2(halfLambert, halfLambert)).rgb;
                //半角向量
                float3 halfA = normalize(normalize(i.viewDir) + normalize(i.lightDir));
                //高光反射
                //fixed3 specularColor = _LightColor0.rgb * _SpecularColor.rgb * pow(max(0, dot(tangentNormal, halfA)), _SpecularNum);
                fixed spec = dot(normalize(tangentNormal), halfA);
                //step函数 大于_SpecularNum 返回1 小于_SpecularNum 返回0
                spec = step(_SpecularNum, spec);
                fixed3 specularColor = _SpecularColor.rgb * spec;
                
                //布林方
                fixed3 color = UNITY_LIGHTMODEL_AMBIENT.rgb * albedo + diffuseColor + specularColor;

                return fixed4(color.rgb, 1);
            }
            ENDCG
        }
    }

    Fallback "Specular"
}

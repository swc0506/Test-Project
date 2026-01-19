Shader "Unlit/W_Normal"
{
    Properties
    {
        _MainColor("MainColor", Color) = (1,1,1,1)
        _MainTex("MainTex", 2D) = ""{}
        _BumpMap("BumpMap", 2D) = ""{}
        _BumpScale("BumpScale", Range(0,1)) = 1
        _SpecularColor("SpecularColor", Color) = (1,1,1,1)
        _SpecularNum("SpecularNum", Range(0,20)) = 18
    }
    SubShader
    {
        Pass
        {
            Tags { "LightMode"="ForwardBase" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct v2f
            {
                float4 pos:SV_POSITION;
                //float2 uvTex:TEXCOORD0;
                //float2 uvBump:TEXCOORD1;
                //我们可以单独的声明两个float2的成员用于记录 颜色和法线纹理的uv坐标
                //也可以直接声明一个float4的成员 xy用于记录颜色纹理的uv，zw用于记录法线纹理的uv
                float4 uv:TEXCOORD0;
                //顶点相对世界坐标位置 用于视角方向计算
                //float3 woldPos:TEXCOORD1;
                //切线 到 世界空间的变换矩阵
                //float3x3 rotation:TEXCOORD2;

                //代表我们切线空间到世界空间的变换矩阵的3行 4的效率更高
                float4 T2W0:TEXCOORD1;
                float4 T2W1:TEXCOORD2;
                float4 T2W2:TEXCOORD3;
            };

            float4 _MainColor;//漫反射颜色
            sampler2D _MainTex;//颜色纹理
            float4 _MainTex_ST;//颜色纹理的缩放和平移
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

                //得到世界空间下的顶点位置
                float3 woldPos = mul(unity_ObjectToWorld, v.vertex);
                //得到世界空间下的法线位置
                float3 wNormal = UnityObjectToWorldNormal(v.normal);
                //得到世界空间下的切线
                float3 wTangent = UnityObjectToWorldDir(v.tangent);
                
                //在顶点着色器当中 得到 切线空间到世界空间的 转换矩阵
                //切线、副切线、法线
                //计算副切线 计算叉乘结果后 垂直与切线和法线的向量有两条 通过乘以 切线当中的w，就可以确定是哪一条
                float3 binormal = cross(wNormal, wTangent) * v.tangent.w;
                //转换矩阵
                // data.rotation = float3x3( wTangent.x, binormal.x, wNormal.x,
                //                           wTangent.y, binormal.y, wNormal.y,
                //                           wTangent.z, binormal.z, wNormal.z);
                data.T2W0 = float4(wTangent.x, binormal.x, wNormal.x, woldPos.x);
                data.T2W1 = float4(wTangent.y, binormal.y, wNormal.y, woldPos.y);
                data.T2W2 = float4(wTangent.z, binormal.z, wNormal.z, woldPos.z);

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
                tangentNormal.z = sqrt(1.0 - saturate(dot(tangentNormal.xy, tangentNormal.xy)));
                //把切线空间下的法线转换到世界空间下
                float3 wPos = float3(i.T2W0.w, i.T2W1.w, i.T2W2.w);
                //float3x3 rotation = float3x3(i.T2W0.xyz, i.T2W1.xyz, i.T2W2.xyz);
                //float3 wNormal = mul(rotation, tangentNormal);
                float3 wNormal = float3(dot(i.T2W0.xyz, tangentNormal), dot(i.T2W1.xyz, tangentNormal), dot(i.T2W2.xyz, tangentNormal));

                //接下来就来处理 带颜色纹理的 布林方光照模型计算
                fixed3 lightDir = normalize(_WorldSpaceLightPos0.xyz);//世界空间下光的方向
                fixed3 viewDir = normalize(UnityWorldSpaceViewDir(wPos));
                
                //颜色纹理和漫反射颜色的 叠加
                fixed3 albedo = tex2D(_MainTex, i.uv.xy) * _MainColor.rgb;
                //兰伯特
                fixed3 lambertColor = _LightColor0.rgb * albedo.rgb * max(0, dot(wNormal, lightDir));
                //半角向量
                float3 halfA = normalize(viewDir + lightDir);
                //高光反射
                fixed3 specularColor = _LightColor0.rgb * _SpecularColor.rgb * pow(max(0, dot(wNormal, halfA)), _SpecularNum);
                //布林方
                fixed3 color = UNITY_LIGHTMODEL_AMBIENT.rgb * albedo + lambertColor + specularColor;

                return fixed4(color.rgb, 1);
            }
            ENDCG
        }
    }
}

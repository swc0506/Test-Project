Shader "Unlit/Phong_F"
{
    Properties
    {
        _MainColor("MainColor", Color) = (1,1,1,1)//材质漫反射颜色
        _SpecularColor ("SpecularColor", Color) = (1,1,1,1)//高光反射光照颜色
        _SpecularNum("SpecularNum", Range(0, 20)) = 0.5//幂次(光泽度)
    }
    SubShader
    {
        Tags { "LightMode"="ForwardBase" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            fixed4 _MainColor;
            fixed4 _SpecularColor;
            half _SpecularNum;
            
            struct v2f
            {
                float4 pos:SV_POSITION;//裁剪空间下的顶点位置信息
                float3 normal:NORMAL;//世界空间下的法线位置
                float3 wPos:TEXCOORD0;//世界空间下的顶点坐标(纹理坐标语义)
            };

            //计算兰伯特光照模型 颜色 相关函数
            fixed3 getLambertColor(in float3 objNormal, in float3 lightDir)
            {
                return _LightColor0.rgb * _MainColor.rgb * max(0, dot(objNormal, lightDir));
            }

            //计算高光反射光照模型 颜色 相关函数
            fixed3 getSpecularColor(in float3 wPos, in float3 objNormal, in float3 lightDir)
            {
                float3 viewDir = _WorldSpaceCameraPos.xyz - wPos;//视角方向
                viewDir = normalize(viewDir);
                float3 reflectDir = reflect(-lightDir, objNormal);

                return _LightColor0.rgb * _SpecularColor.rgb * pow(max(0, dot(viewDir, reflectDir)), _SpecularNum);
            }
            
            v2f vert (appdata_base v)
            {
                v2f v2fData;
                v2fData.pos = UnityObjectToClipPos(v.vertex);
                v2fData.normal = UnityObjectToWorldNormal(v.normal);
                v2fData.wPos = mul(UNITY_MATRIX_M, v.vertex).xyz;//模型空间的顶点位置 转换到 世界坐标
                return v2fData;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //得到光源单位向量
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float3 worldNormal = normalize(i.normal);//对插值后的法线进行归一化
                
                fixed3 lamColor = getLambertColor(worldNormal, lightDir);
                fixed3 specular = getSpecularColor(i.wPos, worldNormal, lightDir);
                fixed3 color = lamColor + specular + UNITY_LIGHTMODEL_AMBIENT.rgb;
                return fixed4(color.rgb, 1);
            }
            ENDCG
        }
    }
}

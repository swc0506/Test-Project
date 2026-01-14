Shader "Unlit/Phong"
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
                float4 pos:SV_POSITION;//裁剪坐标系
                fixed3 color:COLOR;//对应顶点phong反射颜色
            };

            //计算兰伯特光照模型 颜色 相关函数
            fixed3 getLambertColor(in float3 objNormal, in float3 lightDir)
            {
                return _LightColor0.rgb * _MainColor.rgb * max(0, dot(objNormal, lightDir));
            }

            //计算高光反射光照模型 颜色 相关函数
            fixed3 getSpecularColor(in float4 objVertex, in float3 objNormal, in float3 lightDir)
            {
                float3 worldPos = mul(UNITY_MATRIX_M,objVertex);
                float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - worldPos);//视角方向
                float3 reflectDir = reflect(-lightDir, objNormal);

                return _LightColor0.rgb * _SpecularColor.rgb * pow(max(0, dot(viewDir, reflectDir)), _SpecularNum);
            }

            v2f vert (appdata_base v)
            {
                v2f v2fData;
                v2fData.pos = UnityObjectToClipPos(v.vertex);
                float3 normal = UnityObjectToWorldNormal(v.normal);
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                fixed3 lambert = getLambertColor(normal, lightDir);
                fixed3 specular = getSpecularColor(v.vertex, normal, lightDir);
                v2fData.color = lambert + specular + UNITY_LIGHTMODEL_AMBIENT.rgb;//phong光照模型
                return v2fData;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return fixed4(i.color.rgb, 1);
            }
            ENDCG
        }
    }
}

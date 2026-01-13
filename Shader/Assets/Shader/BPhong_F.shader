Shader "Unlit/BPhong_F"
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
                float3 wNormal:NORMAL;//世界空间下的法线位置
                float3 wPos:TEXCOORD0;//世界空间下的顶点坐标(纹理坐标语义)
            };

            //计算兰伯特光照模型 颜色 相关函数
            fixed3 getLambertColor(in float3 objNormal, in float3 lightDir)
            {
                return _LightColor0.rgb * _MainColor.rgb * max(0, dot(objNormal, lightDir));
            }

            //计算高光反射光照模型(BPS) 颜色 相关函数
            fixed3 getSpecularColor(in float3 wPos, in float3 objNormal, in float3 lightDir)
            {
                float3 viewDir = _WorldSpaceCameraPos.xyz - wPos;//视角方向
                viewDir = normalize(viewDir);

                float3 halfVert = normalize(viewDir + lightDir);//半角方向向量

                return _LightColor0.rgb * _SpecularColor.rgb * pow(max(0, dot(objNormal, halfVert)), _SpecularNum);
            }

            v2f vert (appdata_base v)
            {
                v2f v2fData;
                v2fData.pos = UnityObjectToClipPos(v.vertex);
                v2fData.wNormal = UnityObjectToWorldNormal(v.normal);
                v2fData.wPos = mul(UNITY_MATRIX_M,v.vertex);
                return v2fData;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float3 wNormal = normalize(i.wNormal);
                fixed3 lambert = getLambertColor(wNormal, lightDir);
                fixed3 specular = getSpecularColor(i.wPos, wNormal, lightDir);
                fixed3 color = lambert + specular + UNITY_LIGHTMODEL_AMBIENT.rgb;//phong光照模型
                return fixed4(color.rgb, 1);
            }
            ENDCG
        }
    }
}

Shader "Unlit/Specular_F"
{
    Properties
    {
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

            fixed4 _SpecularColor;
            half _SpecularNum;

            struct v2f
            {
                float4 pos:SV_POSITION;//裁剪空间下的顶点位置信息
                float3 wNormal:NORMAL;//世界空间下的法线位置
                float3 wPos:TEXCOORD0;//世界空间下的顶点坐标(纹理坐标的语义)
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata_base v)
            {
                v2f v2fData;
                v2fData.pos = UnityObjectToClipPos(v.vertex);
                
                v2fData.wPos = mul(UNITY_MATRIX_M, v.vertex).xyz;//模型空间的顶点位置 转换到 世界坐标
                v2fData.wNormal = UnityObjectToWorldNormal(v.normal);//法线在世界空间下的向量
                return v2fData;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //Phong式高反射光照模型
                float3 viewDir = _WorldSpaceCameraPos.xyz - i.wPos;//视角方向
                viewDir = normalize(viewDir);//归一化
                
                //标准化后光的反射方向
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float3 reflectDir = reflect(-lightDir, normalize(i.wNormal));//反射光线向量

                fixed3 color = _LightColor0.rgb * _SpecularColor.rgb * pow(max(0, dot(viewDir, reflectDir)), _SpecularNum);
                return fixed4(color.xyz, 1);
            }
            ENDCG
        }
    }
}

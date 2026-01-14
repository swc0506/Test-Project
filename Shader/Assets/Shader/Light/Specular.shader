Shader "Unlit/Specular"
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
                float4 pos:SV_POSITION;
                fixed3 color:COLOR;
            };

            v2f vert (appdata_base v)
            {
                v2f v2fData;
                v2fData.pos = UnityObjectToClipPos(v.vertex);

                //Phong式高反射光照模型
                //1.标准化后观察方向向量 模型空间的顶点位置 转换到 世界坐标
                float3 worldPos = mul(UNITY_MATRIX_M, v.vertex);
                float3 viewDir = _WorldSpaceCameraPos.xyz - worldPos;//视角方向
                viewDir = normalize(viewDir);//归一化
                
                //2.标准化后的反射方向
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float3 normal = UnityObjectToWorldNormal(v.normal);//法线在世界空间下的向量
                float3 reflectDir = reflect(-lightDir, normal);//反射光线向量

                v2fData.color = _LightColor0.rgb * _SpecularColor.rgb * pow(max(0, dot(viewDir, reflectDir)), _SpecularNum);
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

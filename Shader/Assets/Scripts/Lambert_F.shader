Shader "Unlit/Lambert_F"
{
    //兰伯特逐片元
    Properties
    {
        _MainColor("MainColor", Color) = (1,1,1,1)
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

            float4 _MainColor;
            
            struct v2f
            {
                float4 pos:SV_POSITION;//裁剪空间下的顶点位置信息
                float3 normal:NORMAL;//世界空间下的法线位置
            };

            v2f vert (appdata_base v)
            {
                v2f v2fData;
                v2fData.pos = UnityObjectToClipPos(v.vertex);
                v2fData.normal = UnityObjectToWorldNormal(v.normal);
                return v2fData;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //得到光源单位向量
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float3 worldNormal = normalize(i.normal);//对插值后的法线进行归一化

                //兰伯特
                //fixed3 color = _LightColor0.rgb * _MainColor.rgb * max(0, dot(worldNormal, lightDir)) + UNITY_LIGHTMODEL_AMBIENT;
                //半兰伯特
                fixed3 color = _LightColor0.rgb * _MainColor.rgb * (dot(worldNormal, lightDir) * 0.5 + 0.5);
                
                return fixed4(color.rgb, 1);
            }
            ENDCG
        }
    }
}

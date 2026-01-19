Shader "Unlit/Gradient"
{
    Properties
    {
        _MainColor("MainColor", Color) = (1,1,1,1)
        _RampTex("RampTex", 2D) = ""{}
        _SpecularColor("SpecularColor", Color) = (1,1,1,1)
        _SpecularNum("SpecularNum", Range(8, 256)) = 18
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            fixed4 _MainColor;
            sampler2D _RampTex;
            float4 _RamText_ST;
            fixed4 _SpecularColor;
            float _SpecularNum;
            
            struct v2f
            {
                float4 pos:SV_POSITION;//裁剪空间
                float3 worldPos:TEXCOORD0;//世界空间
                float3 wNormal:TEXCOORD1;
            };

            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.wNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //光的方向
                float3 lightDir = normalize(_WorldSpaceLightPos0);
                float3 wNormal = normalize(i.wNormal);
                //漫放射（通过渐变纹理得到的颜色进行叠加）
                fixed halfLambert = dot(wNormal, lightDir) * 0.5 +0.5;
                fixed3 diffuse = _LightColor0.rgb * _MainColor.rgb * tex2D(_RampTex, fixed2(halfLambert, halfLambert));
                //高光反射
                float3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
                float3 halfDir = normalize(lightDir + viewDir);
                fixed3 specular = _LightColor0.rgb * _SpecularColor.rgb * pow(max(0, dot(wNormal, halfDir)), _SpecularNum);

                fixed4 color = fixed4(UNITY_LIGHTMODEL_AMBIENT.rgb + diffuse + specular, 1);//避免最终效果偏灰
                
                return color;
            }
            ENDCG
        }
    }
}

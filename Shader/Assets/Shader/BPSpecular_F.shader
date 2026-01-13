Shader "Unlit/BPSpecular_F"
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
                float3 wNormal:NORMAL;//世界空间下的法线位置
                float3 wPos:TEXCOORD0;//世界空间下的顶点坐标(纹理坐标的语义)
            };

            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.wNormal = UnityObjectToWorldNormal(v.normal);
                o.wPos = mul(UNITY_MATRIX_M, v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //高光反射光照颜色 = 光源的颜色 * 材质高光反射颜色 * max (0, 标准化后顶点法线方向向量 · 标准化后半角向量方向向量)幂
                float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.wPos.xyz);//视角方向
                float3 lightDIr = normalize(_WorldSpaceLightPos0.xyz);//光线方向
                float3 halfVert = normalize(viewDir + lightDIr);//半角方向向量
                fixed3 color = _LightColor0.rgb * _SpecularColor.rgb * pow(max(0, dot(normalize(i.wNormal), halfVert)), _SpecularNum);
                return fixed4(color.rgb, 1);
            }
            ENDCG
        }
    }
}

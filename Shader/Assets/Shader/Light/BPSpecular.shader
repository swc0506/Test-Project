Shader "Unlit/BPSpecular"
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
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                //高光反射光照颜色 = 光源的颜色 * 材质高光反射颜色 * max (0, 标准化后顶点法线方向向量 · 标准化后半角向量方向向量)幂
                float3 wNormal = UnityObjectToWorldNormal(v.normal);
                float3 wPos = mul(UNITY_MATRIX_M, v.vertex);
                float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - wPos.xyz);//视角方向
                float3 lightDIr = normalize(_WorldSpaceLightPos0.xyz);//光线方向
                float3 halfVert = normalize(viewDir + lightDIr);//半角方向向量
                o.color = _LightColor0.rgb * _SpecularColor.rgb * pow(max(0, dot(wNormal, halfVert)), _SpecularNum);
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return fixed4(i.color.rgb, 1);
            }
            ENDCG
        }
    }
}

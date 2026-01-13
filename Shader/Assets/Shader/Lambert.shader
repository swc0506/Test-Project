Shader "Unlit/Lambert"
{
    Properties
    {
        _MainColor("MainColor", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "LightMode"="ForwardBase" }//向前渲染光照模式

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"//自带结构体
            #include "Lighting.cginc"

            //材质漫反射颜色
            fixed4 _MainColor;
            
            struct v2f
            {
                float4 pos:SV_POSITION;//裁剪坐标系
                fixed3 color:COLOR;//对应顶点漫反射
            };

            //逐顶点光照
            v2f vert (appdata_base v)
            {
                v2f v2fData;

                //模型下的顶点位置->裁剪空间
                v2fData.pos = UnityObjectToClipPos(v.vertex);
                
                //模型空间下的法线
                //v.normal
                //获取到 相对于世界坐标系下的 法线信息
                float3 normal = UnityObjectToWorldNormal(v.normal);
                float3 lighterDir = normalize(_WorldSpaceLightPos0.xyz);

                //兰伯特光照模型 _LightColor0 光照颜色
                //v2fData.color = _LightColor0.rgb * _MainColor.rgb * max(0, dot(normal, lighterDir));
                //v2fData.color = v2fData.color + UNITY_LIGHTMODEL_AMBIENT.rgb;//加上兰伯特环境光变量，防止阴影全黑
                //半兰伯特
                v2fData.color = _LightColor0.rgb * _MainColor.rgb * (dot(normal, lighterDir) * 0.5 + 0.5);
                return v2fData;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //将兰伯特光照颜色传递出去
                return fixed4(i.color.rgb, 1);
            }
            ENDCG
        }
    }
}

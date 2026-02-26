Shader "Unlit/Refract_78"
{
    Properties
    {
        //漫反射颜色
        _Color("Color", Color) = (1,1,1,1)
        //折射颜色
        _RefractColor("ReflectColor", Color) = (1,1,1,1)

        //折射率比值
        _RefractRatio ("Refract Ratio", Range(0.1, 1)) = 0.5
        //立方体纹理贴图
        _CubeMap ("Cube Map", Cube) = ""{}
        //折射程度
        _RefractAmount ("Refract Amount", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            samplerCUBE _CubeMap;
            fixed4 _Color;
            fixed4 _RefractColor;
            float _RefractRatio;
            float _RefractAmount;

            struct v2f
            {
                float4 pos : SV_POSITION; //裁剪空间下的顶点位置
                float3 worldNormal:Normal; //世界空间下的法线
                float3 worldPos:TEXCOORD0; //世界空间下的顶点位置
                float3 worldRefr:TEXCOORD1; //折射向量

                SHADOW_COORDS(2) //阴影坐标
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal); //世界空间下的法线
                o.worldPos = mul(unity_ObjectToWorld, v.vertex); //世界空间下的顶点位置
                fixed3 worldViewDIr = UnityWorldSpaceViewDir(o.worldPos); //世界空间下的视线方向 摄像机 - 顶点位置
                o.worldRefr = refract(-normalize(worldViewDIr), normalize(o.worldNormal), _RefractRatio);
                //入射介质折射率/出射介质折射率

                //计算阴影坐标
                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                //漫反射关照相关计算
                //得到光的方向
                fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
                //计算漫反射光的强度
                fixed3 diffuse = _LightColor0.rgb * _Color.rgb * saturate(dot(i.worldNormal, worldLightDir));

                //立方体纹理采样
                fixed3 cubeColor = texCUBE(_CubeMap, i.worldRefr).rgb * _RefractColor.rgb;

                //得到光照衰减值以及阴影相关的衰减
                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
                //计算环境光的强度 在漫反射颜色和反射颜色之间 进行插值 0和1就是极限状态 0 没有折射 1 只有折射
                fixed3 color = UNITY_LIGHTMODEL_AMBIENT.rgb + lerp(diffuse, cubeColor, _RefractAmount) * atten;

                return fixed4(color, 1);
            }
            ENDCG
        }
    }
    FallBack "Refractive/VertexLit"
}
Shader "Unlit/Reflect_76"
{
    Properties
    {
        //漫反射颜色
        _Color("Color", Color) = (1,1,1,1)
        //反射颜色
        _ReflectColor("ReflectColor", Color) = (1,1,1,1)

        _Cube("Cube", Cube) = ""{}
        // 反射率
        _Reflectivity("Reflectivity", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque" "Queue"="Geometry"
        }

        Pass
        {
            Tags
            {
                "LightMode"="ForwardBase"
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase//开启阴影投射

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            samplerCUBE _Cube;
            float _Reflectivity;
            float4 _Color;
            float4 _ReflectColor;

            struct v2f
            {
                float4 pos : SV_POSITION; //裁剪空间下的顶点坐标
                fixed3 worldNormal : NORMAL; //世界空间下的法线
                float3 worldPos : TEXCOORD0; //世界空间下的顶点坐标
                float3 worldReflect : TEXCOORD1; //世界空间下的反射向量
                //阴影相关宏
                SHADOW_COORDS(2)
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex); //顶点坐标转换到裁剪空间
                o.worldNormal = UnityObjectToWorldNormal(v.normal); //法线转换到世界空间
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz; //顶点坐标转换到世界空间
                fixed3 worldViewDir = UnityWorldSpaceViewDir(o.worldPos);
                //视线方向转换到世界空间 内部是用_WorldSpaceCameraPos - worldPos
                o.worldReflect = reflect(-worldViewDir, o.worldNormal); //计算反射向量

                //阴影相关宏 计算阴影坐标
                TRANSFER_SHADOW(o)
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                //漫反射关照相关计算
                //得到光的方向
                fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
                //计算漫反射光的强度
                fixed3 diffuse = _LightColor0.rgb * _Color.rgb * saturate(dot(i.worldNormal, worldLightDir));

                //对立方体纹理利用对应的反射向量进行采样
                fixed3 cubeColor = texCUBE(_Cube, i.worldReflect).rgb * _ReflectColor.rgb;

                //得到光照衰减值以及阴影相关的衰减
                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);

                //计算环境光的强度 在漫反射颜色和反射颜色之间 进行插值 0和1就是极限状态 0 没有反射 1 只有反射
                fixed3 color = UNITY_LIGHTMODEL_AMBIENT.xyz + lerp(diffuse, cubeColor, _Reflectivity) * atten;
                return fixed4(color, 1);
            }
            ENDCG
        }
    }
    FallBack "Reflective/VertexLit"
}
Shader "Unlit/ReflectBase_75"
{
    Properties
    {
        _Cube("Cube", Cube) = ""{}
        // 反射率
        _Reflectivity("Reflectivity", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque"  "Queue"="Geometry"}
        
        Pass
        {
            Tags {"LightMode"="ForwardBase"}
            
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            
            samplerCUBE _Cube;
            float _Reflectivity;
            
            struct v2f
            {
                float4 pos : SV_POSITION;//裁剪空间下的顶点坐标
                float3 worldReflect : TEXCOORD0;//世界空间下的反射向量
            };
            
            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);//顶点坐标转换到裁剪空间
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);//法线转换到世界空间
                fixed3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;//顶点坐标转换到世界空间
                fixed3 worldViewDir = UnityWorldSpaceViewDir(worldPos);//视线方向转换到世界空间 内部是用_WorldSpaceCameraPos - worldPos
                o.worldReflect = reflect(-worldViewDir, worldNormal);//计算反射向量
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 c = texCUBE(_Cube, i.worldReflect);//从Cube贴图中采样颜色
                return c * _Reflectivity;
            }
            
            ENDCG
        }
    }
}

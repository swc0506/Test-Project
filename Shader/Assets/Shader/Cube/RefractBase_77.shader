Shader "Unlit/RefractBase_77"
{
    Properties
    {
        //介质A折射率
        _RefractIndexA ("Refract Index A", Range(1, 2)) = 1
        //介质B折射率
        _RefractIndexB ("Refract Index B", Range(1, 2)) = 1
        //立方体纹理贴图
        _CubeMap ("Cube Map", Cube) = ""{}
        //折射程度
        _RefractAmount ("Refract Amount", Range(0, 1)) = 1
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
            
            samplerCUBE _CubeMap;
            float4 _CubeMap_TexelSize;
            float _RefractIndexA;
            float _RefractIndexB;
            float _RefractAmount;
            
            struct v2f
            {
                float4 pos : SV_POSITION;//裁剪空间下的顶点位置
                float3 worldRefr:TEXCOORD0;//折射向量
            };
            
            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);//世界空间下的法线
                fixed3 worldPos = mul(unity_ObjectToWorld, v.vertex);//世界空间下的顶点位置
                fixed3 worldViewDIr = UnityWorldSpaceViewDir(worldPos);//世界空间下的视线方向 摄像机 - 顶点位置
                o.worldRefr = refract(-normalize(worldViewDIr), normalize(worldNormal), _RefractIndexA / _RefractIndexB);//入射介质折射率/出射介质折射率
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                //立方体纹理采样
                fixed4 color = texCUBE(_CubeMap, i.worldRefr) * _RefractAmount;
                return color;
            }
            
            ENDCG
        }
    }
}

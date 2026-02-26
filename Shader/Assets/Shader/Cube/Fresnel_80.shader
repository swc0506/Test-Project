Shader "Unlit/Fresnel_80"
{
    Properties
    {
        //漫反射颜色
        _Color("Color", Color) = (1,1,1,1)
        _Cube("Cube", Cube) = ""{}
        // 菲涅尔反射率
        _FresnelScale("FresnelScale", Range(0, 1)) = 1
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
            #include "Lighting.cginc"
            #include "AutoLight.cginc"
            
            samplerCUBE _Cube;
            uniform float4 _Color;
            float _FresnelScale;
            
            struct v2f
            {
                float4 pos : SV_POSITION;//裁剪空间下的顶点坐标
                float3 worldNoraml : NORMAL;//世界空间下的法线
                float3 worldPos : TEXCOORD0;//世界空间下的顶点坐标
                float3 worldViewDir : TEXCOORD1;//世界空间下的视线方向
                float3 worldReflect : TEXCOORD2;//世界空间下的反射向量
                
                SHADOW_COORDS(3)
            };
            
            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);//顶点坐标转换到裁剪空间
                o.worldNoraml = UnityObjectToWorldNormal(v.normal);//法线转换到世界空间
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;//顶点坐标转换到世界空间
                o.worldViewDir = UnityWorldSpaceViewDir(o.worldPos);//视线方向转换到世界空间 内部是用_WorldSpaceCameraPos - worldPos
                o.worldReflect = reflect(-o.worldViewDir, o.worldNoraml);//计算反射向量
                
                TRANSFER_SHADOW(o);
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                //漫反射关照相关计算
                //得到光的方向
                fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
                //计算漫反射光的强度
                fixed3 diffuse = _LightColor0.rgb * _Color.rgb * saturate(dot(i.worldNoraml, worldLightDir));
                
                fixed3 c = texCUBE(_Cube, i.worldReflect).rgb;//从Cube贴图中采样颜色
                
                //得到光的衰减系数
                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
                
                //利用schlick近似公式计算菲涅尔反射率
                fixed fresnel = _FresnelScale + (1 - _FresnelScale) * pow(1 - dot(normalize(i.worldViewDir), normalize(i.worldNoraml)), 5);
                
                //菲涅尔反射率混合Cube贴图颜色
                fixed3 color = UNITY_LIGHTMODEL_AMBIENT.rgb + lerp(diffuse.rgb, c.rgb, fresnel) * atten;
                
                //根据菲涅尔反射率混合原始颜色和Cube贴图颜色
                return fixed4(color, 1) * fresnel;
            }
            
            ENDCG
        }
    }
}

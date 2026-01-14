Shader "Unlit/Textu_Light"
{
    //单张纹理shader和BP光照模型结合
    Properties
    {
        _MainTex("MainTex", 2D) = ""{}
        _MainColor("MainColor", Color) = (1,1,1,1)//材质漫反射颜色
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

            fixed4 _MainColor;
            fixed4 _SpecularColor;
            half _SpecularNum;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            struct v2f
            {
                float4 pos:SV_POSITION;//裁剪空间下的顶点位置信息
                float2 uv:TEXCOORD0;//世界空间下的UV坐标(纹理坐标语义)
                float3 wNormal:NORMAL;//世界空间下的法线位置
                float3 wPos:TEXCOORD1;//世界空间下的顶点坐标(纹理坐标语义)
            };
            
            //计算高光反射光照模型(BPS) 颜色 相关函数
            fixed3 getSpecularColor(in float3 wPos, in float3 objNormal, in float3 lightDir)
            {
                float3 viewDir = _WorldSpaceCameraPos.xyz - wPos;//视角方向
                viewDir = normalize(viewDir);

                float3 halfVert = normalize(viewDir + lightDir);//半角方向向量

                return _LightColor0.rgb * _SpecularColor.rgb * pow(max(0, dot(objNormal, halfVert)), _SpecularNum);
            }

            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.wNormal = UnityObjectToWorldNormal(v.normal);
                o.wPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //纹理颜色需要和漫反射材质颜色叠加（乘法） 共同决定最终的颜色
                fixed3 albedo = tex2D(_MainTex, i.uv).rgb * _MainColor.rgb;
                
                float3 normal = normalize(i.wNormal);
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                
                fixed3 color1 = _LightColor0.rgb * albedo * max(0, dot(normal, lightDir));//兰伯特

                //float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.wPos);//视角方向
                float3 viewDir = normalize(UnityWorldSpaceViewDir(i.wPos));//视角方向
                float3 halfVert = normalize(viewDir + lightDir);//半角方向向量
                fixed3 color2 = _LightColor0.rgb * _SpecularColor.rgb * pow(max(0, dot(normal, halfVert)), _SpecularNum);//Phong

                fixed3 color = UNITY_LIGHTMODEL_AMBIENT.rgb * albedo + color1 + color2;//避免最终效果偏灰
                return fixed4(color,1);
            }
            ENDCG
        }
    }
}

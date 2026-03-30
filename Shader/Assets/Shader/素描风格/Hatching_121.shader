Shader "Unlit/Hatching_121"
{
    Properties
    {
        //整体颜色叠加
        _Color("Color", Color) = (1,1,1,1)
        //平铺纹理的系数
        _TileFactor("Tile Factor", Float) = 1.0
        //六张素描纹理
        _Sketch0("Sketch0", 2D) = "white" {}
        _Sketch1("Sketch1", 2D) = "white" {}
        _Sketch2("Sketch2", 2D) = "white" {}
        _Sketch3("Sketch3", 2D) = "white" {}
        _Sketch4("Sketch4", 2D) = "white" {}
        _Sketch5("Sketch5", 2D) = "white" {}

        _OutLineColor("OutLineColor", Color) = (0,0,0,1)
        _OutLineWidth("OutLineWidth", Range(0,0.01)) = 0.005
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }

        UsePass "Unlit/Toon_120/OUTLINE"

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            fixed4 _Color;
            float _TileFactor;
            sampler2D _Sketch0;
            sampler2D _Sketch1;
            sampler2D _Sketch2;
            sampler2D _Sketch3;
            sampler2D _Sketch4;
            sampler2D _Sketch5;

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                //xyz分别代表六张素描纹理的权重
                fixed3 sketchWeight0 : TEXCOORD1;
                fixed3 sketchWeight1 : TEXCOORD2;

                float3 worldPos : TEXCOORD3;
                SHADOW_COORDS(4)
            };


            v2f vert(appdata_base v)
            {
                v2f o;
                //顶点坐标转换
                o.vertex = UnityObjectToClipPos(v.vertex);
                //uv坐标平铺缩放 值越大 之后的素描细节越多 越小 细节越粗糙
                o.uv = v.texcoord.xy * _TileFactor;
                //世界空间光照方向
                fixed3 worldLightDir = normalize(WorldSpaceLightDir(v.vertex));
                //世界空间法线方向转换
                fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
                //兰伯特漫反射光照强度 0-1
                fixed diff = max(0, dot(worldLightDir, worldNormal));

                //将其扩充到0-7 7/7 = 1 越大越亮
                diff = diff * 7.0;

                //初始化权重
                o.sketchWeight0 = fixed3(0, 0, 0);
                o.sketchWeight1 = fixed3(0, 0, 0);

                if (diff > 6.0) //最亮的
                {
                    //不会改变权重
                }
                else if (diff > 5.0) //第二亮的
                {
                    o.sketchWeight0.x = diff - 5.0;
                }
                else if (diff > 4.0) //第三亮的
                {
                    o.sketchWeight0.y = diff - 4.0;
                    o.sketchWeight0.x = 1.0 - o.sketchWeight0.y;
                }
                else if (diff > 3.0) //第四亮的
                {
                    o.sketchWeight0.z = diff - 3.0;
                    o.sketchWeight0.y = 1.0 - o.sketchWeight0.z;
                }
                else if (diff > 2.0) //第五亮的
                {
                    o.sketchWeight1.x = diff - 2.0;
                    o.sketchWeight0.z = 1.0 - o.sketchWeight1.x;
                }
                else if (diff > 1.0) //第六亮的
                {
                    o.sketchWeight1.y = diff - 1.0;
                    o.sketchWeight1.x = 1.0 - o.sketchWeight1.y;
                }
                else //最暗的
                {
                    o.sketchWeight1.z = diff;
                    o.sketchWeight1.y = 1.0 - o.sketchWeight1.z;
                }

                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                TRANSFER_SHADOW(o)

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 sketchColor0 = tex2D(_Sketch0, i.uv) * i.sketchWeight0.x;
                fixed4 sketchColor1 = tex2D(_Sketch1, i.uv) * i.sketchWeight0.y;
                fixed4 sketchColor2 = tex2D(_Sketch2, i.uv) * i.sketchWeight0.z;
                fixed4 sketchColor3 = tex2D(_Sketch3, i.uv) * i.sketchWeight1.x;
                fixed4 sketchColor4 = tex2D(_Sketch4, i.uv) * i.sketchWeight1.y;
                fixed4 sketchColor5 = tex2D(_Sketch5, i.uv) * i.sketchWeight1.z;
                fixed4 witheColor = fixed4(1, 1, 1, 1) * (1 - (i.sketchWeight0.x + i.sketchWeight0.y
                    + i.sketchWeight0.z + i.sketchWeight1.x + i.sketchWeight1.y + i.sketchWeight1.z));

                fixed4 finalColor = witheColor + sketchColor0 + sketchColor1 + sketchColor2 + sketchColor3 +
                    sketchColor4 + sketchColor5;

                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos)
                return fixed4(finalColor.rgb * atten * _Color.rgb, 1);
            }
            ENDCG
        }
    }

    Fallback "Diffuse"
}
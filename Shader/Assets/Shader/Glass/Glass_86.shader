Shader "Unlit/Glass_86"
{
    Properties
    {
        //主纹理
        _MainTex("MainTex", 2D) = ""{}
        //立方体纹理
        _Cube("Cube", Cube) = ""{}
        //法线纹理
        _BumpMap("BumpMap", 2D) = ""{}
        // 折射程度 0 表示完全反射 1 表示完全折射
        _RefractAmount("RefractAmount", Range(0, 1)) = 1
        //控制折射扭曲变量
        _Distortion("Distortion", Range(0, 10)) = 5
    }
    SubShader
    {
        //滞后渲染
        Tags
        {
            "RenderType"="Opaque" "Queue"="transparent"
        }

        // 抓取屏幕图像
        GrabPass {}

        Pass
        {
            Tags
            {
                "LightMode"="ForwardBase"
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            samplerCUBE _Cube;
            sampler2D _BumpMap;
            float4 _BumpMap_ST;
            float _RefractAmount;
            float _Distortion;
            //默认抓取纹理
            sampler2D _GrabTexture;

            struct v2f
            {
                float4 pos : SV_POSITION; //裁剪空间下的顶点坐标
                float4 screenPos : TEXCOORD0; //屏幕空间下的顶点坐标（顶点相对于屏幕的位置）
                float4 uv : TEXCOORD1; //在颜色纹理中采样的UV坐标与法线纹理中采样的UV坐标
                //float3 worldReflect : TEXCOORD2; //世界空间下的反射向量
                
                //代表我们切线空间到世界空间的变换矩阵的3行 4的效率更高
                float4 T2W0:TEXCOORD3;
                float4 T2W1:TEXCOORD4;
                float4 T2W2:TEXCOORD5;
            };

            v2f vert(appdata_full v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex); //顶点坐标转换到裁剪空间
                o.screenPos = ComputeScreenPos(o.pos); //计算屏幕空间下的顶点坐标
                o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex); //计算在颜色纹理中采样的UV坐标
                o.uv.zw = TRANSFORM_TEX(v.texcoord, _BumpMap); //计算在法线纹理中采样的UV坐标
                //float3 worldNormal = UnityObjectToWorldNormal(v.normal); //法线转换到世界空间
                //fixed3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz; //顶点坐标转换到世界空间
                //fixed3 worldViewDir = UnityWorldSpaceViewDir(worldPos);
                //视线方向转换到世界空间 内部是用_WorldSpaceCameraPos - worldPos
                //o.worldReflect = reflect(-worldViewDir, worldNormal); //计算反射向量
                
                //得到世界空间下的顶点位置
                float3 woldPos = mul(unity_ObjectToWorld, v.vertex);
                //得到世界空间下的法线位置
                float3 wNormal = UnityObjectToWorldNormal(v.normal);
                //得到世界空间下的切线
                float3 wTangent = UnityObjectToWorldDir(v.tangent);
                //得到世界空间下的副切线
                float3 binormal = cross(wNormal, wTangent) * v.tangent.w;
                //将切线空间下的顶点位置转换到世界空间
                o.T2W0 = float4(wTangent.x, binormal.x, wNormal.x, woldPos.x);
                o.T2W1 = float4(wTangent.y, binormal.y, wNormal.y, woldPos.y);
                o.T2W2 = float4(wTangent.z, binormal.z, wNormal.z, woldPos.z);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                //把切线空间下的法线转换到世界空间下
                float3 wPos = float3(i.T2W0.w, i.T2W1.w, i.T2W2.w);
                fixed3 viewDir = normalize(UnityWorldSpaceViewDir(wPos));
                //通过纹理采样函数 取出法线纹理贴图当中的数据
                float4 packedNormal = tex2D(_BumpMap, i.uv.zw);
                //将我们取出来的法线数据 进行逆运算并且可能会进行解压缩的运算，最终得到切线空间下的法线数据
                float3 tangentNormal = UnpackNormal(packedNormal);
                //将切线空间下的法线转换到世界空间下
                float3 worldNormal = float3(dot(i.T2W0.xyz, tangentNormal), dot(i.T2W1.xyz, tangentNormal), dot(i.T2W2.xyz, tangentNormal));
                
                //计算反射向量
                fixed3 worldReflect = reflect(-viewDir, worldNormal);
                fixed4 reflColor = texCUBE(_Cube, worldReflect) * tex2D(_MainTex, i.uv); //反射光线采样颜色纹理

                //折射光线采样抓取纹理
                //进行采样偏移 避免采样到当前像素
                fixed2 offset = tangentNormal.xy * _Distortion;
                fixed2 screenUV = (i.screenPos.z * offset + i.screenPos.xy) / i.screenPos.w; //将屏幕空间下的顶点坐标转换到[0,1]范围
                fixed4 grabColor = tex2D(_GrabTexture, screenUV); //从抓取纹理中采样颜色

                float4 c = reflColor * (1 - _RefractAmount) + grabColor * _RefractAmount;

                return c;
            }
            ENDCG
        }
    }
}

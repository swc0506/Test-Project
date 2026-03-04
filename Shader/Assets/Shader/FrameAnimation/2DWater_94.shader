Shader "Unlit/2DWater_94"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        //波动幅度
        _WaveAmplitude("WaveAmplitude", Float) = 1
        //波动频率
        _WaveFrequency("WaveFrequency", Float) = 1
        //波长的倒数
        _WaveWavelength("WaveWavelength", Float) = 1

        //纹理变化速度
        _WaveSpeed("WaveSpeed", Float) = 1
    }
    SubShader
    {
        //需要用到模型空间下的顶点坐标 所以要将批处理关闭
        Tags
        {
            "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True" "DisableBatching"="True"
        }

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            //波动幅度
            float _WaveAmplitude;
            //波动频率
            float _WaveFrequency;
            //波长的倒数
            float _WaveWavelength;
            //纹理变化速度
            float _WaveSpeed;

            v2f vert(appdata_base v)
            {
                v2f o;
                //模型空间下的偏移位置
                float4 offset;
                //让它在模型空间的x轴方向进行偏移 由于模型不是unity标准坐标系 所以用z轴进行偏移
                offset.x = _WaveAmplitude * sin(_Time.y * _WaveFrequency + v.vertex.z * _WaveWavelength);
                offset.yzw = 0;
                o.vertex = UnityObjectToClipPos(v.vertex + offset);
                
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.uv += float2(0, _WaveSpeed * _Time.y);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col.rgb *= _Color.rgb;
                return col;
            }
            ENDCG
        }

        Pass
        {
            Tags
            {
                "LightMode" = "ShadowCaster"
            }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            //  该编译指令时告诉Unity编译器生成多个着色器变体
            //  用于支持不同类型的阴影（SM，SSSM等等）
            //  可以确保着色器能够在所有可能的阴影投射模式下正确渲染
            #pragma multi_compile_shadowcaster
            //  其中包含了关键的阴影计算相关的宏
            #include "UnityCG.cginc"
            
            //波动幅度
            float _WaveAmplitude;
            //波动频率
            float _WaveFrequency;
            //波长的倒数
            float _WaveWavelength;

            struct v2f
            {
                //顶点到片元着色器阴影投射结构体数据宏
                //这个宏定义了一些标准的成员变量
                //这些变量用于在阴影投射路径中传递顶点数据到片元着色器
                //我们主要在结构体中使用
                V2F_SHADOW_CASTER;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                //模型空间下的偏移位置
                float4 offset;
                //让它在模型空间的x轴方向进行偏移 由于模型不是unity标准坐标系 所以用z轴进行偏移
                offset.x = _WaveAmplitude * sin(_Time.y * _WaveFrequency + v.vertex.z * _WaveWavelength);
                offset.yzw = 0;
                v.vertex = v.vertex + offset;
                //o.vertex = UnityObjectToClipPos(v.vertex + offset);方法宏会进行计算 所以这里注释掉
                
                //转移阴影投射器法线偏移宏
                //用于在顶点着色器中计算和传递阴影投射所需的变量
                //主要做了
                //2-2-1.将对象空间的顶点位置转换为裁剪空间的位置
                //2-2-2.考虑法线偏移，以减轻阴影失真问题，尤其是在处理自阴影时
                //2-2-3.传递顶点的投影空间位置，用于后续的阴影计算
                //我们主要在顶点着色器中使用
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o);
                return o;
            }

            float4 frag(v2f i):SV_Target
            {
                //阴影投射片元宏
                //将深度值写入到阴影映射纹理中
                //我们主要在片元着色器中使用
                SHADOW_CASTER_FRAGMENT(i);
            }
            ENDCG
        }
    }
}
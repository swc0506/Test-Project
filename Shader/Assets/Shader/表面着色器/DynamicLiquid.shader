Shader "Custom/DynamicLiquid"
{
    Properties
    {
        //液体颜色
        _Color("Color", Color) = (1,1,1,1)
        //高光颜色 和 光滑度
        _Specular("Specular", Color) = (1,1,1,1)
        //液体高度
        _Height("Height", Float) = 0
        
        //波纹变化速度
        _Speed("Speed", Float) = 1.0
        //波动幅度
        _WaveAmplitude("Wave Amplitude", Float) = 0
        //波动频率
        _WaveFrequency("Wave Frequency", Float) = 0
        //波长的倒数
        _WaveLength("Wave Length", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
        Blend DstColor SrcColor
        ZWrite Off

        CGPROGRAM
        #pragma surface surf StandardSpecular noshadow
        #pragma target 3.0
        
        fixed4 _Color;
        fixed4 _Specular;
        float _Height;
        
        float _Speed;
        float _WaveAmplitude;
        float _WaveFrequency;
        float _WaveLength;
        
        struct Input
        {
            // 世界坐标
            float3 worldPos;
        };

        void surf (Input IN, inout SurfaceOutputStandardSpecular o)
        {
            //将模型空间下的中心点转到世界空间下
            float3 centerPoint = mul(unity_ObjectToWorld, float4(0, 0, 0, 1));
            //当前像素点和中心点的高度差
            float liquidHeight = centerPoint.y - IN.worldPos.y + _Height * 0.01;
            
            // 波纹变化
            float wave = sin(_Time.y * _Speed * _WaveFrequency + IN.worldPos.x * _WaveLength) * _WaveAmplitude;
            liquidHeight += wave;
            
            // liquidHeight >= 0 则返回1 否则返回0
            liquidHeight = step(0, liquidHeight);
            // 剔除
            clip(liquidHeight - 0.001);
            
            //漫反射
            o.Albedo = _Color.rgb;
            //高光
            o.Specular = _Specular.rgb;
            //光滑度
            o.Smoothness = _Specular.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}

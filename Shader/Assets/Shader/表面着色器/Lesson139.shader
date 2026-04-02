Shader "Custom/Lesson139"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _BumpMap ("Normal Map", 2D) = "" {}
        
        _Emission("Emission", Color) = (1,1,1,1)
        _Metallic("Metallic", Range(0,1)) = 0.0
        _Smoothness("Smoothness", Range(0,1)) = 0.0
        
        _Expansion("Expansion", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        CGPROGRAM
        
        #pragma surface surf Standard fullforwardshadows vertex:vert finalcolor:finalColor
        
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _BumpMap;
        fixed4 _Color;
        fixed4 _Emission;
        fixed _Metallic;
        fixed _Smoothness;
        float _Expansion;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 texColor = tex2D(_MainTex, IN.uv_MainTex);
            //漫反射
            o.Albedo = texColor.rgb;
            //透明通道相关
            o.Alpha = texColor.a * _Color.a;
            //切线空间下的法线
            o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
            
            //o.Emission = _Emission.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;
        }
        
        void vert(inout appdata_full v)
        {
            //修改顶点坐标 往外扩充
            v.vertex.xyz += v.normal * _Expansion;
        }
        
        void finalColor(Input IN, SurfaceOutputStandard o, inout fixed4 color)
        {
            color.rgb = color * _Color.rgb;
        }
        
        ENDCG
    }
    FallBack "Diffuse"
}

Shader "Unlit/PageTurning_119"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BackTex ("Back Texture", 2D) = "white" {}
        // 翻页进度 0-180
        _AngleProgress("Angle Progress", Range(0, 180)) = 0
        _WeightX("WeightX", Range(0, 1)) = 0
        _WeightY("WeightY", Range(0, 1)) = 0
        _WaveLength("Wave Length", Range(0, 3)) = 0
        _MoveDis("Move Dis", Float) = 0
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _BackTex;
            float _AngleProgress;
            //X轴收缩权重
            float _WeightX;
            //Y轴收缩权重
            float _WeightY;
            //波长
            float _WaveLength;
            //平移距离
            float _MoveDis;

            v2f vert(appdata_base v)
            {
                v2f o;
                //对顶点进行变换
                //先处理旋转
                float s;
                float c;
                sincos(radians(_AngleProgress), s, c);
                //旋转矩阵 z轴
                float4x4 rotationM = float4x4(c, -s, 0, 0,
                                             s, c, 0, 0,
                                             0, 0, 1, 0,
                                             0, 0, 0, 1);
                
                //由于我们是基于Z轴旋转 所以我们将其按照X轴方向进行偏移
                v.vertex += float4(_MoveDis, 0, 0, 0);
                
                //进行起伏
                float weight = 1 - abs(90 - _AngleProgress)/90;
                //Y轴上下起伏
                v.vertex.y += sin(v.vertex.x * _WaveLength) * weight * _WeightY;
                //X轴收缩
                v.vertex.x -= v.vertex.x * weight * _WeightX;
                
                //平移后 再旋转
                float4 positon = mul(rotationM, v.vertex);
                //在平移回来
                positon -= float4(_MoveDis, 0, 0, 0);
                
                o.vertex = UnityObjectToClipPos(positon);
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag(v2f i, fixed face : VFACE) : SV_Target
            {
                fixed4 col = face > 0 ? tex2D(_MainTex, i.uv) : tex2D(_BackTex, i.uv);

                return col;
            }
            ENDCG
        }
    }
}
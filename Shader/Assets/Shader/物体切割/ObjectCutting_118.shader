Shader "Unlit/ObjectCutting_118"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BackTex ("Back Texture" , 2D) = "white" {}
        // 切割方向 0为x轴 1为y轴 2为z轴
        _CuttingDir ("Cutting Direction" , Float) = 0
        // 是否切割翻转 0为不翻转 1为翻转
        _Invert ("Invert" , Float) = 0
        // 切割位置
        _CuttingPos ("Cutting Position" , Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
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
                //顶点在世界坐标位置
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            sampler2D _BackTex;
            float _CuttingDir;
            float _Invert;
            float4 _CuttingPos;

            v2f vert (appdata_base v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            fixed4 frag (v2f i, fixed face:VFACE) : SV_Target
            {
                fixed4 col = face > 0 ? tex2D(_MainTex, i.uv) : tex2D(_BackTex, i.uv);
                //丢弃中间值
                fixed cutValue = 0;
                //比较x坐标
                if (_CuttingDir == 0)
                    cutValue = step(_CuttingPos.x, i.worldPos.x);
                else if (_CuttingDir == 1)//比较y坐标
                    cutValue = step(_CuttingPos.y, i.worldPos.y);
                else if (_CuttingDir == 2)//比较z坐标
                    cutValue = step(_CuttingPos.z, i.worldPos.z);
                
                cutValue = _Invert ? 1 - cutValue : cutValue;
                if (cutValue == 0)// x<edge 未0 丢弃
                    clip(-1);
                return col;
            }
            ENDCG
        }
    }
}

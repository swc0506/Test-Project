Shader "Unlit/KeyWord_129"
{
    Properties
    {
        [Header(Pic)]_MainTex ("Texture", 2D) = "white" {}
        //[Toggle] 默认会有一个关键字 _SHOWTEX_ON 声明对应的规则的关键字 或者[Toggle(自定义名字)]
        [Toggle]_ShowTex("ShowTex", Float) = 1
        [Enum(Tex, 0, Red, 1, Green, 2, Blue, 3)]_Text_Enum("Text_Enum", Float) = 1
        //默认存在关键字
        //_KETWORDENUM_TEX
        //_KETWORDENUM_RED
        //_KETWORDENUM_GREEN
        //_KETWORDENUM_BLUE
        [KeywordEnum(Tex, Red, Green, Blue)]_KeywordEnum("Text_KeywordEnum", Float) = 1
        [PowerSlider(2)]_PowerSlider("PowerSlider", Range(0, 100)) = 1
        [Space(10)][IntRange]_IntRange("IntRange", Range(0, 100)) = 1
        [HideInInspector]_HideInInspector("HideInInspector", Float) = 1
        [NoScaleOffset]_NoScaleOffset("NoScaleOffset", 2D) = "white" {}
        [Normal]_Normal("Normal", 2D) = "white" {}
        //可以使数值突破1
        [HDR]_HDR_Color("HDRColor", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            
            //只有关键词启用时才会生成对应变体
            //#pragma shader_feature _TEST_KEYWORD_1 _TEST_KEYWORD_2
            #pragma shader_feature _SHOWTEX_ON _KETWORDENUM_TEX _KETWORDENUM_RED _KETWORDENUM_GREEN _KETWORDENUM_BLUE
            //不管是否启用时都会生成对应变体
            //#pragma multi_compile _TEST_KEYWORD_1 _TEST_KEYWORD_2

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Text_Enum;
            float _PowerSlider;
            float _IntRange;
            float _HideInInspector;
            sampler2D _NoScaleOffset;
            sampler2D _Normal;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = fixed4(1,1,1,1);
                
                // #if defined(_TEST_KEYWORD_1)
                // col = tex2D(_MainTex, i.uv);
                // #endif
                //
                // #if defined(_TEST_KEYWORD_2)
                // UNITY_APPLY_FOG(i.fogCoord, col);
                // #endif
                
                #if defined(_SHOWTEX_ON)
                col = tex2D(_MainTex, i.uv);
                #else
                col = fixed4(1,1,1,1);
                #endif
                
                return col;
            }
            ENDCG
        }
    }
}

Shader "Unlit/Lesson3"
{
    Properties
    {
        _My2D("My2D", 2D) = "white" {}
        _MyCube("MyCube", Cube) = ""{}
        _2DArray("2DArray", 2DArray) = ""{}
        _My3D("My3D", 3D) = ""{}
        
        _MyInt("MyInt", Int) = 1
        _MyFloat("MyFloat", Float) = 0.5
        _MyRange("Range", Range(1,5)) = 2
        
        _MyColor("MyColor", Color) = (0.5,0.5,0.5,1)
        _MyVector("MyVector",Vector) = (0.9,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque"
            //"RenderType"="Transparent"
            //"RenderType"="TransparentCutout"
            //"RenderType"="Background"
            //"RenderType"="Overlay"
            
            "Queue"="Geometry+1"
            //"Queue"="Background"
            //"Queue"="AlphaTest"
            //"Queue"="Transparent"
            //"Queue"="Overlay"
            
            //"DisableBatching"="True"
            //"DisableBatching"="LODFading"
            
            //"ForceNoShadowCasting"="True"//不投射阴影
            
            //"IgnoreProjector="True"//忽略Projector（投影机）
            
            //“CanUseSpriteAtlas"="False"//想要将SubShader用于Sprite时，设置为False
            //“PreviewType"=”Panel“//预览平面展示，默认球形
            //"PreviewType"="SkyBox"//预览天空盒展示
        }
        
        LOD 100
        //渲染剔除
        //Cull Back
        //Cull Front
        //Cull Off//不剔除
        //深度缓冲
        //ZWrite On 写入深度缓冲
        //ZWrite Off 不写入深度缓冲
        //一般情况下，透明效果不写入，默认写入
        //深度测试对比方式
        //ZTest LEqual //透明物体less，描边Greater

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

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
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}

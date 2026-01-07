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
    SubShader //会使用第一个能正常使用的SubShader
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
        
        //控制LOD级别，在不同距离下使用不同的渲染方式处理
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
        //混合方式 渲染图像的混合方式 默认不混合
        //Blend One One //线性减淡
        //设置颜色通道的写入蒙版，默认为RGBA
        //ColorMask
        //渲染状态可以写在PASS中，只影响Pass通道

        //使用其他Shader的Pass
        //UsePass "Unlit/Lesson3/MYLESSONPASS1"
        
        //利用GraPass命名把即将绘制对象时的屏幕内容抓取到纹理中
        //在后续通道中即可使用此纹理，从而执行基于图像的高级效果
        //GrabPass
        //{
            //"_BackgroundTexture"
        //}
        
        Pass
        {
            //1.名字
            Name "MyLessonPass1"
            
            //2.渲染标签 Pass有专门的渲染标签
            //Tags{"LightMode" = "标签值"} //指定了该Pass在哪个阶段执行
            //Tags{"RequireOptions" = "标签值"} //用于指定当满足某些条件时才渲染该Pass 比如SoftVegetation
            //Tags{"PassFlags" = "标签值"} //用于改变渲染管线向Pass传递数据的方式 比如OnlyDirectional
            
            //3.渲染状态 SubShader的渲染状态适用于Pass
            
            //4.着色器代码相关
            
            CGPROGRAM
            #pragma vertex vert //顶点着色器
            #pragma fragment frag //片元着色器
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;//语义：顶点位置信息
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
                o.vertex = UnityObjectToClipPos(v.vertex);//得到裁剪空间坐标
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);

                //float3 a = float3(0.5, 0.0, 1.0);
                //float3 b = float3(0.6, -0.1, 0.9);
                //bool3 c = a < b;
                //运算后结果为c = bool3(true, false, false) CG逻辑中不存在短路
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target//把用户输出的颜色存储到一个渲染项目中
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
    Fallback "VertexLit" //备用Shader名
    //Fallback Off //关闭
}

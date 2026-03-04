Shader "Unlit/Billboard_95"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        //用于控制垂直与全向广告牌
        _VerticalBillboard("Vertical Billboard", range(0,1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent"  "Queue"="Transparent"  "IgnoreProjector"="True" "DisableBatching"="True" }//需要用的模型空间

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off//双面渲染
            
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
            float4 _Color;
            float _VerticalBillboard;

            v2f vert (appdata_base v)
            {
                v2f o;
                //确定新坐标中心点
                float3 center = float3(0,0,0);//默认原点
                
                //计算Z轴（normal）
                float3 cameraInObjectPos = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos,1));//相机在模型空间中的位置
                float3 normalDir = cameraInObjectPos - center;
                normalDir.y *= _VerticalBillboard;//根据VerticalBillboard控制垂直与全向广告牌
                normalDir = normalize(normalDir);//归一化
                //为了避免Z轴与010重合，导致无法看到广告牌，所以需要判断z轴与相机是否重合
                float3 oldUp = normalDir.y > 0.999 ? float3(0,0,1) : float3(0,1,0);//模型空间下的y轴正方向位oldUp
                
                //计算XY轴（tangent）
                float3 tangentDir = normalize(cross(oldUp, normalDir));//计算出X轴（tangent）
                float3 bitangentDir = normalize(cross(normalDir, tangentDir));//计算Y轴（bitangent）
                
                //计算顶点位置
                float3 offsetPos = v.vertex.xyz - center;//得到顶点相对于新坐标系中心点的偏移位置
                float3 vertexPos = center + tangentDir * offsetPos.x + bitangentDir * offsetPos.y + normalDir * offsetPos.z;
                o.vertex = UnityObjectToClipPos(float4(vertexPos,1));//将模型空间下的顶点位置转换到裁剪空间
                //计算UV
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col.rgb *= _Color.rgb;
                return col;
            }
            ENDCG
        }
    }
}

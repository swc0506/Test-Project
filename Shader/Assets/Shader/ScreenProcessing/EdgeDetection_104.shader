Shader "Unlit/EdgeDetection_104"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
        // 边缘颜色
        _EdgeColor ("Edge Color", Color) = (0, 0, 0, 1)
        _BackExtent("Back Extent", Range(0, 1)) = 0
        _BackColor("Back Color", Color) = (0, 0, 0, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            Ztest Always
            Cull Off
            Zwrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            
            struct v2f
            {
                // 9个纹理坐标
                half2 uv[9] : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            half4 _MainTex_TexelSize;// Unity内置纹素变量
            float4 _EdgeColor;
            float _BackExtent;
            float4 _BackColor;

            v2f vert (appdata_base v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                // 计算9个纹理坐标
                o.uv[0] = v.texcoord.xy + _MainTex_TexelSize.xy * half2(-1, -1);
                o.uv[1] = v.texcoord.xy + _MainTex_TexelSize.xy * half2(0, -1);
                o.uv[2] = v.texcoord.xy + _MainTex_TexelSize.xy * half2(1, -1);
                o.uv[3] = v.texcoord.xy + _MainTex_TexelSize.xy * half2(-1, 0);
                o.uv[4] = v.texcoord.xy;
                o.uv[5] = v.texcoord.xy + _MainTex_TexelSize.xy * half2(1, 0);
                o.uv[6] = v.texcoord.xy + _MainTex_TexelSize.xy * half2(-1, 1);
                o.uv[7] = v.texcoord.xy + _MainTex_TexelSize.xy * half2(0, 1);
                o.uv[8] = v.texcoord.xy + _MainTex_TexelSize.xy * half2(1, 1);
                return o;
            }
            
            //计算灰度值
            fixed calcLuminance(fixed4 color)
            {
                return 0.2126 * color.r + 0.7152 * color.g + 0.0722 * color.b;
            }
            
            // sobel算子
            half sobel(v2f i)
            {
                half Gx[9] = {
                    -1, -2, -1,
                    0, 0, 0,
                    1, 2, 1
                };
                
                half Gy[9] = {
                    -1, 0, 1,
                    -2, 0, 2,
                    -1, 0, 1
                };
                
                fixed4 color;
                half Gx_sum = 0;
                half Gy_sum = 0;
                for (int j = 0; j < 9; j++)
                {
                    color = tex2D(_MainTex, i.uv[j]);
                    Gx_sum += Gx[j] * calcLuminance(color);
                    Gy_sum += Gy[j] * calcLuminance(color);
                }
                
                return sqrt(Gx_sum * Gx_sum + Gy_sum * Gy_sum);// 边缘检测
                return abs(Gx_sum) + abs(Gy_sum);// 边缘检测
            }

            fixed4 frag (v2f i) : SV_Target
            {
                half edge = sobel(i);
                fixed4 col = lerp(tex2D(_MainTex, i.uv[4]), _EdgeColor, edge);//利用梯度值进行插
                
                //纯色上描边
                fixed4 onlyEdge = lerp(_BackColor, _EdgeColor, edge);
                
                return lerp(col, onlyEdge, _BackExtent);
            }
            ENDCG
        }
    }
}

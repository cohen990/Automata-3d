Shader "Terrain/MultiTextureCubic"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD2;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = v.normal;
                o.uv = v.uv;
                o.uv2 = v.uv2;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                vector<half, 2> sub_uv = i.uv;
                const int width = 6;
                const int height = 4;
                const int unit_width = 3;
                const int unit_height = 4;
                const float threshold = 0.9;

                const int x_offset = round(unit_width * i.uv2.x);
                const int y_offset = round(unit_height * i.uv2.y);
                
                if(dot(i.normal, vector<float, 3>(0, 1, 0)) >= threshold)
                {
                    sub_uv = vector<half, 2>((i.uv.x + 1 + x_offset) / width, (i.uv.y + 2 + y_offset) / height);
                }
                else if(dot(i.normal, vector<float, 3>(0, -1, 0)) >= threshold)
                {
                    sub_uv = vector<half, 2>((i.uv.x + 1 + x_offset) / width, (i.uv.y + y_offset) / height);
                }
                else if(dot(i.normal, vector<float, 3>(1, 0, 0)) >= threshold)
                {
                    sub_uv = vector<half, 2>((i.uv.x + 1 + x_offset) / width, (i.uv.y + 3 + y_offset) / height);
                }
                else if(dot(i.normal, vector<float, 3>(-1, 0, 0)) >= threshold)
                {
                    sub_uv = vector<half, 2>((i.uv.x + 1 + x_offset) / width, (i.uv.y + 1 + y_offset) / height);
                }
                else if(dot(i.normal, vector<float, 3>(0, 0, 1)) >= threshold)
                {
                    sub_uv = vector<half, 2>((i.uv.x + 2 + x_offset) / width, (i.uv.y + 2 + y_offset) / height);
                }
                else if(dot(i.normal, vector<float, 3>(0, 0, -1)) >= threshold)
                {
                    sub_uv = vector<half, 2>((i.uv.x + x_offset) / width, (i.uv.y + 2 + y_offset) / height);
                }
                
                fixed4 col = tex2D(_MainTex, sub_uv);
                return col;
            }
            ENDHLSL
        }
    }
}

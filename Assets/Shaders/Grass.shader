Shader "Terrain/Grass"
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
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = v.normal;
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                vector<half, 2> sub_uv = i.uv;
                float threshold = 0.9999999;
                if(dot(i.normal, vector<float, 3>(0, 1, 0)) >= threshold)
                {
                    sub_uv = vector<half, 2>((i.uv.x + 1) / 3, i.uv.y / 4);
                }
                else if(dot(i.normal, vector<float, 3>(0, -1, 0)) >= threshold)
                {
                    sub_uv = vector<half, 2>((i.uv.x + 1) / 3, (i.uv.y + 2) / 4);
                }
                else if(dot(i.normal, vector<float, 3>(1, 0, 0)) >= threshold)
                {
                    sub_uv = vector<half, 2>((i.uv.x + 1) / 3, (i.uv.y + 3) / 4);
                }
                else if(dot(i.normal, vector<float, 3>(-1, 0, 0)) >= threshold)
                {
                    sub_uv = vector<half, 2>((i.uv.x + 1) / 3, (i.uv.y + 1) / 4);
                }
                else if(dot(i.normal, vector<float, 3>(0, 0, 1)) >= threshold)
                {
                    sub_uv = vector<half, 2>((i.uv.x + 2) / 3, (i.uv.y + 2) / 4);
                }
                else if(dot(i.normal, vector<float, 3>(0, 0, -1)) >= threshold)
                {
                    sub_uv = vector<half, 2>(i.uv.x / 3, (i.uv.y + 2) / 4);
                }
                
                fixed4 col = tex2D(_MainTex, sub_uv);
                return col;
            }
            ENDHLSL
        }
    }
}

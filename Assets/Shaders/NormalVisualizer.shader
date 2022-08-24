Shader "Debug/NormalVisualizer"
{
    Properties
    {
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
                const float x_dot = dot(i.normal, vector<float, 3>(1, 0, 0));
                const float y_dot = dot(i.normal, vector<float, 3>(0, 1, 0));
                const float z_dot = dot(i.normal, vector<float, 3>(0, 0, 1));
                fixed4 col = {x_dot, y_dot, z_dot, 1};
                
                return col;
            }
            ENDHLSL
        }
    }
}

Shader "Debug/NormalVisualizer2"
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
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = v.normal;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float threshold = 0.9999999;
                if(dot(i.normal, vector<float, 3>(0, 1, 0)) >= threshold)
                {
                    fixed4 col = {1, 0, 0, 1};
                    return col;
                }
                if(dot(i.normal, vector<float, 3>(0, -1, 0)) >= threshold)
                {
                    fixed4 col = {0, 1, 0, 1};
                    return col;
                }
                if(dot(i.normal, vector<float, 3>(1, 0, 0)) >= threshold)
                {
                    fixed4 col = {0, 0, 1, 1};
                    return col;
                }
                if(dot(i.normal, vector<float, 3>(-1, 0, 0)) >= threshold)
                {
                    fixed4 col = {1, 0, 1, 1};
                    return col;
                }
                if(dot(i.normal, vector<float, 3>(0, 0, 1)) >= threshold)
                {
                    fixed4 col = {0, 1, 1, 1};
                    return col;
                }
                if(dot(i.normal, vector<float, 3>(0, 0, -1)) >= threshold)
                {
                    fixed4 col = {1, 1, 1, 1};
                    return col;
                }
                fixed4 black = {0, 0, 0, 1};
                return black;
            }
            ENDHLSL
        }
    }
}

Shader "Terrain/MultiTextureCubicSurface"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _MainTex2 ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex: TEXCOORD0;
            float2 uv2_MainTex2 : TEXCOORD2;
        };

        half _Glossiness;
        half _Metallic;
        float4 _Color;

        
        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input i, inout SurfaceOutputStandard o)
        {
            const int width = 6;
            const int height = 4;
            const int unit_width = 3;
            const int unit_height = 4;
            const float threshold = 0.9;

            const float x_offset = round(unit_width * i.uv2_MainTex2.x);
            const float y_offset = round(unit_height * i.uv2_MainTex2.y);
            if(dot(o.Normal, vector<float, 3>(0, 1, 0)) >= threshold)
            {
                i.uv_MainTex = float2((i.uv_MainTex.x + 1 + x_offset) / width, (i.uv_MainTex.y + 2 + y_offset) / height);
            }
            else if(dot(o.Normal, vector<float, 3>(0, -1, 0)) >= threshold)
            {
                i.uv_MainTex = float2((i.uv_MainTex.x + 1 + x_offset) / width, (i.uv_MainTex.y + y_offset) / height);
            }
            else if(dot(o.Normal, vector<float, 3>(1, 0, 0)) >= threshold)
            {
                i.uv_MainTex = float2((i.uv_MainTex.x + 1 + x_offset) / width, (i.uv_MainTex.y + 3 + y_offset) / height);
            }
            else if(dot(o.Normal, vector<float, 3>(-1, 0, 0)) >= threshold)
            {
                i.uv_MainTex = float2((i.uv_MainTex.x + 1 + x_offset) / width, (i.uv_MainTex.y + 1 + y_offset) / height);
            }
            else if(dot(o.Normal, vector<float, 3>(0, 0, 1)) >= threshold)
            {
                i.uv_MainTex = float2((i.uv_MainTex.x + 2 + x_offset) / width, (i.uv_MainTex.y + 2 + y_offset) / height);
            }
            else if(dot(o.Normal, vector<float, 3>(0, 0, -1)) >= threshold)
            {
                i.uv_MainTex = float2((i.uv_MainTex.x + x_offset) / width, (i.uv_MainTex.y + 2 + y_offset) / height);
            }
            else
            {
                i.uv_MainTex = float2(0, 0);
            }

            float4 c = tex2D (_MainTex, i.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}

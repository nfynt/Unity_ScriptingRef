Shader "Nfynt/HeatmapLitShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _HeatTex("Heatmap", 2D) = "white" {}
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
            //float4 color : COLOR;
            float2 uv_MainTex;
            //float2 uv_BumpMap;
            //float3 viewDir;
            float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        //Heatmap props
        uniform int _Points_Length = 0;
        uniform float4 _Points[100]; // (x, y, z) = position
        uniform float4 _Properties[100]; // x = radius, y = intensity

        sampler2D _HeatTex;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            half h = 0;

            for (int i = 0; i < _Points_Length; i++)
            {
                // Calculates the contribution of each point
                float3 posxz = IN.worldPos.xyz;
                posxz.y = 0;
                half di = distance(posxz, _Points[i].xyz);

                half ri = _Properties[i].x;
                half hi = 1 - saturate(di / ri);

                h += hi * _Properties[i].y;
            }

            // Converts (0-1) according to the heat texture
            h = saturate(h);

            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            if (h > 0.2)
            {
                //if (h > 1.0) h = 0.9;
                half4 color = tex2D(_HeatTex, fixed2(h, 0.5));
                o.Albedo = color.rgb;
            }
            else
            {
                o.Albedo = c.rgb;
            }
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}

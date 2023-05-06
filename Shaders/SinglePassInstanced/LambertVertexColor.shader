Shader "Nfynt/LambertVertexColor"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
    }
        SubShader
    {
        //Using forward rendering pipeline, which gets ambient and main directional
        // light data set up; light direction in _WorldSpaceLightPos0 and color in _LightColor0
        Tags { "RenderType" = "Opaque" "LightMode" = "ForwardBase"}
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma instancing_options

            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc" // for _LightColor0

            struct appdata
            {
                float4 vertex : POSITION;
                float4 normal: NORMAL;
                float4 uv : TEXCOORD0;
                fixed4 color : COLOR;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;   //vertex color
                fixed4 diff : COLOR1; // diffuse lighting color

                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert(appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;

                //Diffuse (Lambertian) + ambient lighting 
                
                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                // factor in the light color
                o.diff = nl * _LightColor0;

                // add illumination from ambient or light probes
                // ShadeSH9 function from UnityCG.cginc evaluates it, using world space normal
                o.diff.rgb += ShadeSH9(half4(worldNormal, 1));

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col.rgb *= i.color.rgb;
                // multiply by lighting
                col *= i.diff;
                //Color space conversion for linear setup
                return half4(GammaToLinearSpace(col.rgb),1);
            }
            ENDCG
        }
    }
}

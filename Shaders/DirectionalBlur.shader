Shader "Nfynt/DirectionalBlur"
{ 
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Radius ("Blur Radius", float) = 15 
        hstep("HorizontalStep", Range(0,1)) = 1.0
        vstep("VerticalStep", Range(0,1)) = 0.0
    }

    SubShader
    {
        Tags {"RenderType"="Opaque"}
        ZWrite Off Cull Off
        Pass
        {    
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex   : POSITION;
                float2 uv : TEXCOORD0;
            };    
            struct v2f
            {
                half2 uv  : TEXCOORD0;
                float4 vertex   : SV_POSITION;
            };

            sampler2D _MainTex;
            float _Radius;
            static const float resolution=800;

            //the direction of our blur in range [0.0 - 1.0]
            float hstep;
            float vstep;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {    
                float2 uv = i.uv;
                float4 sum = float4(0.0, 0.0, 0.0, 0.0);
                float2 tc = uv;

                float blur = _Radius / resolution / 4;

                //blur using a 9-tap filter with predefined gaussian weights
                //weights ref: https://github.com/mattdesl/lwjgl-basics/wiki/ShaderLesson5

                sum += tex2D(_MainTex, float2(tc.x - 4.0*blur*hstep, tc.y - 4.0*blur*vstep)) * 0.0162162162;
                sum += tex2D(_MainTex, float2(tc.x - 3.0*blur*hstep, tc.y - 3.0*blur*vstep)) * 0.0540540541;
                sum += tex2D(_MainTex, float2(tc.x - 2.0*blur*hstep, tc.y - 2.0*blur*vstep)) * 0.1216216216;
                sum += tex2D(_MainTex, float2(tc.x - 1.0*blur*hstep, tc.y - 1.0*blur*vstep)) * 0.1945945946;

                sum += tex2D(_MainTex, float2(tc.x, tc.y)) * 0.2270270270;

                sum += tex2D(_MainTex, float2(tc.x + 1.0*blur*hstep, tc.y + 1.0*blur*vstep)) * 0.1945945946;
                sum += tex2D(_MainTex, float2(tc.x + 2.0*blur*hstep, tc.y + 2.0*blur*vstep)) * 0.1216216216;
                sum += tex2D(_MainTex, float2(tc.x + 3.0*blur*hstep, tc.y + 3.0*blur*vstep)) * 0.0540540541;
                sum += tex2D(_MainTex, float2(tc.x + 4.0*blur*hstep, tc.y + 4.0*blur*vstep)) * 0.0162162162;
                return float4(sum.rgb, 1);
            }    
            ENDCG
        }
    }
}

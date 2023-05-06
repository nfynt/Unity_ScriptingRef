Shader "Nfynt/UnlitTintShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TintColor("Tint Color", COLOR) = (1,1,1,1)
        [Toggle] _IsTransparent("Transparent", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            //#pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma instancing_options

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;

                //add instanceID param defined inside 'INSTANCING_ON' macro block
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                //UNITY_FOG_COORDS(1)

                UNITY_VERTEX_INPUT_INSTANCE_ID  // use this to access instanced properties in the fragment shader.
                UNITY_VERTEX_OUTPUT_STEREO      // for unity_StereoEyeIndex param to differentiate betweel left and right eye
            };

            //UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex);
            sampler2D _MainTex;
            float4 _MainTex_ST;
            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _TintColor)
                UNITY_DEFINE_INSTANCED_PROP(float, _IsTransparent)
            UNITY_INSTANCING_BUFFER_END(Props)

            v2f vert (appdata v)
            {
                v2f o;
                
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);   //tells the GPU which eye in the texture array it should render to, based on the value of unity_StereoEyeIndex

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                // Only required if using unity_StereoEyeIndex built-in shader variable to find out which eye the GPU is rendering to. This is useful when testing post processing effects (lerp(_LeftEyeColor, _RightEyeColor, unity_StereoEyeIndex)).
                //UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                //half4 col = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv);

                fixed4 col = tex2D(_MainTex, i.uv);

                col.rgb *= UNITY_ACCESS_INSTANCED_PROP(Props, _TintColor).rgb;

                return half4(col.rgb, UNITY_ACCESS_INSTANCED_PROP(Props, _IsTransparent) > 0 ? col.a : 1);
            }
            ENDCG
        }
    }
}

Shader "Nfynt/DiffuseCoordinates"
{
	Properties
	{
		[NoScaleOffset] _MainTex("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _CamDir ("Camera Direction", Vector) = (0,0,-1,0)
	}
	SubShader
	{
		Pass
		{
			// indicate that our pass is the "base" pass in forward
			// rendering pipeline. It gets ambient and main directional
			// light data set up; light direction in _WorldSpaceLightPos0
			// and color in _LightColor0
			Tags {"LightMode" = "ForwardBase"}

			CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc" // for UnityObjectToWorldNormal
            #include "UnityLightingCommon.cginc" // for _LightColor0

            struct v2f
            {
                float2 uv : TEXCOORD0;
                fixed4 diff : COLOR0; // diffuse lighting color
                float4 vertex : SV_POSITION;
            };

        fixed4 _CamDir;

            v2f vert(appdata_base v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                // get vertex normal in world space
                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                // dot product between normal and light direction for
                // standard diffuse (Lambert) lighting
               // half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                half nl = max(0, dot(worldNormal, _CamDir.xyz));
                // factor in the light color
                o.diff = nl;// *_LightColor0;
                return o;
            }

            sampler2D _MainTex;
            fixed4 _Color;

            fixed4 frag(v2f i) : SV_Target
            {
                // sample texture
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
            // multiply by lighting
            col *= i.diff;
            return col;
        }
        ENDCG
		}
	}
}
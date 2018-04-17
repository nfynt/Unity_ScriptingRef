Shader "Shubham/ImageEffects/Screen"
{
	Properties
	{
		_MainTex("Main Texture",2D) = "white"{}
	}

	SubShader{

		//No culling of depth
		cull off ZWrite off ZTest Always

		Pass{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex:POSITION;
				float2 uv:TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex:SV_POSITION;
				float2 uv:TEXCOORD0;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D _MainTex;

			float4 frag(v2f v) : SV_Target
			{
				float4 color = tex2D(_MainTex,v.uv);
				//color *= float4(v.uv.x,v.uv.y,0,0.5f);
				return color;
			}


			ENDCG
		}
	}
}
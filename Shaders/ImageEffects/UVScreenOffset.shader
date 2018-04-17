Shader "Shubham/ImageEffects/UVScreenOffset"
{
	Properties
	{
		_MainTex("Main Texture",2D) = "white"{}
		_DisplaceTex("Displacement Texture",2D) = "white"{}
		_Magnitude("Magnitude",Range(0,1))=0.1
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
			sampler2D _DisplaceTex;
			float _Magnitude;

			float4 frag(v2f v) : SV_Target
			{
				float2 disp = tex2D(_DisplaceTex,v.uv).xy;
				disp = ((disp*float2(2,2)) - 1)*_Magnitude;

				float4 color = tex2D(_MainTex,v.uv + disp);
				//color *= float4(v.uv.x,v.uv.y,0,0.5f);
				return color;
			}


			ENDCG
		}
	}
}
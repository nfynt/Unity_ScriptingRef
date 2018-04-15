// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Shubham/Basic/ColorSpectrum"
{
	Properties{
		_BlueHue("Blue Hue",Range(0,1)) = 0
	}

	SubShader{
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

			float _BlueHue;

			float4 frag(v2f v) : SV_Target
			{
				float4 color = float4(v.uv.r,v.uv.g,_BlueHue,1);
				return color;
			}


			ENDCG
		}
	}
}
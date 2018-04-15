Shader "Shubham/Basic/SimpleTexture"
{
	Properties
	{
		_MainTex("Main Texture",2D) = "white" {}
		_TintColor("Tint Color",Color) = (1,1,1,1)
		_UseUVTint("Use uv for tint",Range(0,1)) = 0
		_BlueHue("Blue Hue",Range(0,1)) = 0
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
		}

		Pass
		{

		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM

		#pragma vertex vert
		#pragma fragment frag

		#include "UnityCG.cginc"

		struct appdata
		{
			float4 vertex : POSITION;
			float4 uv : TEXCOORD0;
		};

		struct v2f
		{
			float4 vertex : SV_POSITION;
			float4 uv : TEXCOORD0;
		};

		v2f vert(appdata v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.uv = v.uv;
			return o;
		}

		sampler2D _MainTex;
		float4 _TintColor;
		int _UseUVTint;
		float _BlueHue;

		float4 frag(v2f v) : SV_Target
		{
			float4 color = tex2D(_MainTex,v.uv);

			if(_UseUVTint > 0)
				color *= float4(_BlueHue,v.uv.r,v.uv.g,1);
			else
				color *= _TintColor;

			return color;
		}


		ENDCG
		}
	}
}
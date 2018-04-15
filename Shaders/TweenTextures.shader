Shader "Shubham/Basic/TweenBetweenTexture"
{
	Properties
	{
		_TintColor("Tint Color",Color) = (1,1,1,1)
		_MainTex("Main Texture",2D) = "white" {}
		_SecondTex("Second Texture",2D) = "white" {}
		_TweenValue("Tween to secondary",Range(0,1)) = 0
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

		float4 _MainTex_ST;

		v2f vert(appdata v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.uv = v.uv * _MainTex_ST;
			return o;
		}

		sampler2D _MainTex;
		float4 _TintColor;
		sampler2D _SecondTex;
		float _TweenValue;

		float4 frag(v2f v) : SV_Target
		{
			float4 color1 = tex2D(_MainTex,v.uv);
			float4 color2 = tex2D(_SecondTex,v.uv);

			float4 color = color1*(1-_TweenValue) + color2*(_TweenValue);


			return color;
		}


		ENDCG
		}
	}
}
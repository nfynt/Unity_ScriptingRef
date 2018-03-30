Shader "Unlit/Shubham/Custom"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_TintColor("Tint Color",Color)=(1,1,1,1)
		_Transparency("Transparency",Range(0,1))=0.5
		_CutoutThres("Cutout Threshold",Range(0,1))=0.2
		_Distance("Distance",float) = 1
		_Amplitude("Amplitude",float) = 1
		_Speed("Speed",float) = 1
		_Amount("Amount",Range(0.0,1.0)) = 1
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 100

		ZWrite off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _TintColor;
			float _Transparency;
			float _CutoutThres;
			float _Distance;
			float _Amplitude;
			float _Speed;
			float _Amount;
			
			v2f vert (appdata v)
			{
				v2f o;
				v.vertex.x += sin(_Time.y * _Speed + v.vertex.y * _Amplitude) * _Distance * _Amount;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv) + _TintColor;
				col.a = _Transparency;
				clip(col.r - _CutoutThres);
				return col;
			}
			ENDCG
		}
	}
}

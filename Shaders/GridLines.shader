Shader "Nfynt/Unlit/GridLines"
{
	Properties
	{
		_GridSize("Grid Size", Float) = 1
		_GridThickness("Thickness", Range(0.01,0.5)) = 0.01
    _GridColor ("Color", Color) = (1,0,0,1)
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" }
		LOD 100
		ZTest Always

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
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

	
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = mul(unity_ObjectToWorld, v.vertex).xz;
				return o;
			}

	
			float _GridSize;
			float _GridThickness;
			float4 _GridColor;

			float DrawGrid(float2 uv, float sz, float aaThresh)
			{
			   float aaMin = aaThresh*0.1;

			   float2 gUV = uv / sz + aaThresh;
			    
			   gUV = frac(gUV);
			   gUV -= aaThresh;
			   gUV = smoothstep(aaThresh, aaMin, abs(gUV));
			   float d = max(gUV.x, gUV.y);

			   return d;
			}

			fixed4 frag (v2f i) : SV_Target
			{   
			    
				fixed r = DrawGrid(i.uv, _GridSize, _GridThickness);
				return _GridColor*r;
			}
			ENDCG
		}	
	}
}


/*
 __  _ _____   ____  _ _____  
|  \| | __\ `v' /  \| |_   _| 
| | ' | _| `. .'| | ' | | |   
|_|\__|_|   !_! |_|\__| |_|
 

*/
